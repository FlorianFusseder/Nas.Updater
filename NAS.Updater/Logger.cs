using System;
using System.IO;

namespace NAS.Updater
{
    public static class Logger
    {
        private static readonly object LoggerLock = new object();
        private const string Log = @"log.txt";
        private const string FehlerLog = @"fehler.txt";
        private static readonly string Start = $@"___________New Session {DateTime.Now}_______________________________________________________________________________";

        public static void CheckLogFileExists()
        {
            if (!File.Exists(Log))
                File.Create(Log).Close();

            if (!File.Exists(FehlerLog))
                File.Create(FehlerLog).Close();


            using (var writer = new StreamWriter(Log, true))
            {
                writer.WriteLine(Start);
            }
        }

        public static void WriteLogLine(string text)
        {
            lock (LoggerLock)
                using (var writer = new StreamWriter(Log, true))
                {
                    writer.WriteLine(text);
                }
        }

        public static void WriteFailureLine(string text)
        {
            lock(LoggerLock)
                using (var writer = new StreamWriter(FehlerLog, true))
                {
                    writer.WriteLine($"Session: {DateTime.Now}");
                    writer.WriteLine(text);
                }
        }
    }
}
