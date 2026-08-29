using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Logging;
using OSPSuite.Utility.Extensions;
using PKSim.Core;

namespace PKSim.CLI.Commands
{
   public abstract class CLICommand
   {
      public abstract string Name { get; }

      public virtual bool LogCommandName { get; } = true;

      [Option('l', "log", Required = false, HelpText = "Optional. Full path of log files where log output will be written. A log file will not be created if this value is not provided.")]
      public IEnumerable<string> LogFilesFullPath { get; set; } = new string[] { };

      [Option("logLevel", Required = false, HelpText = "Optional. Log verbosity (Debug, Information, Warning, Error). Default is Information.")]
      public LogLevel LogLevel { get; set; } = LogLevel.Information;

      [Option("cores", Required = false, HelpText = "Optional. Maximum number of cores (1 or more) to use for parallel work such as model construction and simulation runs. Default is the number of processors minus one.")]
      public int? NumberOfCores { get; set; }

      //parallelism gets exactly one owner per level: a caller managing process-level fan-out
      //(e.g. the QualificationRunner) passes --cores 1 so this process does not multiply it
      public void ApplyCoresTo(ICoreUserSettings userSettings)
      {
         if (NumberOfCores.HasValue)
            userSettings.MaximumNumberOfCoresToUse = Math.Max(1, NumberOfCores.Value);
      }

      protected virtual void LogDefaultOptions(StringBuilder sb)
      {
         LogFilesFullPath.Each(x => sb.AppendLine($"Log file: {x}"));
         sb.AppendLine($"Log level: {LogLevel}");
         if (NumberOfCores.HasValue)
            sb.AppendLine($"Number of cores: {NumberOfCores.Value}");
      }
   }

   public abstract class CLICommand<TRunOptions> : CLICommand
   {
      public abstract TRunOptions ToRunOptions();
   }
}
