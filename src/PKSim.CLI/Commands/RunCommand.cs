using CommandLine;
using Microsoft.Extensions.Logging;

namespace PKSim.CLI.Commands
{
   public abstract class RunCommand
   {
      public LogLevel LogLevel { get; set; } = LogLevel.Information;

      [Option('l', "log", Required = false, HelpText = "Full path of log file where log output will be written.")]
      public string LogFileFullPath { get; set; }

      [Option('d', "debug", HelpText = "Show all notifications in log file with level debug or higher")]
      public bool Debug
      {
         set
         {
            if (value)
               LogLevel = LogLevel.Debug;
         }
      }

      [Option('w', "warning", HelpText = "Show all notifications in log file with level warning or higher")]
      public bool Warning
      {
         set
         {
            if (value)
               LogLevel = LogLevel.Warning;
         }
      }
   }

   public abstract class RunCommand<TRunOptions> : RunCommand
   {
      public abstract TRunOptions ToRunOptions();
   }
}