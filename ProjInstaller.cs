using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace DatabaseBackupService
{
    [RunInstaller(true)]
    public partial class ProjInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjInstaller()
        {
            InitializeComponent();

            serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService
            };

            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "DatabaseBackupService",
                DisplayName = "Automated Database Backup Service",
                StartType = ServiceStartMode.Automatic,
                Description = "This service is responsible for backing up my database every x minutes.",
                ServicesDependedOn = new string[] { "RpcSs", "EventLog", "MSSQLSERVER" }
            };

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
