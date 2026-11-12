using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace Anatomiva {
 public partial class MainForm:Form {
  Catalog catalog;Storage storage;Panel body,sidebar,header;FlowLayoutPanel flow;Label pageTitle;string page="home",chosen="head";QuizSession session;Attempt displayed;bool resultSaved;List<Button> options=new List<Button>();
  public MainForm(Catalog cat,Storage store){
   catalog=cat;storage=store;Text="Anatomiva · Angela Plescia";Name="Anatomiva";ClientSize=new Size(1180,800);MinimumSize=new Size(1024,740);StartPosition=FormStartPosition.CenterScreen;BackColor=Theme.Paper;AutoScaleMode=AutoScaleMode.Dpi;KeyPreview=true;
   using(var s=System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("app.ico"))if(s!=null)Icon=new Icon(s);
   FormClosing+=(s,e)=>{if(session!=null&&session.Result==null&&!ConfirmLeave())e.Cancel=true;};KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Escape){Navigate("home");e.Handled=true;}};
   BuildFrame();Navigate("home");
  }
  string Lang{get{return storage.Data.Language;}}float ScaleText{get{return storage.Data.LargeText?1.15f:1f;}}
  string T(string it,string en){return Lang=="en"?en:it;}
  Label Label(string text,float size=12,bool bold=false,Color? color=null){return new Label{Text=text,Font=Theme.Font(size*ScaleText,bold),ForeColor=color??Theme.Ink,AutoSize=false,BackColor=Color.Transparent,UseMnemonic=false};}
  Button Button(string text,Action action,bool primary=false,string name=""){
   var b=new Button{Text=text,Name=name,AutoSize=false,Height=48,Width=210,FlatStyle=FlatStyle.Flat,BackColor=primary?Theme.Teal:Color.White,ForeColor=primary?Color.White:Theme.Ink,Font=Theme.Font(11*ScaleText,true),Cursor=Cursors.Hand,TextAlign=ContentAlignment.MiddleCenter,UseVisualStyleBackColor=false,AccessibleName=text,Margin=new Padding(0,0,12,10)};
   b.FlatAppearance.BorderColor=primary?Theme.Teal:Theme.Line;b.FlatAppearance.MouseOverBackColor=primary?Color.FromArgb(12,91,91):Theme.Mint;b.Click+=(s,e)=>action();return b;
  }
  void BuildFrame(){
   foreach(Control old in Controls.Cast<Control>().ToArray())old.Dispose();Controls.Clear();sidebar=new Panel{Dock=DockStyle.Left,Width=200,BackColor=Theme.Ink,Padding=new Padding(20)};
   header=new Panel{Dock=DockStyle.Top,Height=88,BackColor=Color.White,Padding=new Padding(30,20,30,15)};
   body=new Panel{Dock=DockStyle.Fill,Padding=new Padding(28,22,24,16),BackColor=Theme.Paper};
   flow=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,BackColor=Theme.Paper};body.Controls.Add(flow);
   Controls.Add(body);Controls.Add(header);Controls.Add(sidebar);
   var brand=Label("ANATOMIVA",15/ScaleText,true,Color.White);brand.SetBounds(20,28,166,80);sidebar.Controls.Add(brand);
   var tag=Label(T("Scopri il corpo.\nComprendi il movimento.","Explore the body.\nUnderstand movement."),10,false,Color.FromArgb(170,201,204));tag.SetBounds(20,104,166,76);sidebar.Controls.Add(tag);
   var items=new[]{new[]{"home",T("Home","Home")},new[]{"explore",T("Esplora il corpo","Explore the body")},new[]{"quiz",T("Mettiti alla prova","Take a quiz")},new[]{"lab",T("Laboratorio","Movement lab")},new[]{"review",T("Ripasso mirato","Targeted review")},new[]{"progress",T("I tuoi progressi","Your progress")},new[]{"help",T("Come si gioca","How to play")},new[]{"settings",T("Impostazioni","Settings")}};
   int y=194;foreach(var item in items){string id=item[0];var b=Button(item[1],()=>Navigate(id),false,"nav-"+id);b.SetBounds(12,y,176,46);b.BackColor=Theme.Ink;b.ForeColor=Color.White;b.FlatAppearance.BorderSize=0;b.FlatAppearance.MouseOverBackColor=Theme.Teal;b.TextAlign=ContentAlignment.MiddleLeft;b.Font=Theme.Font(10,true);sidebar.Controls.Add(b);y+=49;}
   var credit=Label("ANGELA PLESCIA\nAnatomiva 2.0",9,false,Color.FromArgb(170,201,204));credit.SetBounds(20,sidebar.Height-78,166,46);credit.Anchor=AnchorStyles.Left|AnchorStyles.Bottom;sidebar.Controls.Add(credit);
   pageTitle=Label("",19,true);pageTitle.Dock=DockStyle.Fill;header.Controls.Add(pageTitle);
   var language=Button(Lang=="it"?"IT  /  EN":"EN  /  IT",()=>ChangeLanguage(),false,"language");language.Dock=DockStyle.Right;language.Width=110;header.Controls.Add(language);
   flow.Resize+=(s,e)=>ResizeCards();
  }
  void ResizeCards(){foreach(Control c in flow.Controls)c.Width=Math.Max(300,flow.ClientSize.Width-24);}
  Panel Card(int height=180){var p=new Panel{Height=height,Width=Math.Max(300,flow.ClientSize.Width-24),BackColor=Color.White,Margin=new Padding(0,0,0,18),Padding=new Padding(22)};flow.Controls.Add(p);return p;}
  void TextBlock(Panel p,string text,int x,int y,int width,int height,float size=12,bool bold=false,Color? color=null){var l=Label(text,size,bold,color);l.SetBounds(x,y,width,height);p.Controls.Add(l);}
  void Intro(string title,string subtitle){pageTitle.Text=title;var p=Card(78);p.BackColor=Theme.Paper;TextBlock(p,subtitle,0,8,p.Width-10,68,12,false,Theme.Muted);}
  bool ConfirmLeave(){return MessageBox.Show(this,T("Abbandonare questo quiz? Il tentativo in corso non verrà salvato.","Leave this quiz? The current attempt will not be saved."),"Anatomiva",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes;}
  void Navigate(string target){if(session!=null&&session.Result==null&&target!="question"&&!ConfirmLeave())return;if(target!="question"&&target!="result")session=null;page=target;Render();}
  void Render(){
   flow.SuspendLayout();foreach(Control c in flow.Controls.Cast<Control>().ToArray())c.Dispose();flow.Controls.Clear();flow.AutoScrollPosition=Point.Empty;options.Clear();
   if(page=="home")Home();else if(page=="explore"||page=="quiz")Explore();else if(page=="question")QuestionPage();else if(page=="result")ResultPage();else if(page=="lab")LaboratoryPage();else if(page=="review")ReviewPage();else if(page=="progress")ProgressPage();else if(page=="settings")SettingsPage();else HelpPage();
   foreach(Control c in sidebar.Controls)if(c is Button)c.BackColor=c.Name=="nav-"+page?Theme.Teal:Theme.Ink;
   ResizeCards();flow.ResumeLayout(true);
  }
  void Home(){
   Intro(T("Ciao, esploratore!","Hello, explorer!"),T("Anatomia, movimento e piccoli esperimenti: osserva, prova, comprendi.","Anatomy, movement and small experiments: observe, try, understand."));
   var hero=Card(332);var art=new JointCanvas{State=new LabState("elbow"){Angle=85},Language=Lang,Dock=DockStyle.Right,Width=260,BackColor=Color.White};hero.Controls.Add(art);
   TextBlock(hero,T("IL CORPO\nPRENDE VITA.","BRING ANATOMY\nTO LIFE."),26,28,450,124,24,true);
   TextBlock(hero,T("Esplora il corpo, muovi le articolazioni e sperimenta\nle leve. Impara dai quiz e ripassa i tuoi errori.","Explore the body, move the joints and experiment\nwith levers. Learn from quizzes and revisit mistakes."),28,157,440,102,12,false,Theme.Muted);
   var start=Button(T("Apri il laboratorio →","Open the lab →"),()=>Navigate("lab"),true,"start");start.SetBounds(28,274,240,50);hero.Controls.Add(start);hero.Resize+=(s,e)=>{int w=Math.Max(280,hero.Width-art.Width-65);foreach(Control child in hero.Controls)if(child is Label)child.Width=w;};
   var pathways=Card(112);var pathrow=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=true};pathways.Controls.Add(pathrow);pathrow.Controls.Add(Button(T("Esplora il corpo","Explore the body"),()=>Navigate("explore"),false,"home-explore"));pathrow.Controls.Add(Button(T("Quiz di anatomia","Anatomy quizzes"),()=>Navigate("quiz"),false,"home-quiz"));pathrow.Controls.Add(Button(T("Ripasso mirato","Targeted review"),()=>Navigate("review"),false,"home-review"));
   var quick=Card(142);int attempts=storage.Data.Attempts.Count;int mastered=storage.Data.Attempts.Where(a=>a.Kind=="quiz").GroupBy(a=>a.CategoryId+"-"+a.Level).Count(g=>g.Max(a=>a.Score)==5);
   TextBlock(quick,"12",24,10,100,70,26,true,Theme.Teal);TextBlock(quick,T("parti del corpo","body parts"),24,88,180,44,11);
   TextBlock(quick,"120",quick.Width/3,10,100,70,26,true,Theme.Teal);TextBlock(quick,T("domande bilingui","bilingual questions"),quick.Width/3,88,210,44,11);
   TextBlock(quick,mastered+" / 24",quick.Width*2/3,10,160,70,26,true,Theme.Teal);TextBlock(quick,T("quiz con punteggio pieno","quizzes with a full score"),quick.Width*2/3,88,230,44,10);
   var last=Card(120);TextBlock(last,T("Il tuo percorso","Your journey"),22,16,last.Width-40,32,15,true);TextBlock(last,attempts==0?T("Nessun quiz ancora: ogni scoperta comincia da una domanda.","No quizzes yet: every discovery starts with a question."):T("Hai completato ","You have completed ")+attempts+T(" quiz. Ritrova i tuoi risultati nei progressi."," quizzes. Find your results in your progress."),22,57,last.Width-44,54,11,false,Theme.Muted);
  }
  void Explore(){
   Intro(page=="quiz"?T("Scegli il tuo quiz","Choose your quiz"):T("Esplora il corpo","Explore the body"),T("Seleziona una parte del corpo dalla lista o dai numeri sulla figura. Poi scegli un livello.","Select a body part from the list or the numbers on the figure. Then choose a level."));
   var p=Card(626);var map=new MapPanel(true){Dock=DockStyle.Right,Width=300,Selected=chosen,SelectCategory=id=>{chosen=id;Render();}};p.Controls.Add(map);
   int leftWidth=p.Width-340;
   var cats=new TableLayoutPanel{ColumnCount=2,RowCount=6,Location=new Point(22,20),Size=new Size(leftWidth,288)};cats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));cats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));for(int rowIndex=0;rowIndex<6;rowIndex++)cats.RowStyles.Add(new RowStyle(SizeType.Percent,100f/6));
   for(int i=0;i<catalog.Categories.Count;i++){var cat=catalog.Categories[i];var b=Button((i+1).ToString("00")+"  "+cat.Name.Get(Lang),()=>{chosen=cat.Id;Render();},cat.Id==chosen,"category-"+cat.Id);b.Dock=DockStyle.Fill;b.Height=46;b.Margin=new Padding(0,0,9,8);cats.Controls.Add(b,i%2,i/2);}p.Controls.Add(cats);
   var selected=catalog.Categories.Single(c=>c.Id==chosen);TextBlock(p,selected.Name.Get(Lang),22,326,leftWidth,46,23,true);TextBlock(p,selected.Description.Get(Lang),22,379,leftWidth,96,12,false,Theme.Muted);
   var l1=Button(T("Livello 1 · Scopri","Level 1 · Discover"),()=>StartQuiz(selected,1),true,"level-1");l1.SetBounds(22,492,leftWidth,52);p.Controls.Add(l1);
   var l2=Button(T("Livello 2 · Approfondisci","Level 2 · Learn more"),()=>StartQuiz(selected,2),false,"level-2");l2.SetBounds(22,555,leftWidth,52);p.Controls.Add(l2);
   p.Resize+=(s,e)=>{int w=Math.Max(300,p.Width-map.Width-40);cats.Width=w;foreach(Control c in p.Controls)if(c is Label)c.Width=w;l1.Width=l2.Width=w;};
  }
  void StartQuiz(Category cat,int level){session=new QuizSession(cat,level,Lang,catalog.Version);resultSaved=false;page="question";Render();}
  void QuestionPage(){
   if(session==null){page="quiz";Explore();return;}
   var q=session.Feedback?session.Questions[session.Index-1]:session.Questions[session.Index];int number=session.Feedback?session.Index:session.Index+1;
   Intro(session.Kind=="review"?session.Category.Name.Get(Lang):session.Category.Name.Get(Lang)+" · "+T("Livello ","Level ")+session.Level,T("Domanda ","Question ")+number+" / "+session.Questions.Count+" · "+T("Prenditi il tempo che ti serve. Il tempo non modifica il punteggio.","Take your time. Time does not affect your score."));
   var p=Card(session.Feedback?558:510);var bar=new ProgressBar{Minimum=0,Maximum=session.Questions.Count,Value=number,Location=new Point(24,22),Size=new Size(p.Width-48,9),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};p.Controls.Add(bar);
   TextBlock(p,q.Prompt.Get(session.Language),24,52,p.Width-48,90,18,true);
   var ordered=session.DisplayOptions(q);
   for(int i=0;i<4;i++){var choice=ordered[i];var b=Button(((char)('A'+i))+"    "+choice.Text.Get(session.Language),()=>AnswerQuestion(choice.Id),false,"option-"+choice.Id);b.SetBounds(24,150+i*61,p.Width-48,52);b.TextAlign=ContentAlignment.MiddleLeft;b.Padding=new Padding(16,0,8,0);b.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
    if(session.Feedback){var a=session.Answers.Last();b.Enabled=false;if(choice.Correct){b.BackColor=Theme.Mint;b.Text="✓  "+choice.Text.Get(session.Language);}else if(choice.Text==a.Chosen){b.BackColor=Color.FromArgb(252,231,226);b.Text="×  "+choice.Text.Get(session.Language);}}
    options.Add(b);p.Controls.Add(b);
   }
   if(session.Feedback){var answer=session.Answers.Last();TextBlock(p,(answer.IsCorrect?T("Esatto! ","Correct! "):T("Scopriamo insieme. ","Let's learn together. "))+answer.Explanation.Get(session.Language),24,406,p.Width-48,80,12,true,answer.IsCorrect?Theme.Teal:Theme.Ink);
    var next=Button(session.Complete?T("Vedi il risultato →","See results →"):T("Prossima domanda →","Next question →"),()=>ContinueQuiz(),true,"continue");next.SetBounds(24,494,p.Width-48,48);next.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;p.Controls.Add(next);BeginInvoke((Action)(()=>{if(!next.IsDisposed){next.Focus();flow.ScrollControlIntoView(next);}}));
   }else TextBlock(p,T("Scegli una risposta. Riceverai subito una spiegazione.","Choose an answer. You will get an explanation straight away."),24,426,p.Width-48,64,11,false,Theme.Muted);
  }
  void AnswerQuestion(string id){if(session==null||!session.Submit(id))return;Render();}
  void ContinueQuiz(){if(session==null||!session.Continue())return;if(session.Complete){displayed=session.Result;TrySaveResult();page="result";}Render();}
  void TrySaveResult(){try{storage.Add(displayed);resultSaved=true;}catch(Exception ex){resultSaved=false;ShowError(ex);}}
  void ResultPage(){
   if(displayed==null){page="progress";ProgressPage();return;}var cat=FindCategory(displayed.CategoryId);
   Intro(T("Una nuova scoperta!","A new discovery!"),displayed.Kind=="review"?cat.Name.Get(Lang):cat.Name.Get(Lang)+" · "+T("Livello ","Level ")+displayed.Level);
   var score=Card(230);TextBlock(score,displayed.Score+" / "+displayed.Total,24,20,260,72,42,true,Theme.Teal);TextBlock(score,Math.Round(displayed.Score*100.0/displayed.Total)+"% · "+Math.Round(displayed.Seconds)+T(" secondi"," seconds"),27,103,score.Width-54,36,13);
   TextBlock(score,resultSaved?T("Risultato salvato sul tuo PC.","Result saved on your PC."):T("Risultato NON salvato. Riprova il salvataggio prima di uscire.","Result NOT saved. Retry saving before leaving."),27,154,score.Width-54,55,12,true,resultSaved?Theme.Teal:Theme.Red);
   var actions=Card(resultSaved?108:180);var row=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=true};actions.Controls.Add(row);
   if(!resultSaved)row.Controls.Add(Button(T("Salva di nuovo","Retry saving"),()=>{TrySaveResult();Render();},true,"retry-save"));
   row.Controls.Add(Button(displayed.Kind=="review"?T("Torna al ripasso","Back to review"):T("Riprova","Try again"),()=>{if(displayed.Kind=="review")Navigate("review");else StartQuiz(cat,displayed.Level);},true,"retry"));if(displayed.Kind=="quiz"&&displayed.Level==1)row.Controls.Add(Button(T("Livello successivo","Next level"),()=>StartQuiz(cat,2),false,"next-level"));row.Controls.Add(Button("Home",()=>Navigate("home"),false,"result-home"));
   foreach(var a in displayed.Answers){var p=Card(195);TextBlock(p,(a.IsCorrect?"✓  ":"×  ")+a.Prompt.Get(Lang),22,16,p.Width-44,58,12,true);TextBlock(p,T("La tua risposta: ","Your answer: ")+a.Chosen.Get(Lang)+(a.IsCorrect?"":"   ·   "+T("Corretta: ","Correct: ")+a.Correct.Get(Lang)),22,78,p.Width-44,50,11,false,a.IsCorrect?Theme.Teal:Theme.Red);TextBlock(p,a.Explanation.Get(Lang),22,130,p.Width-44,62,11,false,Theme.Muted);}
  }
  void ProgressPage(){
   Intro(T("I tuoi progressi","Your progress"),T("Ogni tentativo completato resta sul tuo PC. Il punteggio migliore è calcolato per categoria e livello.","Every completed attempt stays on your PC. Your best score is calculated per category and level."));
   var p=Card(430);var grid=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,AllowUserToResizeRows=false,RowHeadersVisible=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,BackgroundColor=Color.White,BorderStyle=BorderStyle.None,SelectionMode=DataGridViewSelectionMode.FullRowSelect,Font=Theme.Font(10*ScaleText),Name="progress-grid"};
   grid.ColumnHeadersDefaultCellStyle.BackColor=Theme.Mint;grid.EnableHeadersVisualStyles=false;grid.RowTemplate.Height=29;grid.Columns.Add("category",T("Parte del corpo","Body part"));grid.Columns.Add("level",T("Livello","Level"));grid.Columns.Add("attempts",T("Tentativi","Attempts"));grid.Columns.Add("best",T("Migliore","Best"));grid.Columns.Add("last",T("Ultimo","Latest"));
   foreach(var cat in catalog.Categories)for(int level=1;level<=2;level++){var list=storage.Data.Attempts.Where(a=>a.Kind=="quiz"&&a.CategoryId==cat.Id&&a.Level==level).OrderBy(a=>a.CompletedUtc).ToList();grid.Rows.Add(cat.Name.Get(Lang),level,list.Count,list.Count==0?"—":list.Max(a=>a.Score)+" / 5",list.Count==0?"—":list.Last().Score+" / 5");}p.Controls.Add(grid);
   LabProgressCards();
   var history=storage.Data.Attempts.OrderByDescending(a=>a.CompletedUtc).ToList();if(history.Count==0){var empty=Card(100);TextBlock(empty,T("Completa il tuo primo quiz per iniziare la cronologia.","Complete your first quiz to start your history."),22,25,empty.Width-44,60,12,false,Theme.Muted);}else{
    var h=Card(70);TextBlock(h,T("Cronologia · clicca un risultato per rileggerlo","History · select a result to review it"),22,18,h.Width-44,40,13,true);
    foreach(var a in history.Take(30)){var card=Card(78);var item=a;var cat=FindCategory(a.CategoryId);var b=Button(a.CompletedUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm")+"  ·  "+cat.Name.Get(Lang)+(a.Kind=="review"?"":"  ·  L"+a.Level)+"  ·  "+a.Score+" / "+a.Total,()=>{displayed=item;resultSaved=true;session=null;page="result";Render();});b.Dock=DockStyle.Fill;card.Controls.Add(b);}
    if(history.Count>30){var n=Card(70);TextBlock(n,T("Mostrati gli ultimi 30 risultati; il riepilogo include tutti i tentativi.","Showing the last 30 results; the summary includes every attempt."),22,15,n.Width-44,50,11);}
   }
   var tools=Card(86);var export=Button(T("Esporta cronologia CSV","Export history CSV"),ExportHistory,false,"export");export.SetBounds(22,18,250,48);tools.Controls.Add(export);
  }
  void ExportHistory(){using(var dlg=new SaveFileDialog{Filter="CSV (*.csv)|*.csv",FileName="Anatomiva-progressi.csv"})if(dlg.ShowDialog(this)==DialogResult.OK){try{ProgressExport.Write(dlg.FileName,storage.Data);MessageBox.Show(this,T("Esportazione completata.","Export completed."));}catch(Exception ex){ShowError(ex);}}}
  void ChangeLanguage(){if(session!=null&&session.Result==null&&!ConfirmLeave())return;try{storage.Settings(Lang=="it"?"en":"it",storage.Data.LargeText);session=null;if(page=="question")page="quiz";BuildFrame();Render();}catch(Exception ex){ShowError(ex);}}
  void SettingsPage(){
   Intro(T("Impostazioni","Settings"),T("Personalizza la lettura. Le preferenze vengono salvate automaticamente sul PC.","Make reading comfortable. Preferences are saved automatically on your PC."));
   var p=Card(260);TextBlock(p,T("Lingua dell'app e dei quiz","App and quiz language"),24,24,p.Width-48,40,15,true);var lang=Button(Lang=="it"?"Passa a English":"Switch to Italiano",ChangeLanguage,true,"settings-language");lang.SetBounds(24,78,280,48);p.Controls.Add(lang);
   var large=new CheckBox{Text=T("Testo più grande","Larger text"),Checked=storage.Data.LargeText,Location=new Point(24,157),Size=new Size(420,55),Font=Theme.Font(13*ScaleText),Name="large-text",ForeColor=Theme.Ink};large.CheckedChanged+=(s,e)=>{try{storage.Settings(Lang,large.Checked);Render();}catch(Exception ex){ShowError(ex);}};p.Controls.Add(large);
   var reset=Card(180);TextBlock(reset,T("Ricomincia il percorso","Start a new journey"),24,18,reset.Width-48,40,15,true);TextBlock(reset,T("Puoi cancellare i progressi. Le impostazioni vengono conservate.","You can delete progress. Your preferences will be kept."),24,62,reset.Width-48,50,11,false,Theme.Muted);var b=Button(T("Cancella progressi…","Delete progress…"),()=>{if(MessageBox.Show(this,T("Cancellare tutti i risultati? Questa operazione non può essere annullata dall'app.","Delete all results? This action cannot be undone in the app."),"Anatomiva",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)try{storage.Reset();Render();}catch(Exception ex){ShowError(ex);}},false,"reset");b.SetBounds(24,119,260,46);reset.Controls.Add(b);
  }
  void HelpPage(){
   Intro(T("Come si gioca","How to play"),T("Un piccolo laboratorio per scoprire il corpo umano, al tuo ritmo.","A little lab to discover the human body at your own pace."));
   var p=Card(450);TextBlock(p,T("01  ESPLORA\nScopri le 12 categorie e prova i quiz bilingui.\n\n02  SPERIMENTA\nNel Laboratorio muovi i cursori di gomito, ginocchio e caviglia.\nSperimenta come massa e distanza cambiano il momento di un carico.\n\n03  COMPLETA LE MISSIONI\nImposta il modello, scegli una risposta e premi Verifica.\nLeggi il feedback; se serve, modifica il modello e riprova.\n\n04  RIPASSA E RITROVA\nIl Ripasso mirato segue i tuoi errori più recenti. Nei Progressi\ntrovi quiz, missioni ed esportazione CSV.","01  EXPLORE\nDiscover 12 categories and try the bilingual quizzes.\n\n02  EXPERIMENT\nIn the Lab, move the elbow, knee and ankle sliders.\nExplore how mass and distance change the torque from a load.\n\n03  COMPLETE MISSIONS\nSet the model, choose an answer and press Check.\nRead the feedback; adjust the model and try again when needed.\n\n04  REVIEW AND TRACK\nTargeted Review follows your most recent mistakes. Progress\ncontains quizzes, missions and CSV export."),24,22,p.Width-48,412,12);
   var about=Card(300);TextBlock(about,T("Il progetto","About the project"),24,18,about.Width-48,40,16,true);TextBlock(about,T("Ideazione e prototipo originale: Angela Plescia.\n\nControlli: mouse, Tab / Shift+Tab, Invio / Spazio; Esc torna alla Home.\nApplicazione educativa offline, senza account o telemetria.\n\nRiferimenti: Nemours KidsHealth e OpenStax; dettagli in FONTI.md.\nI dati sono nella cartella personale dell'app. Non sono dati sanitari.","Original concept and prototype: Angela Plescia.\n\nControls: mouse, Tab / Shift+Tab, Enter / Space; Esc returns Home.\nOffline educational app, without accounts or telemetry.\n\nReferences: Nemours KidsHealth and OpenStax; details in FONTI.md.\nData stays in the app's user folder. No health data is collected."),24,70,about.Width-48,215,11,false,Theme.Muted);
  }
  void ShowError(Exception ex){MessageBox.Show(this,T("Non è stato possibile salvare i dati. Controlla lo spazio libero e i permessi della cartella personale.\n\n","Data could not be saved. Check free space and permissions for your user folder.\n\n")+ex.Message,"Anatomiva",MessageBoxButtons.OK,MessageBoxIcon.Error);}
 }
}
