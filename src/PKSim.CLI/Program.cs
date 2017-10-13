using System;
using CommandLine;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using PKSim.CLI.Commands;
using PKSim.CLI.Core.RunOptions;
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

         Parser.Default.ParseArguments<JsonRunCommand, SnapshotRunCommand>(args)
            .WithParsed<JsonRunCommand>(startJsonRun)
            .WithParsed<SnapshotRunCommand>(startSnapshotsRun)
            .WithNotParsed(err => { valid = false; });

         if (!valid)
            return (int) ExitCodes.Error;

         return (int) ExitCodes.Success;
      }

      private static void startSnapshotsRun(SnapshotRunCommand snapshotRunOptions)
      {
         startCommand<SnapshotRunner, SnapshotRunOptions>("Snapshot run", snapshotRunOptions);
      }

      private static void startJsonRun(JsonRunCommand jsonRunOptions)
      {
         startCommand<JsonSimulationRunner, JsonRunOptions>("Batch run", jsonRunOptions);
      }

      private static void startCommand<TBatchRunner, TRunOptions>(string command, RunCommand<TRunOptions> runCommand) where TBatchRunner : IBatchRunner<TRunOptions>
      {
         var logger = initializeLogger(runCommand);
         logger.AddInfo($"Starting {command} with arguments:\n{runCommand}");
         ApplicationStartup.Start();
         var runner = IoC.Resolve<TBatchRunner>();
         runner.RunBatchAsync(runCommand.ToRunOptions()).Wait();
         logger.AddInfo($"{command} finished");
      }

      private static ILogger initializeLogger(RunCommand runCommand)
      {
         var loggerFactory = IoC.Resolve<ILoggerFactory>();
     
         loggerFactory
            .AddConsole(runCommand.LogLevel);

         if(!string.IsNullOrEmpty(runCommand.LogFileFullPath))
            loggerFactory.AddFile(runCommand.LogFileFullPath);

         return IoC.Resolve<ILogger>();
      }
   }
}