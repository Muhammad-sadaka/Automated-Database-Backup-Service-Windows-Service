# 🗄️ Database Backup Service — C# Windows Service

![Language](https://img.shields.io/badge/language-C%23-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![Database](https://img.shields.io/badge/database-SQL%20Server-red)
![Type](https://img.shields.io/badge/type-Windows%20Service-green)

A Windows Service written in C# that **automatically backs up a SQL Server database at a fixed interval**. Each run produces a timestamped `.bak` file and writes the result to a log file.

It is a compact reference for:

- Building a Windows Service with `ServiceBase`
- Scheduling recurring work with `System.Threading.Timer`
- Running T-SQL `BACKUP DATABASE` from C# with `SqlCommand`
- Reading settings from `App.config`
- Installing a service with `ProjInstaller` and `installutil`
- Running the same code as a console app for debugging

---

## ✨ Features

- Automatic backups every **N minutes** (configurable)
- First backup runs immediately when the service starts
- Timestamped backup files: `Backup_yyyyMMdd_HHmmss.bak`
- Database name is taken from the connection string, so no hard-coded name
- Timestamped logging to `ServiceLog.txt`
- Configuration through `App.config`, with defaults when values are missing
- Creates the backup and log folders automatically
- Runs as a Windows Service (automatic start) or as a console app
- Errors are caught and logged, so one failed backup never stops the service

---

## 🧱 How It Works

```
Timer (every N minutes) ──► open SQL connection ──► BACKUP DATABASE ... TO DISK ──► .bak file
                                                                         │
                                                                         └──► ServiceLog.txt
```

1. `OnStart` creates a `Timer` that fires immediately and then every `BackupIntervalMinutes`.
2. `BackUpDatabase` builds a file name from the current date and time.
3. It connects to SQL Server and runs:
   ```sql
   BACKUP DATABASE [YourDb] TO DISK = 'path\Backup_20261120_031500.bak' WITH INIT
   ```
4. Success or failure is written to the log.
5. `OnStop` disposes the timer.

---

## 📁 Project Structure

```
DatabaseBackupService/
├── DatabaseBackupService.cs   # Service logic (timer, backup, log)
├── ProjInstaller.cs           # Service installer configuration
├── Program.cs                 # Entry point
├── App.config                 # Settings
└── README.md
```

---

## ⚙️ Configuration

Edit `App.config`:

```xml
<configuration>
  <appSettings>
    <add key="ConnectionString" value="Server=.;Database=MyDb;Integrated Security=True;" />
    <add key="BackupFolder" value="F:\DatabaseBackups" />
    <add key="LogFolder" value="F:\DatabaseBackups\Logs" />
    <add key="BackupIntervalMinutes" value="60" />
  </appSettings>
</configuration>
```

| Key                     | Description                              | Default                       |
|-------------------------|------------------------------------------|-------------------------------|
| `ConnectionString`      | SQL Server connection (includes the database name) | none (required)     |
| `BackupFolder`          | Where `.bak` files are saved             | `F:\DatabaseBackups`          |
| `LogFolder`             | Where `ServiceLog.txt` is written        | `F:\DatabaseBackups\Logs`     |
| `BackupIntervalMinutes` | Minutes between backups                  | `60`                          |

---

## 🚀 Getting Started

### Requirements
- Windows
- .NET Framework 4.x
- SQL Server (any edition that supports `BACKUP DATABASE`)
- Visual Studio 2019+ (to build)
- Administrator rights (to install the service)

### 1. Clone and build
```bash
git clone https://github.com/<your-username>/<your-repo>.git
```
Open the solution in Visual Studio and build in **Release** mode.

### 2. Test in console mode (recommended first)
Run the project from Visual Studio. In interactive mode the service runs in the console and prints log messages. Press **Enter** to stop it.

### 3. Install the service
Open **Developer Command Prompt for VS as Administrator**:

```bash
installutil DatabaseBackupService.exe
```

### 4. Start the service
```bash
net start DatabaseBackupService
```

### 5. Stop and uninstall
```bash
net stop DatabaseBackupService
installutil /u DatabaseBackupService.exe
```

### Service settings
| Setting      | Value                                   |
|--------------|-----------------------------------------|
| Service name | `DatabaseBackupService`                 |
| Display name | Automated Database Backup Service       |
| Start type   | Automatic                               |
| Account      | `LocalService`                          |
| Depends on   | `RpcSs`, `EventLog`, `MSSQLSERVER`      |

---

## 🔑 Important: Permissions

Two different accounts are involved, and both must have access:

1. **The service account** (`LocalService` by default) must be able to **write the log folder** and **connect to SQL Server**. With `Integrated Security=True`, `LocalService` usually has no SQL login, so either grant it one, run the service under another account, or use SQL authentication in the connection string.
2. **The SQL Server service account** is the one that actually **writes the `.bak` file**. The `BackupFolder` must be writable by that account, not only by the Windows service.

Most "backup failed" errors come from one of these two.

> **SQL Server instance name:** the installer depends on `MSSQLSERVER`, which is the **default instance** only. For a named instance, change it to `MSSQL$InstanceName` in `ProjInstaller.cs`.

---

## 🧪 Quick Test

1. Start the service (or run in console mode).
2. A `.bak` file appears in the backup folder immediately.
3. Check `ServiceLog.txt`:

```
[2026-11-20 03:15:00] Backup schedule initiated: every 60 minute(s).
[2026-11-20 03:15:02] Database backup successful: F:\DatabaseBackups\Backup_20261120_031500.bak
```

---

## ⚠️ Known Limitations

- No retention policy: old backups are never deleted, so the folder grows forever
- If a backup takes longer than the interval, two backups can overlap
- The backup path is inserted directly into the SQL text (safe for normal paths, but a path containing `'` will break the command)
- Full backups only (no differential or transaction log backups)
- No backup verification (`CHECKSUM`, `RESTORE VERIFYONLY`) or compression
- The connection string is stored in plain text in `App.config`
- `OnStop` does not wait for a running backup to finish
- Log writes are not synchronized
- A missing `BackupIntervalMinutes` key is read as `0`, which makes the timer fire only once (see the roadmap item below)

## 🗺️ Roadmap

- [ ] Validate the interval (use the default when the value is missing or ≤ 0)
- [ ] Automatic cleanup of backups older than N days
- [ ] Prevent overlapping backups
- [ ] Differential and log backups
- [ ] `WITH CHECKSUM, COMPRESSION` and restore verification
- [ ] Encrypt the connection string section
- [ ] Email or Event Log alerts on failure
- [ ] Thread-safe logging
- [ ] Unit tests

---

## 🤝 Contributing

1. Fork the repository
2. Create a branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push: `git push origin feature/your-feature`
5. Open a Pull Request

---

## 📄 License

Released under the [MIT License](LICENSE).

## 👤 Author

**Muhammad** — Junior Software Developer, Damascus, Syria

- GitHub: [@Muhammad-sadaka](https://github.com/Muhammad-sadaka)

If this project helped you learn, please give it a ⭐