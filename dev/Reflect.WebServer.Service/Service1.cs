using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using NamedPipeWrapper;
using Newtonsoft.Json;
using Reflect.WebServer.Data;
using Timer = System.Timers.Timer;

namespace Reflect.WebServer.Service
{
    public partial class Service1 : ServiceBase
    {
        private readonly FileServer _fileServer;
        private readonly NamedPipeServer<ServerCommand> _server;

        private readonly Timer _timer = new Timer();

        public Service1()
        {
            InitializeComponent();
            ServiceName = "Reflect.WebServer.Service";

            GlobalSettings.Load();

            var pSecure = new PipeSecurity();
            pSecure.SetAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, AccessControlType.Allow));

            _server = new NamedPipeServer<ServerCommand>(GlobalSettings.PIPE_NAME, pSecure);
            _server.Error += ServerOnError;
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.ClientMessage += OnClientMessage;

            _fileServer = new FileServer();
            _fileServer.LogEventHandler += FileServerLogHandler;
            _fileServer.StatusEventHandler += FileServerStatusHandler;
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += OnElapsedTime;
            _timer.Interval = 3000;
            //_timer.Enabled = true;
            //_timer.Start();

            _server.Start();
            _fileServer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _fileServer.Stop();
            _server.Stop();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            var msg = $"Service is recall at {DateTime.Now}";

            _server.PushMessage(new ServerCommand {Action = ServerAction.Info, Data = msg});
        }

        private void ServerOnError(Exception exception)
        {
            LogText.WriteToFile($"ServerOnError {exception.Message}");
        }

        private void OnClientConnected(NamedPipeConnection<ServerCommand, ServerCommand> connection)
        {
            PrintInfo();
        }

        private void PrintInfo()
        {
            var info = new ServerInfo
            {
                ServerIsOnline = _fileServer.IsRunning
            };

            _server.PushMessage(new ServerCommand
                {Action = ServerAction.Info, Data = JsonConvert.SerializeObject(info)});
        }

        private void OnClientDisconnected(NamedPipeConnection<ServerCommand, ServerCommand> connection)
        {
        }

        private void OnClientMessage(NamedPipeConnection<ServerCommand, ServerCommand> connection,
            ServerCommand command)
        {
            if (command.Action == ServerAction.RebootServer)
            {
                GlobalSettings.Load();
                _fileServer.Stop();
                Thread.Sleep(3000);
                _fileServer.Start();
            }

            if (command.Action == ServerAction.StopServer)
            {
                _fileServer.Stop();
                Thread.Sleep(3000);
            }

            if (command.Action == ServerAction.StartServer)
            {
                GlobalSettings.Load();
                Thread.Sleep(3000);
                _fileServer.Start();
                Thread.Sleep(3000);
            }
        }

        #region "Logging"

        private void FileServerLogHandler(object sender, string msg)
        {
            _server.PushMessage(new ServerCommand {Action = ServerAction.Log, Data = msg});
        }

        private void FileServerStatusHandler(object sender, string msg)
        {
            PrintInfo();
        }

        #endregion
    }
}