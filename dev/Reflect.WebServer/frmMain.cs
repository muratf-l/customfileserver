using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using NamedPipeWrapper;
using Reflect.WebServer.Data;

namespace Reflect.WebServer
{
    public partial class FrmMain : Form
    {
        private readonly NamedPipeClient<ServerCommand> _client =
            new NamedPipeClient<ServerCommand>(GlobalSettings.PIPE_NAME);

        public FrmMain()
        {
            InitializeComponent();

            //var assembly = Assembly.GetExecutingAssembly();

            //var fileLogo = assembly.GetManifestResourceStream("Reflect.WebServer.logo.png");
            //pictureBox1.Image = Image.FromStream(fileLogo);

            //var fileIco = assembly.GetManifestResourceStream("Reflect.WebServer.globalconnection.png");
            //pictureBox2.Image = Image.FromStream(fileIco);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            GlobalSettings.Load();

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Text = string.Concat(Text, " ", version);

            SetLog($"{Text}");
            SetLog("Reflect Inc.");
            SetLog("https://reflecth.ca");

            PrintSettings();

            _client.Error += ClientOnError;
            _client.ServerMessage += OnServerMessage;
            _client.Disconnected += OnDisconnected;
            _client.AutoReconnect = true;
            _client.Start();
        }

        private void ClientOnError(Exception exception)
        {
            SetLog($"Server ClientOnError : {exception.Message}");
        }

        private void OnServerMessage(NamedPipeConnection<ServerCommand, ServerCommand> connection,
            ServerCommand message)
        {
            if (message.Action == ServerAction.Info)
            {
                SetLog($"server message {message.Data}");

                var infObjects = message.ToObjects<ServerInfo>();

                if (infObjects == null) return;

                void MethodInvokerDelegate()
                {
                    btnStartStop.Enabled = true;

                    if (infObjects.ServerIsOnline)
                        btnStartStop.Checked = true;
                    else
                        btnStartStop.Checked = false;
                }

                if (InvokeRequired)
                    Invoke((MethodInvoker) MethodInvokerDelegate);
                else
                    MethodInvokerDelegate();
            }

            if (message.Action == ServerAction.Log)
                SetLog($"server message {message.Data}");
        }

        private void OnDisconnected(NamedPipeConnection<ServerCommand, ServerCommand> connection)
        {
            SetLog("Disconnected from server");
        }

        private void PrintSettings()
        {
            SetLog("-----------------------------------------------");
            SetLog("Application Settings");
            SetLog($"Server Port : {GlobalSettings.ServerPort}");
            SetLog($"Server Root : {GlobalSettings.ServerRoot}");
            SetLog("-----------------------------------------------");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new FrmSettings())
            {
                var dr = form.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    GlobalSettings.Load();
                    PrintSettings();

                    _client.PushMessage(new ServerCommand
                        {Action = ServerAction.RebootServer});
                }
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Checked)
                _client.PushMessage(new ServerCommand
                    {Action = ServerAction.StartServer});
            else
                _client.PushMessage(new ServerCommand
                    {Action = ServerAction.StopServer});
        }

        #region "Logging"

        private void LogHandler(object sender, string msg)
        {
            SetLog(msg);
        }

        private void SetLogText(object sender, EventArgs e)
        {
            if (txtLogs.Text.Length > 5000)
                txtLogs.Text = "";

            txtLogs.Text = string.Concat(txtLogs.Text, "\n", DateTime.Now, "\t", sender.ToString());
            Debug.WriteLine(sender.ToString());
        }

        private void SetLog(string msg)
        {
            msg = string.Concat(msg, "\r\n");

            try
            {
                Invoke(new EventHandler(SetLogText), msg, null);
            }

            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region "test"

        private void btnServerTest_Click(object sender, EventArgs e)
        {
            //Invoke(new EventHandler(StartTest));
            StartTest();
        }

        private void StartTest()
        {
            var fileServer = new FileServer();
            fileServer.LogEventHandler += LogHandler;
            fileServer.Start();
        }

        #endregion
    }
}