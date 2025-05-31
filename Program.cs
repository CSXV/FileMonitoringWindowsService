using System;
using System.ServiceProcess;

namespace MyFirstService {
  internal static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /*static void Main() {
      ServiceBase[] ServicesToRun;
      ServicesToRun = new ServiceBase[] {
                new Service1()
      };

      ServiceBase.Run(ServicesToRun);
    }*/

    static void Main() {
      if(Environment.UserInteractive) {
        Console.WriteLine("Running in console mode...");
        Service1 service = new Service1();

        service.StartInConsole();
      } else {
        ServiceBase[] ServicesToRun;
        ServicesToRun = new ServiceBase[] {
                new Service1()
      };

        ServiceBase.Run(ServicesToRun);

      }
    }
  }
}
