using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Anatomiva;
class CoreTests {
 static int count=0;static void Check(bool value,string name){if(!value)throw new Exception(name);count++;Console.WriteLine("PASS "+name);}
 static QuizSession Finish(Catalog c,int correct){var q=new QuizSession(c.Categories[5],1,"it",c.Version);for(int i=0;i<5;i++){
  var opt=q.Questions[q.Index].Options.First(o=>o.Correct==(i<correct));Check(q.Submit(opt.Id),"accept answer "+i);Check(!q.Submit(opt.Id),"ignore double answer "+i);Check(q.Continue(),"continue "+i);
 }return q;}
 static void LearningChecks(Catalog c,string dir){
  Check(Math.Abs(Mechanics.ExternalTorque(1,.30)-2.943)<1e-10,"torque 1kg 30cm = 2.943Nm");
  Check(Math.Abs(Mechanics.ExternalTorque(1,.15)*2-Mechanics.ExternalTorque(1,.30))<1e-10,"halving distance halves torque");
  Check(Math.Abs(Mechanics.ExternalTorque(2,.30)-2*Mechanics.ExternalTorque(1,.30))<1e-10,"doubling mass doubles torque");
  Check(Mechanics.ExternalTorque(1,0)==0,"zero lever arm gives zero torque");bool rejected=false;try{Mechanics.ExternalTorque(Double.NaN,.2);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"reject nonfinite mechanics input");
  var straight=Mechanics.HingeEndpoint(100,0);var bent=Mechanics.HingeEndpoint(100,90);Check(Math.Abs(straight[0])<1e-8&&straight[1]==100,"hinge reference geometry");Check(Math.Abs(bent[0]-100)<1e-8&&Math.Abs(bent[1])<1e-8,"hinge right-angle geometry");
  var foot=Mechanics.FootEndpoint(100,-15);Check(foot[1]<0,"negative ankle angle raises toes");
  foreach(var module in LabModule.All()){
   var state=new LabState(module.Id);state.Choice=module.CorrectChoice;var bad=state.Evaluate();Check(!bad.Passed,"initial model does not satisfy "+module.Id);
   state.Angle=module.TargetAngle;if(module.IsLever)state.DistanceCm=10;var pass=state.Evaluate();Check(pass.Passed,"mission success "+module.Id);Check(pass.Id==state.Evaluate().Id,"repeated evaluation idempotent "+module.Id);
   state.Choice=(module.CorrectChoice+1)%4;Check(!state.Evaluate().Passed,"concept error blocks success "+module.Id);
   if(!module.IsLever){state.Choice=module.CorrectChoice;state.Angle=module.TargetAngle+module.Tolerance;Check(state.Evaluate().Passed,"inclusive angle tolerance "+module.Id);state.Angle++;Check(!state.Evaluate().Passed,"outside angle tolerance "+module.Id);}
  }
  var lever=new LabState("lever"){Choice=0,DistanceCm=10,MassTenths=20};Check(!lever.Evaluate().Passed,"changing mass cannot bypass lever mission");
  var s=new Storage(Path.Combine(dir,"learning","progress.xml"));var lab=new LabState("ankle"){Angle=-15,Choice=3};s.AddLab(lab.Evaluate());s.AddLab(lab.Evaluate());Check(s.Data.Labs.Count==1,"lab saves once");Check(new Storage(s.PathName).Data.Labs.Single().Passed,"lab reopens with correct result");
  var quiz=Finish(c,3);s.Add(quiz.Result);var pending=ReviewPlanner.Pending(c,s.Data.Attempts);Check(pending.Count==2,"two missed questions enter review");
  var review=new QuizSession(pending,"en",c.Version);while(!review.Complete){review.Submit(review.Questions[review.Index].Options.First(o=>o.Correct).Id);review.Continue();}Check(review.Result.Total==2&&review.Result.Score==2&&review.Result.Kind=="review","short targeted review has its own denominator");s.Add(review.Result);Check(ReviewPlanner.Pending(c,s.Data.Attempts).Count==0,"correct review clears pending errors");
  var original=s.Data.Attempts.First(a=>a.Kind=="quiz");Check(original.Score==3,"review does not change historical quiz score");var reopened=new Storage(s.PathName);Check(reopened.Data.Attempts.Last().Kind=="review"&&reopened.Data.Attempts.Last().Total==2,"review persists with variable length");
  var missAgain=new QuizSession(new[]{pending[0]},"it",c.Version);missAgain.Submit(pending[0].Options.First(o=>!o.Correct).Id);missAgain.Continue();s.Add(missAgain.Result);Check(ReviewPlanner.Pending(c,s.Data.Attempts).Count==1,"later mistake re-enters review");
  var many=new QuizSession(c.Categories[0].Questions,"it",c.Version);Check(many.Questions.Count==5,"review sessions capped at five");
  string[] rows=ProgressExport.Lines(s.Data);Check(rows.Length==s.Data.Attempts.Count+s.Data.Labs.Count+1&&rows.All(x=>x.Split(',').Length==13),"CSV includes quizzes reviews and labs with consistent columns");
  var legacy=Path.Combine(dir,"legacy.xml");var legacyXml=XDocument.Load(s.PathName);legacyXml.Root.Name="bodyExplorer";legacyXml.Root.SetAttributeValue("schema",1);legacyXml.Root.Elements("lab").Remove();legacyXml.Root.Elements("attempt").Where(a=>(string)a.Attribute("kind")=="review").Remove();foreach(var a in legacyXml.Root.Elements("attempt"))a.Attribute("kind").Remove();legacyXml.Save(legacy);var before=File.ReadAllText(legacy);var migrated=Path.Combine(dir,"new-profile","progress.xml");Check(Storage.ImportLegacyIfNeeded(migrated,legacy),"legacy progress import runs on first launch");Check(File.ReadAllText(legacy)==before,"legacy file is preserved byte-for-byte");Check(new Storage(migrated).Data.Attempts.Single().Score==3,"legacy score preserved");Check((int)XDocument.Load(migrated).Root.Attribute("schema")==2,"migration writes schema 2");Check(!Storage.ImportLegacyIfNeeded(migrated,legacy),"existing new profile is never overwritten");
  s.Reset();Check(new Storage(s.PathName).Data.Labs.Count==0&&s.Data.Attempts.Count==0,"reset clears labs and reviews together");
  var blockedPath=Path.Combine(dir,"blocked-lab");File.WriteAllText(blockedPath,"file");var blocked=new Storage(Path.Combine(blockedPath,"progress.xml"));rejected=false;try{blocked.AddLab(lab.Evaluate());}catch{rejected=true;}Check(rejected&&blocked.Data.Labs.Count==0,"lab write failure rolls back");
  var newer=XDocument.Load(migrated);newer.Root.SetAttributeValue("schema",99);newer.Save(migrated);rejected=false;try{new Storage(migrated);}catch(InvalidDataException){rejected=true;}Check(rejected,"unknown schema rejected without overwriting");
 }
 static int Main(){string dir=Path.Combine(Path.GetTempPath(),"BodyExplorer-tests-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(dir);try{
  var c=Catalog.Embedded();Check(c.Categories.Count==12,"12 categories");Check(c.Categories.Sum(x=>x.Questions.Count)==120,"120 bilingual questions");Check(c.Categories.Single(x=>x.Id=="arm").Questions.All(q=>!q.Prompt.En.Contains("ear located")),"arm content corrected");
  var xml=XDocument.Load("content/catalog.xml");xml.Root.Elements("category").First().Elements("question").First().Elements("option").First().SetAttributeValue("correct",false);bool invalid=false;try{Catalog.Load(xml);}catch(InvalidDataException){invalid=true;}Check(invalid,"reject invalid correct answer count");
  var full=Finish(c,5);Check(full.Result.Score==5,"perfect score");Check(!full.Submit("0"),"ignore answer after completion");Check(!full.Continue(),"ignore repeated continue");Check(full.Result.Seconds>=0,"monotonic duration");
  Check(Finish(c,0).Result.Score==0,"zero score");Check(Finish(c,3).Result.Score==3,"mixed score");
  var unfinished=new QuizSession(c.Categories[0],2,"en",c.Version);Check(unfinished.Result==null,"abandon has no result");var order=unfinished.DisplayOptions(unfinished.Questions[0]).Select(o=>o.Id).ToArray();Check(order.Distinct().Count()==4,"shuffle keeps option IDs");Check(order.SequenceEqual(unfinished.DisplayOptions(unfinished.Questions[0]).Select(o=>o.Id)),"option order stable while rendering");Check(!unfinished.Submit("invalid"),"reject unknown option");
  string path=Path.Combine(dir,"folder with spaces","progress.xml");var s=new Storage(path);s.Add(full.Result);s.Add(full.Result);Check(s.Data.Attempts.Count==1,"idempotent save");s.Settings("en",true);var re=new Storage(path);Check(re.Data.Language=="en"&&re.Data.LargeText,"settings persist");Check(re.Data.Attempts[0].Score==5&&re.Data.Attempts[0].Answers[0].Prompt.En!=null,"answer snapshots persist");Check(File.Exists(path+".bak"),"atomic save creates backup");
  re.Reset();Check(new Storage(path).Data.Attempts.Count==0,"reset persists");Check(new Storage(path).Data.Language=="en","reset keeps language");
  File.WriteAllText(path,"broken");bool corrupt=false;try{new Storage(path);}catch{corrupt=true;}Check(corrupt&&File.ReadAllText(path)=="broken","corrupt data preserved");
  string block=Path.Combine(dir,"block");File.WriteAllText(block,"file");var blocked=new Storage(Path.Combine(block,"progress.xml"));bool failed=false;try{blocked.Add(full.Result);}catch{failed=true;}Check(failed&&blocked.Data.Attempts.Count==0,"failed write rolls back attempt");
  failed=false;try{blocked.Settings("en",true);}catch{failed=true;}Check(failed&&blocked.Data.Language=="it"&&!blocked.Data.LargeText,"failed settings roll back");
  LearningChecks(c,dir);Console.WriteLine("ALL "+count+" CHECKS PASSED");return 0;
 }catch(Exception ex){Console.Error.WriteLine(ex);return 1;}finally{Directory.Delete(dir,true);}}
}
