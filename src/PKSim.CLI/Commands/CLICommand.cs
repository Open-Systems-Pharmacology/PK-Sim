﻿using System.Collections.Generic;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace PKSim.CLI.Commands
{
   public abstract class CLICommand
   {
      public abstract string Name { get; }

      public virtual bool LogCommandName { get; } = true;

      [Option('l', "log", Required = false, HelpText = "Optional. Full path of log files where log output will be written. A log file will not be created if this value is not provided.")]
      public IEnumerable<string> LogFilesFullPath { get; set; } = new string[] { };

      [Option('a', "append", Required = false, HelpText = "Optional. true to append data to the file; false to overwrite the file (default). If the specified file does not exist, this parameter has no effect, and a new file is created. ")]
      public bool AppendToLog { get; set; }

      [Option("logLevel", Required = false, HelpText = "Optional. Log verbosity (Debug, Information, Warning, Error). Default is Information.")]
      public LogLevel LogLevel { get; set; } = LogLevel.Information;

      protected virtual void LogDefaultOptions(StringBuilder sb)
      {
         sb.AppendLine($"Log file: {LogFilesFullPath}");
         sb.AppendLine($"Log level: {LogLevel}");
      }
   }

   public abstract class CLICommand<TRunOptions> : CLICommand
   {
      public abstract TRunOptions ToRunOptions();
   }
}