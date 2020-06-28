using System;
using System.IO;
using System.Reflection;

namespace Reflect.WebServer.Service
{
    internal class LogText
    {
        public static void WriteToFile(string message)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var filepath = Path.Combine(path, DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt");

            if (!File.Exists(filepath))
                using (var sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message);
                }
            else
                using (var sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                }
        }
    }
}