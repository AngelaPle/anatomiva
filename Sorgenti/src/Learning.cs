using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
namespace Anatomiva {
 public static class ReviewPlanner {
  public static Category ReviewCategory(){return new Category{Id="review",Name=new LocalText("Ripasso mirato","Targeted review"),Description=new LocalText("Riparti dai concetti da consolidare.","Revisit concepts that need practice."),Questions=new List<Question>()};}
  public static List<Question> Pending(Catalog catalog,IEnumerable<Attempt> attempts){
   var latest=new Dictionary<string,Answer>();var times=new Dictionary<string,DateTime>();
   foreach(var a in attempts.OrderBy(x=>x.CompletedUtc))foreach(var answer in a.Answers){latest[answer.QuestionId]=answer;times[answer.QuestionId]=a.CompletedUtc;}
   return catalog.Categories.SelectMany(c=>c.Questions).Where(q=>latest.ContainsKey(q.Id)&&!latest[q.Id].IsCorrect).OrderBy(q=>times[q.Id]).ThenBy(q=>q.Id).ToList();
  }
 }
 public static class Mechanics {
  public const double Gravity=9.81;
  public static double ExternalTorque(double massKg,double distanceMeters){if(Double.IsNaN(massKg)||Double.IsInfinity(massKg)||Double.IsNaN(distanceMeters)||Double.IsInfinity(distanceMeters)||massKg<0||distanceMeters<0)throw new ArgumentOutOfRangeException();return massKg*Gravity*distanceMeters;}
  // Angle is flexion from a straight downward segment. No clinical ROM is inferred.
  public static double[] HingeEndpoint(double length,double flexionDegrees){var rad=flexionDegrees*Math.PI/180;return new[]{length*Math.Sin(rad),length*Math.Cos(rad)};}
  // Positive plantar flexion, negative dorsiflexion; neutral foot is horizontal.
  public static double[] FootEndpoint(double length,double angleDegrees){var rad=angleDegrees*Math.PI/180;return new[]{length*Math.Cos(rad),length*Math.Sin(rad)};}
 }
 public class LabModule {
  public string Id;public LocalText Name,Description,Goal,Question,Explanation;public LocalText[] Choices;public int MinAngle,MaxAngle,StartAngle,TargetAngle,Tolerance,CorrectChoice;
  public bool IsLever{get{return Id=="lever";}}
  public static List<LabModule> All(){return new List<LabModule>{
   new LabModule{Id="elbow",Name=new LocalText("Gomito","Elbow"),Description=new LocalText("Muovi l'avambraccio attorno al gomito. Il segmento chiaro è il riferimento esteso: 0° di flessione.","Move the forearm around the elbow. The pale segment is the straight reference: 0° flexion."),Goal=new LocalText("Missione: porta il gomito a 90° di flessione (±5°), poi scegli il nome del movimento rispetto al riferimento esteso.","Mission: set the elbow to 90° flexion (±5°), then name the movement from the straight reference."),Question=new LocalText("Come si chiama questo movimento?","What is this movement called?"),Choices=new[]{new LocalText("Estensione","Extension"),new LocalText("Flessione","Flexion"),new LocalText("Dorsiflessione","Dorsiflexion"),new LocalText("Abduzione","Abduction")},CorrectChoice=1,MinAngle=0,MaxAngle=140,StartAngle=15,TargetAngle=90,Tolerance=5,Explanation=new LocalText("La flessione piega il gomito e riduce l'angolo interno tra braccio e avambraccio. Il valore mostrato è la flessione dal riferimento esteso, non quell'angolo interno.","Flexion bends the elbow and decreases the internal angle between upper arm and forearm. The displayed value is flexion from the straight reference, not that internal angle.")},
   new LabModule{Id="knee",Name=new LocalText("Ginocchio","Knee"),Description=new LocalText("Osserva il movimento tra coscia e gamba in uno schema a cerniera. L'anca rimane ferma nel disegno.","Observe thigh and lower-leg movement in a hinge schematic. The hip stays fixed in the drawing."),Goal=new LocalText("Missione: imposta 60° di flessione del ginocchio (±5°). Poi identifica il gruppo muscolare che contribuisce a raddrizzarlo.","Mission: set knee flexion to 60° (±5°). Then identify the muscle group that helps straighten it."),Question=new LocalText("Quale gruppo muscolare contribuisce all'estensione del ginocchio?","Which muscle group contributes to knee extension?"),Choices=new[]{new LocalText("Deltoide","Deltoid"),new LocalText("Bicipite del braccio","Biceps brachii"),new LocalText("Quadricipite","Quadriceps"),new LocalText("Massetere","Masseter")},CorrectChoice=2,MinAngle=0,MaxAngle=120,StartAngle=10,TargetAngle=60,Tolerance=5,Explanation=new LocalText("Il quadricipite contribuisce a raddrizzare il ginocchio. Lo schema mostra solo flessione ed estensione, senza riprodurre tutta la meccanica reale dell'articolazione.","The quadriceps helps straighten the knee. The schematic shows only flexion and extension, without reproducing the full mechanics of the real joint.")},
   new LabModule{Id="ankle",Name=new LocalText("Caviglia","Ankle"),Description=new LocalText("Solleva o abbassa la punta del piede. Qui 0° indica il piede perpendicolare alla gamba; i valori negativi sollevano la punta.","Raise or lower the toes. Here 0° means the foot is perpendicular to the lower leg; negative values raise the toes."),Goal=new LocalText("Missione: porta la punta verso la gamba fino a −15° nel modello (±3°), poi identifica il movimento.","Mission: bring the toes toward the lower leg to −15° in the model (±3°), then identify the movement."),Question=new LocalText("Come si chiama il movimento che avvicina il dorso del piede alla gamba?","What is the movement that brings the top of the foot toward the lower leg called?"),Choices=new[]{new LocalText("Flessione plantare","Plantar flexion"),new LocalText("Abduzione della spalla","Shoulder abduction"),new LocalText("Flessione del gomito","Elbow flexion"),new LocalText("Dorsiflessione","Dorsiflexion")},CorrectChoice=3,MinAngle=-25,MaxAngle=45,StartAngle=10,TargetAngle=-15,Tolerance=3,Explanation=new LocalText("La dorsiflessione avvicina il dorso del piede alla gamba. Nella flessione plantare la punta si allontana. Il segno meno è una convenzione di questo disegno.","Dorsiflexion brings the top of the foot toward the lower leg. Plantar flexion points the toes away. The minus sign is a convention used in this drawing.")},
   new LabModule{Id="lever",Name=new LocalText("Leve del braccio","Arm levers"),Description=new LocalText("L'avambraccio resta orizzontale. Cambia massa e distanza del carico dal gomito e osserva il momento esterno.","The forearm stays horizontal. Change the load mass and its distance from the elbow and observe the external torque."),Goal=new LocalText("Missione: mantieni il carico a 1,0 kg e avvicinalo al gomito fino a ottenere un momento ≤1,20 N·m (distanza ≤12 cm). Poi rispondi.","Mission: keep the load at 1.0 kg and move it toward the elbow until torque is ≤1.20 N·m (distance ≤12 cm). Then answer."),Question=new LocalText("A parità di massa, quale cambiamento riduce il momento esterno?","For the same mass, which change reduces the external torque?"),Choices=new[]{new LocalText("Ridurre la distanza dal gomito","Reducing the distance from the elbow"),new LocalText("Cambiare il colore del carico","Changing the load color"),new LocalText("Aumentare la distanza dal gomito","Increasing the distance from the elbow"),new LocalText("Cambiare il nome del carico","Changing the load name")},CorrectChoice=0,MinAngle=90,MaxAngle=90,StartAngle=90,TargetAngle=90,Tolerance=0,Explanation=new LocalText("Con avambraccio orizzontale e gravità verticale, il momento esterno del solo carico è τ = m × g × d. A massa uguale, dimezzare la distanza dimezza il momento. Non è una misura della forza muscolare.","With a horizontal forearm and vertical gravity, external torque from the load alone is τ = m × g × d. For the same mass, halving distance halves torque. This is not a measurement of muscle force.")}
  };}
  public static LabModule Find(string id){var found=All().FirstOrDefault(m=>m.Id==id);if(found==null)throw new InvalidDataException("Unknown lab module");return found;}
 }
 public class LabState {
  public LabModule Module;public int Angle,DistanceCm=30,MassTenths=10,Choice=-1;LabTrial previous;
  public LabState(string module){Module=LabModule.Find(module);Angle=Module.StartAngle;}
  public LabTrial Evaluate(){
   if(Choice<0||Choice>3)throw new InvalidOperationException("Choose an answer first");
   if(previous!=null&&previous.Angle==Angle&&previous.DistanceCm==DistanceCm&&previous.MassTenths==MassTenths&&previous.Choice==Choice)return previous;
   var trial=new LabTrial{Id=Guid.NewGuid().ToString("N"),ModuleId=Module.Id,Angle=Angle,DistanceCm=DistanceCm,MassTenths=MassTenths,Choice=Choice,CompletedUtc=DateTime.UtcNow};trial.Validate();previous=trial;return trial;
  }
 }
 public class LabTrial {
  public string Id,ModuleId;public int Angle,DistanceCm,MassTenths,Choice;public DateTime CompletedUtc;
  public bool NumericGoal{get{var m=LabModule.Find(ModuleId);return m.IsLever?MassTenths==10&&DistanceCm<=12&&Torque<=1.20:Math.Abs(Angle-m.TargetAngle)<=m.Tolerance;}}
  public bool ConceptGoal{get{return Choice==LabModule.Find(ModuleId).CorrectChoice;}}
  public bool Passed{get{return NumericGoal&&ConceptGoal;}}
  public double Torque{get{return Mechanics.ExternalTorque(MassTenths/10.0,DistanceCm/100.0);}}
  public void Validate(){var m=LabModule.Find(ModuleId);if(String.IsNullOrEmpty(Id)||Angle<m.MinAngle||Angle>m.MaxAngle||DistanceCm<5||DistanceCm>35||MassTenths<5||MassTenths>30||Choice<0||Choice>3)throw new InvalidDataException("Invalid laboratory attempt");}
 }
}
