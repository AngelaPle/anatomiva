using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace Anatomiva {
 public partial class MainForm {
  void InspectText(Control control,string screen,System.Collections.Generic.List<string> warnings){
   var label=control as Label;if(label!=null&&label.Visible&&label.Width>0&&!String.IsNullOrWhiteSpace(label.Text)){
    var required=TextRenderer.MeasureText(label.Text,label.Font,new Size(Math.Max(1,label.Width-3),32767),TextFormatFlags.WordBreak|TextFormatFlags.NoPadding|TextFormatFlags.NoPrefix);
    if(required.Height>label.Height+3)warnings.Add(screen+": "+label.Text.Replace("\n"," / ").Substring(0,Math.Min(100,label.Text.Length))+" needs "+required.Height+"px, has "+label.Height+"px");
   }foreach(Control child in control.Controls)InspectText(child,screen,warnings);
  }
  void ClickTest(string name){var b=Controls.Find(name,true).OfType<Button>().Single();if(!b.Enabled)throw new Exception("Disabled test button: "+name);b.PerformClick();Application.DoEvents();}
  public void RunUiChecks(string output){
   var warnings=new System.Collections.Generic.List<string>();Directory.CreateDirectory(output);Action<string> shot=name=>{PerformLayout();Refresh();Application.DoEvents();using(var bmp=new Bitmap(ClientSize.Width,ClientSize.Height)){using(var g=Graphics.FromImage(bmp))g.CopyFromScreen(PointToScreen(Point.Empty),Point.Empty,ClientSize);bmp.Save(Path.Combine(output,name+".png"));}InspectText(this,name,warnings);};
   shot("home-it");ClickTest("start");shot("lab-elbow-it");var angle=Controls.Find("lab-angle",true).OfType<TrackBar>().Single();Controls.Find("lab-choice-1",true).OfType<RadioButton>().Single().Checked=true;ClickTest("lab-evaluate");if(storage.Data.Labs.Count!=1||storage.Data.Labs[0].Passed)throw new Exception("Incorrect angle was accepted");angle.Value=90;ClickTest("lab-evaluate");if(storage.Data.Labs.Count!=2||!storage.Data.Labs.Last().Passed)throw new Exception("Elbow mission failed");shot("mission-feedback-it");
   foreach(var module in LabModule.All().Where(m=>m.Id!="elbow")){
    ClickTest("lab-tab-"+module.Id);
    if(module.IsLever){Controls.Find("lab-distance",true).OfType<TrackBar>().Single().Value=10;shot("lab-lever-it");}
    else Controls.Find("lab-angle",true).OfType<TrackBar>().Single().Value=module.TargetAngle;
    Controls.Find("lab-choice-"+module.CorrectChoice,true).OfType<RadioButton>().Single().Checked=true;ClickTest("lab-evaluate");if(!storage.Data.Labs.Last().Passed)throw new Exception("Mission failed: "+module.Id);
   }
   ClickTest("nav-quiz");ClickTest("category-arm");ClickTest("level-1");shot("quiz-it");for(int i=0;i<5;i++){var id=session.Questions[session.Index].Options.First(o=>o.Correct==(i<3)).Id;ClickTest("option-"+id);ClickTest("continue");}
   if(!resultSaved||displayed.Score!=3)throw new Exception("Quiz save regression");shot("result-it");ClickTest("nav-review");if(ReviewPlanner.Pending(catalog,storage.Data.Attempts).Count!=2)throw new Exception("Pending review mismatch");shot("review-it");ClickTest("start-review");shot("review-question-it");while(session.Result==null){ClickTest("option-"+session.Questions[session.Index].Options.First(o=>o.Correct).Id);ClickTest("continue");}if(displayed.Total!=2||displayed.Score!=2||ReviewPlanner.Pending(catalog,storage.Data.Attempts).Count!=0)throw new Exception("Adaptive review regression");shot("review-result-it");
   ClickTest("nav-progress");shot("progress-it");
   ClickTest("language");Navigate("settings");Controls.Find("large-text",true).OfType<CheckBox>().Single().Checked=true;ClientSize=new Size(1024,768);Navigate("home");shot("home-en-large");Navigate("lab");ClickTest("lab-tab-ankle");Controls.Find("lab-angle",true).OfType<TrackBar>().Single().Value=-15;shot("lab-ankle-en-large");var animate=Controls.Find("lab-animate",true).OfType<Button>().Single();animate.PerformClick();var lab=Controls.Find("lab-view",true).OfType<LabView>().Single();if(!lab.Animating)throw new Exception("Animation did not start");animate.PerformClick();if(lab.Animating)throw new Exception("Animation did not pause");
   Navigate("help");shot("help-en-large");Navigate("settings");shot("settings-en-large");
   int before=storage.Data.Attempts.Count;foreach(var cat in catalog.Categories)for(int level=1;level<=2;level++){Navigate("quiz");ClickTest("category-"+cat.Id);ClickTest("level-"+level);for(int n=0;n<5;n++){ClickTest("option-"+session.Questions[session.Index].Options.First(o=>o.Correct==(n%2==0)).Id);ClickTest("continue");}if(displayed.Score!=3||!resultSaved)throw new Exception("Category regression "+cat.Id);}
   var reopened=new Storage(storage.PathName);if(reopened.Data.Attempts.Count!=before+24||reopened.Data.Labs.Count!=5)throw new Exception("Persistence regression");ProgressExport.Write(Path.Combine(output,"synthetic-export.csv"),reopened.Data);
   File.WriteAllLines(Path.Combine(output,"LAYOUT_CHECK.txt"),warnings.Count==0?new[]{"No label overflow detected by text measurement in sampled screens."}:warnings.ToArray());
   File.WriteAllText(Path.Combine(output,"UI_REPORT.txt"),"PASS: native control handlers (Button.PerformClick / TrackBar / RadioButton / CheckBox) exercised for all 4 lab missions plus numeric-error feedback, 24 category-level quizzes, 3/5 mixed score, adaptive 2-question review and pending-error removal, language change, large text at 1024x768, animation start/pause, persistence of quizzes/reviews/labs and CSV export. Screenshots captured from actual Mono/Xvfb execution on Linux. Native Windows installation, rendering, SmartScreen and high-DPI behavior are not tested.");
  }
 }
}
