using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;

namespace DatabaseBackupService
{
    public partial class DatabaseBackupService : ServiceBase
    {
        private string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private string BackupFolder = ConfigurationManager.AppSettings["BackupFolder"];
        private string LogFolder = ConfigurationManager.AppSettings["LogFolder"];
        private int BackupIntervalMinutes = 0;
        Timer timer;


        public DatabaseBackupService()
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(LogFolder))
            {
                LogFolder = @"F:\DatabaseBackups\Logs";
                Log("LogFolder is missing in App.config. Using default: " + LogFolder);
            }


            if (string.IsNullOrWhiteSpace(BackupFolder))
            {
                BackupFolder = @"F:\DatabaseBackups";
                Log("BackupFolder is missing in App.config. Using default: " + BackupFolder);
            }

            try
            {
                BackupIntervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["BackupIntervalMinutes"]);
            }
            catch
            {
                BackupIntervalMinutes = 60;
                Log("Backup Interval Minutes is missing in App.config. Using default: " + BackupIntervalMinutes);

            }

            Directory.CreateDirectory(LogFolder);
            Directory.CreateDirectory(BackupFolder);
        }

        public void StartInConsole()
        {
            OnStart(null);
            Console.WriteLine("Press Enter to stop the service...");
            Console.ReadLine();
            OnStop();
        }

        private void Log(string message)
        {
            string logFilePath = Path.Combine(LogFolder, "ServiceLog.txt");
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";

            File.AppendAllText(logFilePath, logMessage);

            if (Environment.UserInteractive)
            {
                Console.WriteLine(logMessage);
            }
        }

        protected override void OnStart(string[] args)
        {
              timer = new Timer(
             callback: BackUpDatabase,                  // Callback method
             state: null,                              // State object (not used here)
             dueTime: TimeSpan.Zero,                   // Start immediately
             period: TimeSpan.FromMinutes(BackupIntervalMinutes) // Interval
         );
         Log($"Backup schedule initiated: every {BackupIntervalMinutes} minute(s).");
       
        }

        protected override void OnStop()
        {
            timer?.Dispose();
            Log("Service Stopped.");
        }

        public void BackUpDatabase(object sender)
        {
            try
            {
                string backupFileName = Path.Combine(BackupFolder, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");

                // Perform database backup
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string backupQuery = $@"BACKUP DATABASE [{connection.Database}] TO DISK = '{backupFileName}' WITH INIT";
                    using (SqlCommand command = new SqlCommand(backupQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                Log($"Database backup successful: {backupFileName}");

            }
            catch (Exception ex)
            {
                Log($"Error during backup: {ex.Message}");
            }
        }

    }
}
