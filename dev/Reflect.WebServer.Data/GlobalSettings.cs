using System.IO;
using System.Reflection;

namespace Reflect.WebServer.Data
{
    public static class GlobalSettings
    {
        public const string PIPE_NAME = "Reflect.WebServer.Pipe";

        public static int ServerPort { get; set; }

        public static string ServerRoot { get; set; }

        public static void Load()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Reflect.WebServer.Data.ini");

            var fileIni = new IniFile(path);

            var serverPort = fileIni.Read("Port", "Server", "8080");

            ServerPort = int.TryParse(serverPort, out var result) ? result : 8080;

            ServerRoot = fileIni.Read("Root", "Server");


            if (string.IsNullOrEmpty(ServerRoot))
                ServerRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content");
        }

        public static void Save()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Reflect.WebServer.Data.ini");

            var fileIni = new IniFile(path);
            fileIni.Write("Port", ServerPort.ToString(), "Server");
            fileIni.Write("Root", ServerRoot, "Server");
        }
    }
}