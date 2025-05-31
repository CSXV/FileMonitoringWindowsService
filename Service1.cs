using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace MyFirstService {
  public partial class Service1 : ServiceBase {
    private FileSystemWatcher fileWatcher;

    private string SourceFolder;
    private string DestinationFolder;
    private string LogFolder;
    private string logFilePath;

    public Service1() {
      InitializeComponent();

      fileWatcher = new FileSystemWatcher();

      SourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
      DestinationFolder = ConfigurationManager.AppSettings["DesintationFolder"];
      LogFolder = ConfigurationManager.AppSettings["LogFolder"];

      if(string.IsNullOrWhiteSpace(SourceFolder)) {
        SourceFolder = @"C:\FileMonitoring\Source";
        throw new ConfigurationErrorsException($"Source Folder is not specified in the configuration file, using default in {SourceFolder}");
      }
      if(string.IsNullOrWhiteSpace(DestinationFolder)) {
        DestinationFolder = @"C:\FileMonitoring\Destination";
        throw new ConfigurationErrorsException($"Destination folder is not specified in the configuration file, using defalut in {DestinationFolder}");
      }
      if(string.IsNullOrWhiteSpace(LogFolder)) {
        LogFolder = @"C:\FileMonitoring\Logs";
        throw new ConfigurationErrorsException($"Log folder is not specified in the configuration file, using defalut in {LogFolder}");
      }

      if(!Directory.Exists(SourceFolder)) {
        Directory.CreateDirectory(SourceFolder);
      }
      if(!Directory.Exists(LogFolder)) {
        Directory.CreateDirectory(LogFolder);
      }
      if(!Directory.Exists(DestinationFolder)) {
        Directory.CreateDirectory(DestinationFolder);
      }

      logFilePath = Path.Combine(LogFolder, "Logs.txt");
    }

    private void LogServiceEvent(string message) {
      string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";

      File.AppendAllText(logFilePath, message);

      if(Environment.UserInteractive) {
        Console.WriteLine(logMessage);
      }
    }

    protected override void OnStart(string[] args) {
      LogServiceEvent("Service Started");

      fileWatcher.Path = SourceFolder;
      fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
      fileWatcher.Filter = "*.*";

      fileWatcher.Created += new FileSystemEventHandler(OnCreated);

      fileWatcher.EnableRaisingEvents = true;
      fileWatcher.IncludeSubdirectories = false;

      LogServiceEvent($"File monitoring started on folder {SourceFolder}");
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
      try {
        string originalFilePath = e.FullPath;

        LogServiceEvent("New file detected in: {originalFilePath}");

        string fileExtension = Path.GetExtension(originalFilePath);
        string newFileName = Guid.NewGuid().ToString() + fileExtension;
        string newFilePath = Path.Combine(DestinationFolder, newFileName);

        Thread.Sleep(500);
        File.Move(originalFilePath, newFilePath);

        LogServiceEvent($"File renamed and moved: {originalFilePath} -> {newFilePath}");
      } catch(Exception ex) {
        LogServiceEvent(ex.ToString());
      }
    }

    protected override void OnStop() {
      LogServiceEvent("Service Stopped");

      fileWatcher.EnableRaisingEvents = false;
      fileWatcher.Dispose();
    }

    public void StartInConsole() {
      OnStart(null);

      Console.WriteLine("Press enter to stop the service");
      Console.ReadLine();
      OnStop();

      Console.ReadKey();
    }
  }
}
