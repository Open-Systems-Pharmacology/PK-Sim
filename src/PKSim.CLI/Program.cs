using System;
using CommandLine;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using PKSim.CLI.Commands;
using PKSim.CLI.Core.Services;
using PKSim.CLI.Services;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.CLI
{
   [Flags]
   enum ExitCodes
   {
      Success = 0,
      Error = 1 << 0,
   }

   class Program
   {
      static int Main(string[] args)
      {
         //starting batch tool with arguments
         var valid = true;

         ApplicationStartup.Initialize();

         Parser.Default.ParseArguments<JsonRunCommand, SnapshotRunCommand, ExportRunCommand, BatchConverterCommand>(args)
            .WithParsed<JsonRunCommand>(startCommand)
            .WithParsed<SnapshotRunCommand>(startCommand)
            .WithParsed<ExportRunCommand>(startCommand)
            .WithParsed<BatchConverterCommand>(startCommand)
            .WithNotParsed(err => valid = false);

         if (!valid)
            return (int) ExitCodes.Error;

         return (int) ExitCodes.Success;
      }

      private static void startCommand<TRunOptions>(CLICommand<TRunOptions> command)   
      {
         var logger = initializeLogger(command);
         logger.AddInfo($"Starting {command.Name.ToLower()} run with arguments:\n{command}");
         ApplicationStartup.Start();
         var runner = IoC.Resolve<IBatchRunner<TRunOptions>>();
         runner.RunBatchAsync(command.ToRunOptions()).Wait();
         logger.AddInfo($"{command.Name} run finished");
      }

      private static ILogger initializeLogger(CLICommand runCommand)
      {
         var loggerFactory = IoC.Resolve<ILoggerFactory>();

         loggerFactory
            .AddConsole(runCommand.LogLevel);

         if (!string.IsNullOrEmpty(runCommand.LogFileFullPath))
            loggerFactory.AddFile(runCommand.LogFileFullPath, runCommand.LogLevel, runCommand.AppendToLog);

         return IoC.Resolve<ILogger>();
      }
   }
}