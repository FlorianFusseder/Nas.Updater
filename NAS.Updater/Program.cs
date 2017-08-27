using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistenz;
using System.IO;
using System;
using System.Windows.Forms;

namespace NAS.Updater
{

    class Program
    {
        private static string NasPfad = @"\\BANANA-NAS\BackupRechner";
        [STAThread]
        static void Main(string[] args)
        {
            var tr = new Tray();

            var serializer = new Serializer();
            var list = serializer.Load<List<SpeicherPfadCombo>>("Pfade.txt") ?? new List<SpeicherPfadCombo>();
            list = list.Distinct().ToList();


            if (args.Any())
            {

                if (args.ElementAt(0) == "man" || args.ElementAt(0) == "-?" || args.ElementAt(0) == "-help")
                {
                    MessageBox.Show($"Zum Pfad hinzufuegen -a \"Lokaler Ordnerpfad\" nutzen");
                }
                else
                {
                    args[0] = string.Empty;
                    var nstr = args.Aggregate((m, n) => m + " " + n);

                    var combo = new SpeicherPfadCombo
                    {
                        Lokal = nstr,
                        Nas = NasPfad + "\\" + Path.GetFileName(nstr),
                    };

                    if (Directory.Exists(nstr))
                    {

                        if (combo.Check())
                            list.Add(combo);


                        serializer.Save("Pfade.txt", list);
                        MessageBox.Show("Erfolgreich hinzugefügt", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                        MessageBox.Show("Ordner existiert nicht");
                }
            }
            else
            {
                

                try
                {
                    Logger.CheckLogFileExists();

                    var commandList = new List<FileAktualisiererCommand>();
                    var taskList = new List<Task>();

                    list.Where(m => m.Check()).ToList().ForEach(m => commandList.Add(new FileAktualisiererCommand(new FileAktualisierer(m))));
                    commandList.ForEach(m => taskList.Add(Task.Factory.StartNew(m.Execute)));

                    Task.WaitAll(taskList.ToArray());
                }
                catch (Exception e)
                {

                    throw new Exception($"Fehler: {e.Message}");
                }
                Logger.WriteLogLine($"Fertig {DateTime.Now}");
            }
            tr.Close();
        }
    }
}
