# 🗄️ Automated SQL Server Database Backup Service

**Automated Database Backup Service** is a reliable background Windows Service designed to automate full **Microsoft SQL Server** database backups at customizable time intervals[span_1](start_span)[span_1](end_span). Built using **C# (.NET Framework)** and **T-SQL**, the system operates silently via the Service Control Manager (SCM), manages service dependencies (like SQL Server startup), provides dual execution modes for seamless testing, and maintains detailed operational log files[span_2](start_span)[span_2](end_span).

---

## 🌟 Key Features

### ⏱️ Automated & Scheduled Backups
* **Dynamic Interval Execution:** Automatically triggers full SQL Server database backups based on configurable time intervals (in minutes)[span_3](start_span)[span_3](end_span).
* **Timestamped Output Files:** Generates dynamic backup files tagged with execution date and time (e.g., `Backup_20260814_143000.bak`)[span_4](start_span)[span_4](end_span).

### 🔄 Dual Execution Modes
* **Background Windows Service:** Runs silently in the background managed by Windows Service Control Manager (SCM) with `LocalService` identity[span_5](start_span)[span_5](end_span).
* **Interactive Console Mode:** Supports immediate execution via `Environment.UserInteractive` for rapid debugging, live monitoring, and testing[span_6](start_span)[span_6](end_span).

### 📝 Robust Logging & Fallbacks
* **Operational Auditing:** Writes step-by-step service logs (`ServiceLog.txt`) capturing start/stop events, backup statuses, and errors[span_7](start_span)[span_7](end_span).
* **Automatic Directory Provisioning:** Checks and automatically creates required backup and log directories on startup if they do not exist[span_8](start_span)[span_8](end_span).
* **Default Configuration Fallbacks:** Features fallback safety values for missing or incomplete configuration settings[span_9](start_span)[span_9](end_span).

### ⚙️ System Dependencies & Reliability
* **Service Dependency Mapping:** Explicitly configured to depend on core services (`MSSQLSERVER`, `RpcSs`, `EventLog`) to guarantee SQL Server is active before backups begin[span_10](start_span)[span_10](end_span).

---

## 🛠️ Tech Stack & Architecture

* **Language:** C#[span_11](start_span)[span_11](end_span)
* **Framework:** .NET Framework (`System.ServiceProcess`, `System.Configuration.Install`)[span_12](start_span)[span_12](end_span)
* **Database Engine:** Microsoft SQL Server (`System.Data.SqlClient`, T-SQL `BACKUP DATABASE`)[span_13](start_span)[span_13](end_span)
* **Core Mechanisms:** Multithreading Timer (`System.Threading.Timer`), `System.IO`[span_14](start_span)[span_14](end_span)

---

## ⚙️ Configuration (App.config)

The service settings are completely decoupled from code and can be modified via the `App.config` file without needing to rebuild binaries[span_15](start_span)[span_15](end_span):

<appSettings>
  <add key="ConnectionString" value="Server=YOUR_SERVER;Database=YOUR_DATABASE;Integrated Security=True;" />
  <add key="BackupFolder" value="F:\DatabaseBackups" />
  <add key="LogFolder" value="F:\DatabaseBackups\Logs" />
  <add key="BackupIntervalMinutes" value="60" />
</appSettings>

---

## 🚀 How to Run & Install

### Prerequisites
* **Visual Studio** (2019 / 2022 / 2026) with .NET Desktop Development workload.
* **Microsoft SQL Server** & **SQL Server Management Studio (SSMS)**.

---

### Option 1: Running in Debug / Console Mode
Simply build and run the executable (`DatabaseBackupService.exe`) directly[span_16](start_span)[span_16](end_span). The application automatically detects the interactive session and launches a console interface for real-time output and debugging[span_17](start_span)[span_17](end_span).

---

### Option 2: Installing as a Native Windows Service

1. **Open Developer Command Prompt for Visual Studio** as **Administrator**.
2. **Navigate to the output build directory** containing `DatabaseBackupService.exe`.
3. **Register the Service:**
   InstallUtil.exe DatabaseBackupService.exe
4. **Manage Service:**
   * Open `services.msc` on your machine.
   * Locate **Automated Database Backup Service**[span_18](start_span)[span_18](end_span).
   * Start or set startup type to **Automatic**[span_19](start_span)[span_19](end_span).

5. **Uninstall Service:**
   InstallUtil.exe /u DatabaseBackupService.exe

---

## 👨‍💻 Author

**Muhammad Sadaka**  
* GitHub: [@Muhammad-sadaka](https://github.com/Muhammad-sadaka)

