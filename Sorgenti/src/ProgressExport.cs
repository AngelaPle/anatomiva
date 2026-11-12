using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
namespace Anatomiva {
 public static class ProgressExport {
  static string F(double value){return value.ToString("F2",CultureInfo.InvariantCulture);}
  public static string[] Lines(UserData data){var rows=new System.Collections.Generic.List<string>{"date_utc,kind,category_or_module,level,score,total,seconds,language,angle_degrees,mass_kg,distance_cm,torque_Nm,passed"};
   foreach(var a in data.Attempts)rows.Add(String.Join(",",new[]{a.CompletedUtc.ToString("o"),a.Kind,a.CategoryId,a.Level.ToString(),a.Score.ToString(),a.Total.ToString(),F(a.Seconds),a.Language,"","","","",""}));
   foreach(var l in data.Labs)rows.Add(String.Join(",",new[]{l.CompletedUtc.ToString("o"),"lab",l.ModuleId,"","","","","",l.Angle.ToString(),F(l.MassTenths/10.0),l.DistanceCm.ToString(),l.ModuleId=="lever"?F(l.Torque):"",l.Passed?"true":"false"}));return rows.ToArray();
  }
  public static void Write(string path,UserData data){File.WriteAllLines(path,Lines(data),new UTF8Encoding(true));}
 }
}
