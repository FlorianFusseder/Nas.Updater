using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAS.Updater
{
    static class Logger
    {
        static private object LoggerLock = new object();
        static private readonly string log = @"log.txt";
        static private readonly string fehlerLog = @"fehler.txt";
        static private readonly string start = $@"___________New Session {DateTime.Now}_______________________________________________________________________________";

        public static void CheckLogFileExists()
        {
            if (!File.Exists(log))
                File.Create(log).Close();

            if (!File.Exists(fehlerLog))
                File.Create(fehlerLog).Close();


            using (var writer = new StreamWriter(log, true))
            {
                writer.WriteLine(start);
            }
        }

        public static void WriteLogLine(string text)
        {
            lock (LoggerLock)
                using (var writer = new StreamWriter(log, true))
                {
                    writer.WriteLine(text);
                }
        }

        public static void WriteFailureLine(string text)
        {
            lock(LoggerLock)
                using (var writer = new StreamWriter(fehlerLog, true))
                {
                    writer.WriteLine($"Session: {DateTime.Now}");
                    writer.WriteLine(text);
                }
        }
    }
}
