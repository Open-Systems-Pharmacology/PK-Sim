using CommandLine;
using Microsoft.Extensions.Logging;

namespace PKSim.CLI.Commands
{
   public abstract class RunCommand
   {
      [Option('l', "log", Required = false, HelpText = "Full path of log file where log output will be written.")]
      public string LogFileFullPath { get; set; }

      [Option("logLevel", Required = false, HelpText = "Log file verbosity (Debug, Information, Warning, Error). Default is Information.")]
      public LogLevel LogLevel { get; set; } = LogLevel.Information;
   }

   public abstract class RunCommand<TRunOptions> : RunCommand
   {
      public abstract TRunOptions ToRunOptions();
   }
}