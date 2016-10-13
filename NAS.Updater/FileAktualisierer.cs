using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAS.Updater
{


    class FileAktualisierer
    {
        private SpeicherPfadCombo pfadKlasse;
        private object ListenLock = new object();
        private List<Task> tasks;


        public FileAktualisierer(SpeicherPfadCombo pfad)
        {
            this.pfadKlasse = pfad;
        }

        internal void Aktualisiere()
        {
            var lOrdner = new List<string>();
            var lDateien = new List<string>();
            var nOrdner = new List<string>();
            var nDateien = new List<string>();

            var t = DateTime.Now;

            tasks = new List<Task>
            {
                Task.Run(() => FindeDateien(pfadKlasse.Lokal, lOrdner, lDateien)),
                Task.Run(() => FindeDateien(pfadKlasse.Nas, nOrdner, nDateien)),
            };

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            Logger.WriteLogLine($"{"Lokale Ordner:",-20} {lOrdner.Count}");
            Logger.WriteLogLine($"{"Lokale Dateien:",-20} {lDateien.Count}");
            Logger.WriteLogLine($"{"Nas Ordner:",-20} {nOrdner.Count}");
            Logger.WriteLogLine($"{"Nas Dateien:",-20} {nDateien.Count}");



            tasks.Add(
            Task.Run(delegate
                {
                    var neueOrdner = lOrdner.Where(m => !nOrdner.Exists(i => i.Replace(pfadKlasse.Nas, string.Empty) == m.Replace(pfadKlasse.Lokal, string.Empty))).ToList();
                    neueOrdner.Sort(hirachieAbsteigend);

                    foreach (var lokal in neueOrdner)
                    {
                        try
                        {
                            Directory.CreateDirectory(pfadKlasse.Nas + lokal.Replace(pfadKlasse.Lokal, string.Empty));
                            Logger.WriteLogLine($"{"Create folder in NAS:",-20} {lokal}");
                        }
                        catch (Exception)
                        {
                            Logger.WriteFailureLine($"{"Could not create folder in NAS:",-20} {lokal}");
                            SystemSounds.Beep.Play();
                        }
                    }
                }
                ));


            tasks.Add(
                Task.Run(delegate
                {
                    var geloeschteDateien = nDateien.Where(nf => !lDateien.Exists(lf => lf.Replace(pfadKlasse.Lokal, string.Empty) == nf.Replace(pfadKlasse.Nas, string.Empty)));
                    foreach (var gFile in geloeschteDateien)
                    {
                        try
                        {
                            File.Delete(gFile);
                            Logger.WriteLogLine($"{"Deleted File in NAS:",-20} {gFile}");
                        }
                        catch (Exception)
                        {
                            Logger.WriteFailureLine($"{"Could not delete file in Nas:",-20} {gFile}");
                            SystemSounds.Beep.Play();
                        }
                    }
                }
            ));

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            tasks.Add(
                Task.Run(delegate
                {
                    var neueFiles = lDateien.Where(lf => !nDateien.Exists(nf => nf.Replace(pfadKlasse.Nas, string.Empty) == lf.Replace(pfadKlasse.Lokal, string.Empty)));
                    foreach (var nFile in neueFiles)
                    {
                        try
                        {
                            File.Copy(nFile, pfadKlasse.Nas + nFile.Replace(pfadKlasse.Lokal, string.Empty));
                            Logger.WriteLogLine($"{"Copied new file in NAS:",-20} {nFile}");
                        }
                        catch (Exception)
                        {
                            Logger.WriteFailureLine($"{"Could not copy new file in NAS:",-20} {nFile}");
                            SystemSounds.Beep.Play();
                        }
                    }
                }
            ));

            tasks.Add(
                Task.Run(delegate
                {
                    var geloeschteOrdner = nOrdner.Where(m => !lOrdner.Exists(i => m.Replace(pfadKlasse.Nas, string.Empty) == i.Replace(pfadKlasse.Lokal, string.Empty))).ToList();
                    geloeschteOrdner.Sort(hirachieAufsteigend);

                    foreach (var nas in geloeschteOrdner)
                    {
                        try
                        {
                            Directory.Delete(nas, true);
                            Logger.WriteLogLine($"{"Deleted Folder in NAS:",-20} {nas}");
                        }
                        catch (Exception)
                        {
                            Logger.WriteFailureLine($"{"Could not Delete Folder in NAS",-20} {nas}");
                            SystemSounds.Beep.Play();
                        }
                    }
                }
            ));

            tasks.Add(
                Task.Run(delegate
                {
                    var t1 = DateTime.Now;
                    var nestedTasks = new List<Task>();
                    var gleicheDateien = lDateien.Where(lf => nDateien.Exists(nf => nf.Replace(pfadKlasse.Nas, string.Empty) == lf.Replace(pfadKlasse.Lokal, string.Empty)));
                    string datei = string.Empty;
                    DateTime lZeit;
                    DateTime nZeit;

                    foreach (var item in gleicheDateien)
                    {
                        datei = item.Replace(pfadKlasse.Lokal, string.Empty);
                        lZeit = File.GetLastWriteTime(pfadKlasse.Lokal + datei);
                        nZeit = File.GetLastWriteTime(pfadKlasse.Nas + datei);

                        if (lZeit > nZeit)
                        {
                            try
                            {
                                File.Copy(pfadKlasse.Lokal + datei, pfadKlasse.Nas + datei, true);
                                Logger.WriteLogLine($"Copy altered file from {pfadKlasse.Lokal}{datei} ({lZeit}) to {pfadKlasse.Nas}{datei} ({nZeit})");
                            }
                            catch (Exception)
                            {
                                Logger.WriteFailureLine($"could not Copy altered file from {pfadKlasse.Lokal}{datei} to {pfadKlasse.Nas}{datei}");
                                SystemSounds.Beep.Play();
                            }
                        }
                    }
                }
                ));

            Task.WaitAll(tasks.ToArray());

        }

        private int hirachieAbsteigend(string x, string y)
        {
            int a = x.Count(m => m == '\\');
            int b = y.Count(m => m == '\\');

            if (a > b) return 1;
            else if (a < b) return -1;
            else return 0;
        }

        private int hirachieAufsteigend(string x, string y)
        {
            int a = x.Count(m => m == '\\');
            int b = y.Count(m => m == '\\');

            if (a > b) return -1;
            else if (a < b) return 1;
            else return 0;
        }

        private void FindeDateien(string pfad, List<string> Ordner, List<string> Dateien)
        {
            var dir = Directory.GetDirectories(pfad).ToList();
            var t = new List<Task>();

            if (dir.Count() > 0)
            {
                foreach (var item in dir)
                {
                    t.Add(Task.Run(() => FindeDateien(item, Ordner, Dateien)));
                }
            }

            lock (ListenLock)
            {
                Ordner.AddRange(dir);
                Dateien.AddRange(Directory.GetFiles(pfad));
            }

            Task.WaitAll(t.ToArray());
        }
    }
}
