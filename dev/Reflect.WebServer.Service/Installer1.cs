using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Reflect.WebServer.Service
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        private readonly ServiceInstaller _serviceInstaller1 = new ServiceInstaller();

        private readonly ServiceProcessInstaller _serviceProcessInstaller1 = new ServiceProcessInstaller();

        public Installer1()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            _serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;

            _serviceProcessInstaller1.Password = null;
            _serviceProcessInstaller1.Username = null;
            _serviceInstaller1.ServiceName = "Reflect.WebServer.Service.Monitor";
            _serviceInstaller1.Description = "Reflect.WebServer.Service WatchDog";
            _serviceInstaller1.DisplayName = "Reflect.WebServer.Service.Monitor";
            _serviceInstaller1.StartType = ServiceStartMode.Automatic;

            Installers.AddRange(new Installer[]
            {
                _serviceProcessInstaller1,
                _serviceInstaller1
            });
        }
    }
}