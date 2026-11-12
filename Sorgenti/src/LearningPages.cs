using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Anatomiva {
 public partial class MainForm {
  LabState currentLab=new LabState("elbow");
  Category FindCategory(string id){return catalog.Categories.FirstOrDefault(c=>c.Id==id)??ReviewPlanner.ReviewCategory();}
  void LaboratoryPage(){
   Intro(T("Laboratorio del movimento","Movement laboratory"),T("Scegli un esperimento. Muovi i cursori, poi scorri per completare la missione e leggere il feedback.","Choose an experiment. Move the sliders, then scroll to complete the mission and read the feedback."));
   var tabs=Card(94);var row=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=4,RowCount=1};for(int i=0;i<4;i++)row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25));int col=0;
   foreach(var module in LabModule.All()){var item=module;var b=Button(module.IsLever?T("Leve","Levers"):module.Name.Get(Lang),()=>{currentLab=new LabState(item.Id);Render();},currentLab.Module.Id==module.Id,"lab-tab-"+module.Id);b.Dock=DockStyle.Fill;b.Margin=new Padding(0,0,8,0);row.Controls.Add(b,col++,0);}tabs.Controls.Add(row);
   var view=new LabView(currentLab,Lang,ScaleText,trial=>storage.AddLab(trial)){Width=Math.Max(600,flow.ClientSize.Width-24),Margin=new Padding(0,0,0,18)};flow.Controls.Add(view);
   var note=Card(152);TextBlock(note,T("Un modello per capire","A model for understanding"),22,14,note.Width-44,35,14,true);TextBlock(note,T("Le figure sono schemi didattici 2D. Angoli, carichi e obiettivi sono simulati: non misurano il tuo corpo e non sono esercizi prescritti. Nel modulo Leve si considera soltanto il momento esterno del carico, senza peso dell'arto o forze muscolari.","The figures are 2D teaching schematics. Angles, loads and targets are simulated: they do not measure your body and are not prescribed exercises. The Levers module considers only the load's external torque, without limb weight or muscle forces."),22,55,note.Width-44,90,11,false,Theme.Muted);
  }
  void ReviewPage(){
   var pending=ReviewPlanner.Pending(catalog,storage.Data.Attempts);
   Intro(T("Ripasso mirato","Targeted review"),T("Il ripasso segue i tuoi risultati: un concetto resta qui finché la sua risposta più recente è errata.","Review follows your results: a concept stays here while your most recent answer to it is incorrect."));
   var summary=Card(210);TextBlock(summary,pending.Count==0?T("Nessun errore da ripassare","No mistakes to revisit"):pending.Count+T(" concetti da consolidare"," concepts to revisit"),24,20,summary.Width-48,50,20,true,Theme.Teal);
   TextBlock(summary,pending.Count==0?T("Completa un quiz per iniziare, oppure prova il laboratorio. Una risposta corretta successiva toglie automaticamente il concetto dal ripasso.","Complete a quiz to begin, or try the lab. A later correct answer automatically removes a concept from review."):T("Ogni sessione ripropone fino a 5 domande, dando precedenza agli errori meno recenti. Le nuove risposte aggiornano il prossimo ripasso, senza modificare i risultati storici.","Each session revisits up to 5 questions, prioritizing older mistakes. New answers update the next review without changing your historical results."),24,83,summary.Width-48,94,12,false,Theme.Muted);
   var tools=Card(105);var b=Button(pending.Count==0?T("Scegli un quiz","Choose a quiz"):T("Inizia il ripasso →","Start review →"),()=>{if(pending.Count==0)Navigate("quiz");else StartReview();},true,"start-review");b.SetBounds(22,22,280,48);tools.Controls.Add(b);
   foreach(var question in pending.Take(12)){var q=question;var cat=catalog.Categories.Single(c=>c.Questions.Any(x=>x.Id==q.Id));var p=Card(145);TextBlock(p,cat.Name.Get(Lang)+" · "+T("Livello ","Level ")+q.Level,22,16,p.Width-44,30,11,true,Theme.Teal);TextBlock(p,q.Prompt.Get(Lang),22,54,p.Width-44,75,12);}
   if(pending.Count>12){var p=Card(75);TextBlock(p,T("Mostrati i primi 12 concetti; le sessioni includono anche gli altri.","Showing the first 12 concepts; review sessions also include the others."),22,16,p.Width-44,50,11);}
  }
  void StartReview(){var pending=ReviewPlanner.Pending(catalog,storage.Data.Attempts);if(pending.Count==0){Navigate("review");return;}session=new QuizSession(pending,Lang,catalog.Version);resultSaved=false;page="question";Render();}
  void LabProgressCards(){
   int completed=storage.Data.Labs.Where(t=>t.Passed).Select(t=>t.ModuleId).Distinct().Count();var p=Card(130);TextBlock(p,T("Missioni del laboratorio","Laboratory missions"),22,16,p.Width-44,35,16,true);TextBlock(p,completed+" / 4 "+T("completate","completed")+"  ·  "+storage.Data.Labs.Count+T(" prove salvate"," saved trials"),22,65,p.Width-44,50,13,false,Theme.Teal);
   foreach(var trial in storage.Data.Labs.OrderByDescending(t=>t.CompletedUtc).Take(12)){
    var card=Card(132);var m=LabModule.Find(trial.ModuleId);TextBlock(card,(trial.Passed?"✓  ":"↻  ")+m.Name.Get(Lang)+"  ·  "+trial.CompletedUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),22,16,card.Width-44,40,12,true,trial.Passed?Theme.Teal:Theme.Ink);
    string values=m.IsLever?(trial.MassTenths/10.0).ToString("F1")+" kg · "+trial.DistanceCm+" cm · "+trial.Torque.ToString("F2")+" N·m":trial.Angle+"°";
    TextBlock(card,values+"  ·  "+(trial.Passed?T("Missione completata","Mission completed"):T("Da riprovare","Try again")),22,65,card.Width-44,54,11,false,Theme.Muted);
   }
  }
 }
}
