using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
namespace Anatomiva {
 class JointCanvas:Panel {
  public LabState State;public string Language;bool English{get{return Language=="en";}}
  public JointCanvas(){DoubleBuffered=true;BackColor=Color.FromArgb(241,248,246);AccessibleName="Interactive joint schematic";}
  void DrawText(Graphics g,string text,float x,float y,int width,float size=11,Color? color=null){using(var b=new SolidBrush(color??Theme.Ink))g.DrawString(text,Theme.Font(size),b,new RectangleF(x,y,width,55));}
  void Segment(Graphics g,PointF a,PointF b,Color color,int width=16,bool dotted=false){using(var p=new Pen(color,width)){p.StartCap=p.EndCap=LineCap.Round;if(dotted)p.DashStyle=DashStyle.Dot;g.DrawLine(p,a,b);}}
  protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);if(State==null||Width<1||Height<1)return;var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;float scale=Math.Min(Width/440f,Height/360f);var save=g.Save();g.TranslateTransform((Width-440*scale)/2,(Height-360*scale)/2);g.ScaleTransform(scale,scale);
   var m=State.Module;
   if(m.IsLever){
    PointF pivot=new PointF(55,142),end=new PointF(380,142);Segment(g,new PointF(55,48),pivot,Theme.Ink);Segment(g,pivot,end,Theme.Teal);float x=55+State.DistanceCm/35f*310;
    using(var b=new SolidBrush(Theme.Gold))g.FillRectangle(b,x-17,146,34,33);
    using(var p=new Pen(Theme.Red,3)){p.EndCap=LineCap.ArrowAnchor;g.DrawLine(p,x,181,x,228);}
    using(var p=new Pen(Theme.Muted,1)){g.DrawLine(p,55,107,x,107);g.DrawLine(p,55,100,55,113);g.DrawLine(p,x,100,x,113);}
    DrawText(g,"d = "+State.DistanceCm+" cm",90,74,220,12);DrawText(g,(State.MassTenths/10.0).ToString("F1")+" kg",x-30,235,90,10);DrawText(g,"F = "+(State.MassTenths/10.0*Mechanics.Gravity).ToString("F2")+" N",260,180,170,10,Theme.Red);
    double torque=Mechanics.ExternalTorque(State.MassTenths/10.0,State.DistanceCm/100.0),reference=Mechanics.ExternalTorque(State.MassTenths/10.0,.30);
    DrawText(g,(English?"Current: ":"Attuale: ")+torque.ToString("F2")+" N·m",20,276,240,11);DrawText(g,(English?"Same mass at 30 cm: ":"Stessa massa a 30 cm: ")+reference.ToString("F2")+" N·m",20,318,300,10);
    using(var b=new SolidBrush(Theme.Teal))g.FillRectangle(b,300,281,(float)(110*torque/10.31),13);using(var b=new SolidBrush(Color.FromArgb(171,187,190)))g.FillRectangle(b,300,322,(float)(110*reference/10.31),13);
   }else{
    bool ankle=m.Id=="ankle";PointF pivot=ankle?new PointF(200,215):new PointF(170,167),top=ankle?new PointF(200,55):new PointF(170,50);double[] offset=ankle?Mechanics.FootEndpoint(135,State.Angle):Mechanics.HingeEndpoint(135,State.Angle);PointF end=new PointF(pivot.X+(float)offset[0],pivot.Y+(float)offset[1]);PointF reference=ankle?new PointF(pivot.X+135,pivot.Y):new PointF(pivot.X,pivot.Y+135);
    double[] target=ankle?Mechanics.FootEndpoint(135,m.TargetAngle):Mechanics.HingeEndpoint(135,m.TargetAngle);using(var p=new Pen(Theme.Gold,3)){p.DashStyle=DashStyle.Dash;g.DrawEllipse(p,pivot.X+(float)target[0]-16,pivot.Y+(float)target[1]-16,32,32);}
    Segment(g,pivot,reference,Color.FromArgb(179,197,198),5,true);Segment(g,top,pivot,Theme.Ink);Segment(g,pivot,end,Theme.Teal);
    using(var p=new Pen(Theme.Gold,3))g.DrawArc(p,pivot.X-43,pivot.Y-43,86,86,ankle?0:90,ankle?State.Angle:-State.Angle);
    using(var b=new SolidBrush(Theme.Gold))g.FillEllipse(b,pivot.X-10,pivot.Y-10,20,20);
    DrawText(g,State.Angle+"°",300,35,110,28,Theme.Teal);DrawText(g,English?"fixed segment":"segmento fisso",25,61,135,10);DrawText(g,English?"moving segment":"segmento mobile",290,115,135,10);DrawText(g,English?"Pale line: reference · Gold ring: mission target":"Linea chiara: riferimento · Anello oro: bersaglio",24,323,390,9,Theme.Muted);
   }
   g.Restore(save);
  }
 }
 class LabView:UserControl {
  public LabState State;string lang;float scale;Action<LabTrial> save;JointCanvas canvas;Panel simulation,controls,mission,feedback;Label reading,feedbackText;TrackBar angle,distance,mass;RadioButton[] choices;Button evaluate,animate;Timer timer;bool syncing=false;int direction=1;
  public bool Animating{get{return timer.Enabled;}}
  string T(string it,string en){return lang=="en"?en:it;}
  Label L(string text,float size=11,bool bold=false,Color? color=null){return new Label{Text=text,Font=Theme.Font(size*scale,bold),ForeColor=color??Theme.Ink,UseMnemonic=false};}
  Button B(string text,Action click,string name){var b=new Button{Text=text,Name=name,BackColor=Theme.Teal,ForeColor=Color.White,FlatStyle=FlatStyle.Flat,Font=Theme.Font(11*scale,true),Cursor=Cursors.Hand,AccessibleName=text};b.FlatAppearance.BorderSize=0;b.Click+=(s,e)=>click();return b;}
  void Put(Panel panel,Control c,int x,int y,int w,int h){c.SetBounds(x,y,w,h);panel.Controls.Add(c);}
  public LabView(LabState state,string language,float textScale,Action<LabTrial> onSave){
   State=state;lang=language;scale=textScale;save=onSave;BackColor=Color.White;Height=1010;Name="lab-view";timer=new Timer{Interval=45};timer.Tick+=(s,e)=>{int next=angle.Value+direction*2;if(next>=angle.Maximum){next=angle.Maximum;direction=-1;}if(next<=angle.Minimum){next=angle.Minimum;direction=1;}angle.Value=next;};
   simulation=new Panel{Location=new Point(18,18),Height=375};Controls.Add(simulation);canvas=new JointCanvas{State=State,Language=lang};simulation.Controls.Add(canvas);controls=new Panel();simulation.Controls.Add(controls);
   string hint=state.Module.Id=="elbow"?T("Obiettivo: 90° di flessione (±5°). Poi identifica il movimento.","Target: 90° flexion (±5°). Then identify the movement."):state.Module.Id=="knee"?T("Obiettivo: 60° di flessione (±5°). Poi identifica il muscolo che raddrizza il ginocchio.","Target: 60° flexion (±5°). Then identify the muscle that straightens the knee."):state.Module.Id=="ankle"?T("Obiettivo: −15° (±3°), con la punta verso la gamba. Poi identifica il movimento.","Target: −15° (±3°), toes toward the leg. Then identify the movement."):T("Mantieni 1,0 kg e porta il carico a ≤12 cm dal gomito: obiettivo ≤1,20 N·m.","Keep 1.0 kg and move the load to ≤12 cm from the elbow: target ≤1.20 N·m.");
   var desc=L(hint,11,true,Theme.Muted);desc.Name="lab-description";Put(controls,desc,14,6,300,96);
   reading=L("",17,true,Theme.Teal);Put(controls,reading,14,103,300,48);
   if(state.Module.IsLever){
    var dl=L(T("Distanza del carico dal gomito (cm)","Load distance from elbow (cm)"),10,true);Put(controls,dl,14,151,300,32);
    distance=new TrackBar{Minimum=5,Maximum=35,TickFrequency=5,Value=state.DistanceCm,Name="lab-distance",AccessibleName=T("Distanza del carico in centimetri","Load distance in centimeters")};Put(controls,distance,10,185,300,45);
    var ml=L(T("Massa del carico (kg)","Load mass (kg)"),10,true);Put(controls,ml,14,234,300,32);
    mass=new TrackBar{Minimum=5,Maximum=30,TickFrequency=5,Value=state.MassTenths,Name="lab-mass",AccessibleName=T("Massa del carico, decimi di chilogrammo","Load mass in tenths of a kilogram")};Put(controls,mass,10,270,300,45);
    distance.ValueChanged+=(s,e)=>{if(syncing)return;State.DistanceCm=distance.Value;Changed();};mass.ValueChanged+=(s,e)=>{if(syncing)return;State.MassTenths=mass.Value;Changed();};
   }else{
    var caption=L(state.Module.Id=="ankle"?T("− dorsiflessione   |   + flessione plantare","− dorsiflexion   |   + plantar flexion"):T("Flessione dal riferimento esteso (gradi)","Flexion from the straight reference (degrees)"),10,true);Put(controls,caption,14,155,300,48);
    angle=new TrackBar{Minimum=state.Module.MinAngle,Maximum=state.Module.MaxAngle,TickFrequency=state.Module.Id=="ankle"?10:20,Value=state.Angle,Name="lab-angle",AccessibleName=T("Angolo del modello in gradi","Model angle in degrees")};Put(controls,angle,10,210,300,50);angle.ValueChanged+=(s,e)=>{if(syncing)return;State.Angle=angle.Value;Changed();};
    animate=B(T("Anima","Animate"),()=>{timer.Enabled=!timer.Enabled;animate.Text=timer.Enabled?T("Pausa","Pause"):T("Anima","Animate");},"lab-animate");Put(controls,animate,14,276,130,42);
   }
   var reset=B(T("Ripristina","Reset model"),ResetModel,"lab-reset");Put(controls,reset,State.Module.IsLever?14:155,325,155,40);
   var limits=L(T("Schema 2D: i limiti del cursore non sono valori clinici di ROM.","2D schematic: slider limits are not clinical ROM values."),10,false,Theme.Muted);limits.SetBounds(22,407,700,45);Controls.Add(limits);
   mission=new Panel{Location=new Point(18,461),Height=365,BackColor=Theme.Paper};Controls.Add(mission);
   Put(mission,L(T("OSSERVA · IMPOSTA · SPIEGA","OBSERVE · SET · EXPLAIN"),11,true,Theme.Teal),18,14,650,28);
   Put(mission,L(state.Module.Goal.Get(lang),12,true),18,49,650,85);
   Put(mission,L(state.Module.Question.Get(lang),11),18,141,650,56);
   choices=new RadioButton[4];for(int i=0;i<4;i++){int choice=i;choices[i]=new RadioButton{Text=state.Module.Choices[i].Get(lang),Name="lab-choice-"+i,Font=Theme.Font(10.5f*scale),ForeColor=Theme.Ink,Checked=state.Choice==i,AccessibleName=state.Module.Choices[i].Get(lang)};Put(mission,choices[i],18,205+i*35,650,34);choices[i].CheckedChanged+=(s,e)=>{if(choices[choice].Checked&&!syncing){State.Choice=choice;Changed();}};}
   evaluate=B(T("Verifica la missione →","Check the mission →"),Evaluate,"lab-evaluate");evaluate.SetBounds(18,840,650,48);Controls.Add(evaluate);
   feedback=new Panel{Location=new Point(18,907),Height=150,BackColor=Theme.Mint};Controls.Add(feedback);feedbackText=L(T("Muovi i cursori, scegli una risposta e verifica la missione. Puoi riprovare: ogni prova viene salvata nei progressi.","Move the sliders, choose an answer and check the mission. You can retry: every check is saved in your progress."),11);Put(feedback,feedbackText,18,15,650,120);Height=1080;
   Resize+=(s,e)=>LayoutParts();LayoutParts();UpdateReadings();
  }
  void LayoutParts(){int w=Math.Max(600,Width-36);simulation.Width=w;int left=(int)(w*.48);canvas.SetBounds(0,0,left,375);controls.SetBounds(left+10,0,w-left-10,375);foreach(Control c in controls.Controls)if(!(c is Button))c.Width=Math.Max(180,controls.Width-24);foreach(Control c in Controls)if(c is Label)c.Width=w;mission.Width=w;feedback.Width=w;evaluate.Width=w;foreach(Control c in mission.Controls)c.Width=w-36;feedbackText.Width=w-36;}
  void Changed(){evaluate.Enabled=true;feedback.BackColor=Theme.Mint;feedbackText.Text=T("Modello aggiornato. Scegli la risposta e verifica la missione.","Model updated. Choose an answer and check the mission.");UpdateReadings();}
  void UpdateReadings(){reading.Text=State.Module.IsLever?Mechanics.ExternalTorque(State.MassTenths/10.0,State.DistanceCm/100.0).ToString("F2")+" N·m  ·  "+(State.MassTenths/10.0).ToString("F1")+" kg":State.Angle+"°";canvas.Invalidate();}
  void ResetModel(){timer.Stop();if(animate!=null)animate.Text=T("Anima","Animate");syncing=true;State.Angle=State.Module.StartAngle;State.DistanceCm=30;State.MassTenths=10;State.Choice=-1;if(angle!=null)angle.Value=State.Angle;if(distance!=null)distance.Value=30;if(mass!=null)mass.Value=10;foreach(var radio in choices)radio.Checked=false;syncing=false;Changed();}
  void Evaluate(){
   timer.Stop();if(animate!=null)animate.Text=T("Anima","Animate");if(State.Choice<0){feedbackText.Text=T("Prima scegli una risposta alla domanda della missione.","First choose an answer to the mission question.");return;}
   var trial=State.Evaluate();try{save(trial);}catch(Exception ex){feedback.BackColor=Color.FromArgb(252,231,226);feedbackText.Text=T("Prova NON salvata. Puoi riprovare il salvataggio. ","Trial NOT saved. You can retry saving. ")+ex.Message;return;}
   evaluate.Enabled=false;feedback.BackColor=trial.Passed?Theme.Mint:Color.FromArgb(255,244,219);
   feedbackText.Text=(trial.Passed?T("✓ Missione completata e salvata. ","✓ Mission completed and saved. "):T("Da rivedere: ","Try again: "))+(!trial.NumericGoal?T("imposta i valori richiesti dalla missione. ","set the values requested by the mission. "):"")+(!trial.ConceptGoal?T("La risposta corretta è: ","The correct answer is: ")+State.Module.Choices[State.Module.CorrectChoice].Get(lang)+". ":"")+State.Module.Explanation.Get(lang);
   var scroll=Parent as ScrollableControl;if(scroll!=null)scroll.ScrollControlIntoView(feedback);
  }
  protected override void Dispose(bool disposing){if(disposing&&timer!=null)timer.Dispose();base.Dispose(disposing);}
 }
}
