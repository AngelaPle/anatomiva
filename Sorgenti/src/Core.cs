using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Anatomiva {
 public class LocalText {
  public string It, En;
  public LocalText(string it,string en){It=it;En=en;}
  public string Get(string lang){return lang=="en"?En:It;}
  public static LocalText Read(XElement e){return new LocalText((string)e.Attribute("it"),(string)e.Attribute("en"));}
 }
 public class Choice {public string Id; public LocalText Text;public bool Correct;}
 public class Question {public string Id,Source; public int Level; public LocalText Prompt,Explanation; public List<Choice> Options;}
 public class Category {public string Id;public LocalText Name,Description;public List<Question> Questions;}
 public class Catalog {
  public List<Category> Categories;public string Version;
  public static Catalog Embedded(){using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("catalog.xml")){return Load(XDocument.Load(s));}}
  public static Catalog Load(XDocument doc){
   var c=new Catalog{Version=(string)doc.Root.Attribute("version"),Categories=new List<Category>()};
   foreach(var e in doc.Root.Elements("category")){
    var cat=new Category{Id=(string)e.Attribute("id"),Name=LocalText.Read(e),Description=new LocalText((string)e.Attribute("descriptionIt"),(string)e.Attribute("descriptionEn")),Questions=new List<Question>()};
    foreach(var q in e.Elements("question")) cat.Questions.Add(new Question{Id=(string)q.Attribute("id"),Source=(string)q.Attribute("source"),Level=(int)q.Attribute("level"),Prompt=LocalText.Read(q.Element("prompt")),Explanation=LocalText.Read(q.Element("explanation")),Options=q.Elements("option").Select(o=>new Choice{Id=(string)o.Attribute("id"),Correct=(bool)o.Attribute("correct"),Text=LocalText.Read(o)}).ToList()});
    c.Categories.Add(cat);
   }c.Validate();return c;
  }
  static bool TextOK(LocalText t){return t!=null&&!String.IsNullOrWhiteSpace(t.It)&&!String.IsNullOrWhiteSpace(t.En);}
  public void Validate(){
   if(Categories.Count!=12||Categories.Select(c=>c.Id).Distinct().Count()!=12)throw new InvalidDataException("Expected 12 unique categories.");
   var ids=new HashSet<string>();
   foreach(var c in Categories){
    if(!TextOK(c.Name)||!TextOK(c.Description)||c.Questions.Count!=10)throw new InvalidDataException("Incomplete category "+c.Id);
    foreach(int l in new[]{1,2})if(c.Questions.Count(q=>q.Level==l)!=5)throw new InvalidDataException("Incomplete level");
    foreach(var q in c.Questions){
     if(!ids.Add(q.Id)||!TextOK(q.Prompt)||!TextOK(q.Explanation)||q.Source!="anatomy"||q.Options.Count!=4||q.Options.Count(o=>o.Correct)!=1||q.Options.Any(o=>!TextOK(o.Text))||q.Options.Select(o=>o.Id).Distinct().Count()!=4)throw new InvalidDataException("Invalid question "+q.Id);
     foreach(string lang in new[]{"it","en"})if(q.Options.Select(o=>o.Text.Get(lang)).Distinct().Count()!=4)throw new InvalidDataException("Duplicate options "+q.Id);
    }
   }
  }
 }
 public class Answer {
  public string QuestionId;public LocalText Prompt,Chosen,Correct,Explanation; public bool IsCorrect;
 }
 public class Attempt {
  public string Id,CategoryId,Language,ContentVersion;public string Kind="quiz";public int Level;public DateTime CompletedUtc;public double Seconds;public List<Answer> Answers=new List<Answer>();
  public int Score{get{return Answers.Count(a=>a.IsCorrect);}}
  public int Total{get{return Answers.Count;}}
 }
 public class QuizSession {
  public Category Category;public string Kind="quiz";public int Level;public string Language,ContentVersion;public List<Question> Questions;public List<Answer> Answers=new List<Answer>();
  public int Index{get{return Answers.Count;}}public bool Feedback{get;private set;}public bool Complete{get{return Index==Questions.Count;}}
  Dictionary<string,List<Choice>> display=new Dictionary<string,List<Choice>>();
  public List<Choice> DisplayOptions(Question q){return display[q.Id];}
  Stopwatch clock=Stopwatch.StartNew();public Attempt Result{get;private set;}
  public QuizSession(Category cat,int level,string language,string version){Category=cat;Level=level;Language=language;ContentVersion=version;Questions=cat.Questions.Where(q=>q.Level==level).ToList();if(Questions.Count!=5)throw new ArgumentException("Missing level");PrepareOptions();}
  public QuizSession(IEnumerable<Question> questions,string language,string version){Kind="review";Category=ReviewPlanner.ReviewCategory();Level=0;Language=language;ContentVersion=version;Questions=questions.Take(5).ToList();if(Questions.Count<1||Questions.Select(q=>q.Id).Distinct().Count()!=Questions.Count)throw new ArgumentException("Empty or duplicate review");PrepareOptions();}
  void PrepareOptions(){var rng=new Random(Guid.NewGuid().GetHashCode());foreach(var q in Questions)display[q.Id]=q.Options.OrderBy(o=>rng.Next()).ToList();}
  public bool Submit(string optionId){
   if(Complete||Feedback)return false;var q=Questions[Index];var choice=q.Options.FirstOrDefault(o=>o.Id==optionId);if(choice==null)return false;
   Answers.Add(new Answer{QuestionId=q.Id,Prompt=q.Prompt,Chosen=choice.Text,Correct=q.Options.Single(o=>o.Correct).Text,Explanation=q.Explanation,IsCorrect=choice.Correct});Feedback=true;return true;
  }
  public bool Continue(){
   if(!Feedback)return false;Feedback=false;
   if(Complete&&Result==null){clock.Stop();Result=new Attempt{Id=Guid.NewGuid().ToString("N"),CategoryId=Category.Id,Kind=Kind,Level=Level,Language=Language,ContentVersion=ContentVersion,Seconds=clock.Elapsed.TotalSeconds,CompletedUtc=DateTime.UtcNow,Answers=new List<Answer>(Answers)};}return true;
  }
 }
 public class UserData {public string Language="it";public bool LargeText;public List<Attempt> Attempts=new List<Attempt>();public List<LabTrial> Labs=new List<LabTrial>();}
 public class Storage {
  public string PathName{get;private set;}public UserData Data{get;private set;}
  public Storage(string path){PathName=path;Data=File.Exists(path)?Read(path):new UserData();}
  static XmlReader Reader(string path){return XmlReader.Create(path,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=30000000});}
  public static UserData Read(string path){
   using(var r=Reader(path)){
    var root=XDocument.Load(r).Root;if(root==null||(root.Name!="bodyExplorer"&&root.Name!="anatomiva")||((int?)root.Attribute("schema")!=1&&(int?)root.Attribute("schema")!=2))throw new InvalidDataException("Unsupported progress format.");
    var d=new UserData{Language=(string)root.Attribute("language"),LargeText=(bool)root.Attribute("largeText")};if(d.Language!="it"&&d.Language!="en")throw new InvalidDataException("Invalid language");
    foreach(var a in root.Elements("attempt")){
     var t=new Attempt{Id=(string)a.Attribute("id"),CategoryId=(string)a.Attribute("category"),Level=(int)a.Attribute("level"),Language=(string)a.Attribute("language"),ContentVersion=(string)a.Attribute("contentVersion"),Kind=(string)a.Attribute("kind")??"quiz",CompletedUtc=DateTime.Parse((string)a.Attribute("completedUtc"),CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind),Seconds=(double)a.Attribute("seconds")};
     foreach(var e in a.Elements("answer"))t.Answers.Add(new Answer{QuestionId=(string)e.Attribute("question"),IsCorrect=(bool)e.Attribute("correct"),Prompt=LocalText.Read(e.Element("prompt")),Chosen=LocalText.Read(e.Element("chosen")),Correct=LocalText.Read(e.Element("solution")),Explanation=LocalText.Read(e.Element("explanation"))});
     if((t.Kind!="quiz"&&t.Kind!="review")||(t.Kind=="quiz"&&(t.Answers.Count!=5||t.Level<1||t.Level>2))||(t.Kind=="review"&&(t.Answers.Count<1||t.Answers.Count>5||t.Level!=0))||t.Seconds<0||Double.IsNaN(t.Seconds)||Double.IsInfinity(t.Seconds)||String.IsNullOrEmpty(t.Id))throw new InvalidDataException("Invalid attempt");d.Attempts.Add(t);
    }
    foreach(var e in root.Elements("lab")){if((string)e.Attribute("modelVersion")!="1")throw new InvalidDataException("Unsupported lab model version");var trial=new LabTrial{Id=(string)e.Attribute("id"),ModuleId=(string)e.Attribute("module"),Angle=(int)e.Attribute("angle"),DistanceCm=(int)e.Attribute("distanceCm"),MassTenths=(int)e.Attribute("massTenths"),Choice=(int)e.Attribute("choice"),CompletedUtc=DateTime.Parse((string)e.Attribute("completedUtc"),CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind)};trial.Validate();d.Labs.Add(trial);}
    if(d.Labs.Select(t=>t.Id).Distinct().Count()!=d.Labs.Count)throw new InvalidDataException("Duplicate lab attempt");
    if(d.Attempts.Select(t=>t.Id).Distinct().Count()!=d.Attempts.Count)throw new InvalidDataException("Duplicate attempt");return d;
   }
  }
  static XElement Text(string tag,LocalText t){return new XElement(tag,new XAttribute("it",t.It),new XAttribute("en",t.En));}
  public void Save(){
   var root=new XElement("anatomiva",new XAttribute("schema",2),new XAttribute("language",Data.Language),new XAttribute("largeText",Data.LargeText));
   foreach(var a in Data.Attempts)root.Add(new XElement("attempt",new XAttribute("id",a.Id),new XAttribute("kind",a.Kind),new XAttribute("category",a.CategoryId),new XAttribute("level",a.Level),new XAttribute("language",a.Language),new XAttribute("contentVersion",a.ContentVersion),new XAttribute("completedUtc",a.CompletedUtc.ToString("o",CultureInfo.InvariantCulture)),new XAttribute("seconds",a.Seconds),a.Answers.Select(e=>new XElement("answer",new XAttribute("question",e.QuestionId),new XAttribute("correct",e.IsCorrect),Text("prompt",e.Prompt),Text("chosen",e.Chosen),Text("solution",e.Correct),Text("explanation",e.Explanation)))));
   foreach(var l in Data.Labs)root.Add(new XElement("lab",new XAttribute("id",l.Id),new XAttribute("module",l.ModuleId),new XAttribute("angle",l.Angle),new XAttribute("distanceCm",l.DistanceCm),new XAttribute("massTenths",l.MassTenths),new XAttribute("choice",l.Choice),new XAttribute("completedUtc",l.CompletedUtc.ToString("o",CultureInfo.InvariantCulture)),new XAttribute("modelVersion","1")));
   Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PathName));string temp=PathName+".tmp-"+Guid.NewGuid().ToString("N");
   try{using(var fs=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None)){new XDocument(root).Save(fs);fs.Flush(true);}if(File.Exists(PathName))File.Replace(temp,PathName,PathName+".bak");else File.Move(temp,PathName);}finally{if(File.Exists(temp))File.Delete(temp);}
  }
  public void Add(Attempt attempt){if(Data.Attempts.Any(a=>a.Id==attempt.Id))return;Data.Attempts.Add(attempt);try{Save();}catch{Data.Attempts.Remove(attempt);throw;}}
  public void Settings(string lang,bool large){var old=Data.Language;var was=Data.LargeText;Data.Language=lang;Data.LargeText=large;try{Save();}catch{Data.Language=old;Data.LargeText=was;throw;}}
  public void AddLab(LabTrial trial){trial.Validate();if(Data.Labs.Any(t=>t.Id==trial.Id))return;Data.Labs.Add(trial);try{Save();}catch{Data.Labs.Remove(trial);throw;}}
  public static bool ImportLegacyIfNeeded(string target,string legacy){if(File.Exists(target)||!File.Exists(legacy))return false;var old=Read(legacy);var store=new Storage(target);store.Data=old;store.Save();return true;}
  public void Reset(){var old=Data.Attempts;var labs=Data.Labs;Data.Attempts=new List<Attempt>();Data.Labs=new List<LabTrial>();try{Save();}catch{Data.Attempts=old;Data.Labs=labs;throw;}}
 }
}
