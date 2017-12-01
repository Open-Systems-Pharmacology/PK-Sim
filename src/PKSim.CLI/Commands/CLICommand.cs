using CommandLine;
using Microsoft.Extensions.Logging;

namespace PKSim.CLI.Commands
{
   public abstract class CLICommand
   {
      public abstract string Name { get; }

      [Option('l', "log", Required = false, HelpText = "Optional. Full path of log file where log output will be written. A log file will not be created if this value is not provided.")]
      public string LogFileFullPath { get; set; }

      [Option('a', "append", Required = false, HelpText = "Optional. true to append data to the file; false to overwrite the file (default). If the specified file does not exist, this parameter has no effect, and a new file is created. ")]
      public bool AppendToLog { get; set; }

      [Option("logLevel", Required = false, HelpText = "Optional. Log verbosity (Debug, Information, Warning, Error). Default is Information.")]
      public LogLevel LogLevel { get; set; } = LogLevel.Information;
   }

   public abstract class CLICommand<TRunOptions> : CLICommand
   {
      public abstract TRunOptions ToRunOptions();
   }
}