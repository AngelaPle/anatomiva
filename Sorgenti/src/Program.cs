using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
[assembly:AssemblyTitle("Anatomiva")]
[assembly:AssemblyDescription("Offline anatomy learning · Angela Plescia")]
[assembly:AssemblyCompany("Angela Plescia")]
[assembly:AssemblyProduct("Anatomiva")]
[assembly:AssemblyVersion("2.0.0.0")]
namespace Anatomiva {
 static class Program {
  [STAThread]static int Main(string[] args){
   Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
   bool ui=args.Length==2&&args[0]=="--ui-test";
   string dir=ui?Path.GetFullPath(args[1]):Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"AngelaPlescia","Anatomiva");
   bool created;using(var mutex=new Mutex(true,"Anatomiva-AngelaPlescia-"+Environment.UserName,out created)){
    if(!created&&!ui){MessageBox.Show("Anatomiva è già aperto.\nAnatomiva is already running.");return 0;}
    try{
     var catalog=Catalog.Embedded();string path=Path.Combine(dir,"progress.xml");Storage store;
     if(!ui){try{Storage.ImportLegacyIfNeeded(path,Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"AngelaPlescia","BodyExplorer","progress.xml"));}catch(Exception ex){if(MessageBox.Show("Non è stato possibile importare i progressi di Body Explorer. Il file precedente è intatto. Continuare con un nuovo profilo Anatomiva?\n\nCould not import Body Explorer progress. The previous file is untouched. Continue with a new Anatomiva profile?\n\n"+ex.Message,"Anatomiva",MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2)!=DialogResult.Yes)return 1;}}
     try{store=new Storage(path);}catch(Exception){
      bool valid=false;try{Storage.Read(path+".bak");valid=true;}catch{}
      if(valid&&MessageBox.Show("Il file dei progressi non è leggibile. Vuoi ripristinare l'ultima copia di sicurezza? Il file attuale verrà conservato.\n\nProgress cannot be read. Restore the last backup? The current file will be preserved.","Anatomiva",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes){
       File.Copy(path,path+".damaged-"+DateTime.UtcNow.ToString("yyyyMMddHHmmss"));File.Copy(path+".bak",path,true);store=new Storage(path);
      }else{MessageBox.Show("Impossibile leggere i progressi. Nessun dato è stato cancellato.\nCannot read progress. No data was deleted.\n\n"+path,"Anatomiva");return 1;}
     }
     using(var form=new MainForm(catalog,store)){
      if(ui){form.Show();Application.DoEvents();form.RunUiChecks(dir);form.Dispose();return 0;}Application.Run(form);
     }return 0;
    }catch(Exception ex){try{Directory.CreateDirectory(dir);File.AppendAllText(Path.Combine(dir,"error.log"),DateTime.UtcNow.ToString("o")+" "+ex+Environment.NewLine);}catch{}
     if(ui){Console.Error.WriteLine(ex);return 1;}
     MessageBox.Show("Anatomiva non ha potuto avviarsi.\nAnatomiva could not start.\n\n"+ex.Message,"Anatomiva",MessageBoxButtons.OK,MessageBoxIcon.Error);return 1;
    }
   }
  }
 }
}
