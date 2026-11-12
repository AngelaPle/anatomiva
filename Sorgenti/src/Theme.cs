using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace Anatomiva {
 static class Theme {
  public static Color Ink=Color.FromArgb(25,48,61),Muted=Color.FromArgb(90,110,119),Teal=Color.FromArgb(16,113,111),Mint=Color.FromArgb(225,244,238),Paper=Color.FromArgb(245,248,247),Line=Color.FromArgb(216,228,226),Gold=Color.FromArgb(244,191,85),Red=Color.FromArgb(165,52,52);
  static System.Collections.Generic.Dictionary<string,Font> fonts=new System.Collections.Generic.Dictionary<string,Font>();
  public static Font Font(float size,bool bold=false){string key=size.ToString(System.Globalization.CultureInfo.InvariantCulture)+bold;Font f;if(!fonts.TryGetValue(key,out f)){f=new Font("Segoe UI",size,bold?FontStyle.Bold:FontStyle.Regular);fonts[key]=f;}return f;}
 }
 class MapPanel:Panel {
  public string Selected=""; public Action<string> SelectCategory;public string Language="it";public bool Interactive;
  static string[] ids={"head","hair","eye","ear","shoulder","arm","hand","finger","leg","knee","foot","toe"};
  static PointF[] points={new PointF(.50f,.12f),new PointF(.34f,.05f),new PointF(.55f,.20f),new PointF(.65f,.23f),new PointF(.37f,.34f),new PointF(.27f,.42f),new PointF(.17f,.48f),new PointF(.82f,.49f),new PointF(.42f,.67f),new PointF(.57f,.74f),new PointF(.40f,.92f),new PointF(.63f,.92f)};
  public MapPanel(bool interactive){Interactive=interactive;DoubleBuffered=true;BackColor=Color.White;AccessibleName="Body illustration";if(interactive)for(int i=0;i<ids.Length;i++){
   string id=ids[i];var b=new Button{Text=(i+1).ToString(),Name="map-"+id,FlatStyle=FlatStyle.Flat,Size=new Size(27,27),BackColor=Theme.Teal,ForeColor=Color.White,Font=Theme.Font(9,true),Cursor=Cursors.Hand,AccessibleName=id,TabStop=true};b.FlatAppearance.BorderSize=0;b.Click+=(s,e)=>{if(SelectCategory!=null)SelectCategory(id);};Controls.Add(b);
  }}
  RectangleF Area(){float h=Math.Min(Height-20,(Width-20)*1.3f),w=h/1.3f;return new RectangleF((Width-w)/2,(Height-h)/2,w,h);}
  protected override void OnResize(EventArgs e){base.OnResize(e);var a=Area();for(int i=0;i<Controls.Count;i++){Controls[i].Location=new Point((int)(a.X+points[i].X*a.Width-13),(int)(a.Y+points[i].Y*a.Height-13));}Invalidate();}
  protected override void OnPaint(PaintEventArgs e){
   base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;var a=Area();if(a.Width<=0)return;var save=g.Save();g.TranslateTransform(a.X,a.Y);g.ScaleTransform(a.Width/400,a.Height/520);
   using(var shadow=new SolidBrush(Theme.Mint))g.FillEllipse(shadow,75,468,250,25);
   Color skin=Color.FromArgb(227,170,130),dark=Color.FromArgb(105,63,50);
   using(var pen=new Pen(skin,39)){pen.StartCap=pen.EndCap=LineCap.Round;g.DrawLine(pen,162,307,162,433);g.DrawLine(pen,238,307,238,433);g.DrawLine(pen,157,170,95,216);g.DrawLine(pen,95,216,63,244);g.DrawLine(pen,243,170,305,216);g.DrawLine(pen,305,216,337,244);}
   using(var b=new SolidBrush(skin)){g.FillEllipse(b,125,425,63,40);g.FillEllipse(b,216,425,63,40);g.FillRectangle(b,177,122,46,52);g.FillEllipse(b,128,155,144,171);g.FillEllipse(b,126,61,31,58);g.FillEllipse(b,243,61,31,58);g.FillEllipse(b,143,31,114,126);}
   using(var b=new SolidBrush(Color.FromArgb(59,119,140))){g.FillRectangle(b,132,260,136,53);g.FillRectangle(b,132,299,57,35);g.FillRectangle(b,211,299,57,35);}
   using(var b=new SolidBrush(Color.FromArgb(239,191,83))){g.FillEllipse(b,132,154,136,69);g.FillRectangle(b,143,183,114,80);}
   using(var b=new SolidBrush(dark)){g.FillEllipse(b,140,19,120,60);g.FillEllipse(b,136,33,35,58);g.FillEllipse(b,230,30,30,60);g.FillEllipse(b,169,84,10,14);g.FillEllipse(b,221,84,10,14);}
   using(var p=new Pen(dark,3)){g.DrawArc(p,180,103,39,23,10,160);}
   using(var p=new Pen(skin,8)){p.StartCap=p.EndCap=LineCap.Round;for(int j=0;j<4;j++){g.DrawLine(p,61,239+j*5,40-j*3,235+j*11);g.DrawLine(p,339,239+j*5,360+j*3,235+j*11);}}
   g.Restore(save);
  }
 }
}
