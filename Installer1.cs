using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace MyFirstService {
  [RunInstaller(true)]
  public partial class Installer1 : Installer {
    private ServiceProcessInstaller serviceProcessInstaller;
    private ServiceInstaller serviceInstaller;

    public Installer1() {
      InitializeComponent();

      // Configure the Service Process Installer
      serviceProcessInstaller = new ServiceProcessInstaller {
        // Adjust as needed (e.g., NetworkService, LocalService)
        Account = ServiceAccount.LocalSystem
      };

      // Configure the Service Installer
      serviceInstaller = new ServiceInstaller {
        // Must match the ServiceName in your ServiceBase class
        ServiceName = "FileMonitoringService",
        DisplayName = "Monitor new files in specific folder",
        // Or Automatic, depending on requirements
        StartType = ServiceStartMode.Automatic,
        Description = "this is my file service watcher in folder",
        ServicesDependedOn = new string[] { "RpcSs", "EventLog", "LanmanWorkstation" }
      };

      // Add installers to the installer collection
      Installers.Add(serviceProcessInstaller);
      Installers.Add(serviceInstaller);
    }
  }
}
