using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;

namespace NAS.Updater
{


    class FileAktualisierer
    {
        private readonly SpeicherPfadCombo _pfadKlasse;
        private readonly object _listenLock = new object();
        private List<Task> _tasks;


        public FileAktualisierer(SpeicherPfadCombo pfad)
        {
            if (pfad != null) _pfadKlasse = pfad;
        }

        internal void Aktualisiere()
        {
            var lOrdner = new List<string>();
            var lDateien = new List<string>();
            var nOrdner = new List<string>();
            var nDateien = new List<string>();

            _tasks = new List<Task>
            {
                Task.Run(action: () => FindeDateien(_pfadKlasse.Lokal, lOrdner, lDateien)),
                Task.Run(() => FindeDateien(_pfadKlasse.Nas, nOrdner, nDateien)),
            };

            Task.WaitAll(_tasks.ToArray());
            _tasks.Clear();

            Logger.WriteLogLine($"{"Lokale Ordner:",-20} {lOrdner.Count}");
            Logger.WriteLogLine($"{"Lokale Dateien:",-20} {lDateien.Count}");
            Logger.WriteLogLine($"{"Nas Ordner:",-20} {nOrdner.Count}");
            Logger.WriteLogLine($"{"Nas Dateien:",-20} {nDateien.Count}");



            _tasks.Add(
            Task.Run(() =>
                {
                    var neueOrdner = lOrdner.Where(m => !nOrdner.Exists(i => i.Replace(_pfadKlasse.Nas, string.Empty) == m.Replace(_pfadKlasse.Lokal, string.Empty))).ToList();
                    neueOrdner.Sort(HirachieAbsteigend);

                    foreach (var lokal in neueOrdner)
                    {
                        try
                        {
                            Directory.CreateDirectory(_pfadKlasse.Nas + lokal.Replace(_pfadKlasse.Lokal, string.Empty));
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


            _tasks.Add(
                Task.Run(() =>
                    {
                        var geloeschteDateien = nDateien.Where(nf => !lDateien.Exists(lf => lf.Replace(_pfadKlasse.Lokal, string.Empty) == nf.Replace(_pfadKlasse.Nas, string.Empty)));
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

            Task.WaitAll(_tasks.ToArray());
            _tasks.Clear();

            _tasks.Add(
                Task.Run(delegate
                {
                    var neueFiles = lDateien.Where(lf => !nDateien.Exists(nf => nf.Replace(_pfadKlasse.Nas, string.Empty) == lf.Replace(_pfadKlasse.Lokal, string.Empty)));
                    foreach (var nFile in neueFiles)
                    {
                        try
                        {
                            File.Copy(nFile, _pfadKlasse.Nas + nFile.Replace(_pfadKlasse.Lokal, string.Empty));
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

            _tasks.Add(
                Task.Run(() =>
                {
                    var geloeschteOrdner = nOrdner.Where(m => !lOrdner.Exists(i => m.Replace(_pfadKlasse.Nas, string.Empty) == i.Replace(_pfadKlasse.Lokal, string.Empty))).ToList();
                    geloeschteOrdner.Sort(HirachieAufsteigend);

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

            _tasks.Add(
                Task.Run(() =>
                {
                    var gleicheDateien = lDateien.Where(lf => nDateien.Exists(nf => nf.Replace(_pfadKlasse.Nas, string.Empty) == lf.Replace(_pfadKlasse.Lokal, string.Empty)));

                    foreach (var item in gleicheDateien)
                    {
                        var datei = item.Replace(_pfadKlasse.Lokal, string.Empty);
                        var lZeit = File.GetLastWriteTime(_pfadKlasse.Lokal + datei);
                        var nZeit = File.GetLastWriteTime(_pfadKlasse.Nas + datei);

                        if (lZeit > nZeit)
                        {
                            try
                            {
                                File.Copy(_pfadKlasse.Lokal + datei, _pfadKlasse.Nas + datei, true);
                                Logger.WriteLogLine($"Copy altered file from {_pfadKlasse.Lokal}{datei} ({lZeit}) to {_pfadKlasse.Nas}{datei} ({nZeit})");
                            }
                            catch (Exception)
                            {
                                Logger.WriteFailureLine($"could not Copy altered file from {_pfadKlasse.Lokal}{datei} to {_pfadKlasse.Nas}{datei}");
                                SystemSounds.Beep.Play();
                            }
                        }
                    }
                }
                ));

            Task.WaitAll(_tasks.ToArray());

        }

        private int HirachieAbsteigend(string x, string y)
        {
            var a = x.Count(m => m == '\\');
            var b = y.Count(m => m == '\\');

            if (a > b) return 1;
            if (a < b) return -1;
            return 0;
        }

        private int HirachieAufsteigend(string x, string y)
        {
            var a = x.Count(m => m == '\\');
            var b = y.Count(m => m == '\\');

            if (a > b) return -1;
            if (a < b) return 1;
            return 0;
        }

        private void FindeDateien(string pfad, List<string> ordner, List<string> dateien)
        {
            var dir = Directory.GetDirectories(pfad).ToList();
            var t = new List<Task>();

            if (dir.Any())
            {
                foreach (var item in dir)
                {
                    t.Add(Task.Run(() => FindeDateien(item, ordner, dateien)));
                }
            }

            lock (_listenLock)
            {
                ordner.AddRange(dir);
                dateien.AddRange(Directory.GetFiles(pfad));
            }

            Task.WaitAll(t.ToArray());
        }
    }
}
