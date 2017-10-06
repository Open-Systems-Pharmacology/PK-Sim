using System;
using CommandLine;
using OSPSuite.Utility.Container;
using PKSim.CLI.Commands;
using PKSim.CLI.Core;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

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
         startCommand<SnapshotRunner, SnapshotRunOptions>("Snapshot run", snapshotRunOptions.ToRunOptions());
      }

      private static void startJsonRun(JsonRunCommand jsonRunOptions)
      {
         startCommand<JsonSimulationRunner, JsonRunOptions>("Batch run", jsonRunOptions.ToRunOptions());
      }

      private static void startCommand<TBatchRunner, TStartOptions>(string command, TStartOptions startOptions) where TBatchRunner : IBatchRunner<TStartOptions>
      {
         Console.WriteLine($"Starting {command.ToLowerInvariant()} with arguments:");
         Console.WriteLine(startOptions.ToString());
         ApplicationStartup.Start();
         var runner = IoC.Resolve<TBatchRunner>();
         runner.RunBatchAsync(startOptions).Wait();
         Console.WriteLine($"{command} finished");
      }
   }
}