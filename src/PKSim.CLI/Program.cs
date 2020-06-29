using System;
using CommandLine;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using PKSim.CLI.Commands;
using PKSim.CLI.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Services;

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
      static bool _valid = true;

      static int Main(string[] args)
      {

         ApplicationStartup.Initialize();

         Parser.Default.ParseArguments<JsonRunCommand, SnapshotRunCommand, ExportRunCommand, QualificationRunCommand>(args)
            .WithParsed<JsonRunCommand>(startCommand)
            .WithParsed<SnapshotRunCommand>(startCommand)
            .WithParsed<ExportRunCommand>(startCommand)
            .WithParsed<QualificationRunCommand>(startCommand)
            .WithNotParsed(err => _valid = false);

         if (!_valid)
            return (int)ExitCodes.Error;

         return (int)ExitCodes.Success;
      }

      private static void startCommand<TRunOptions>(CLICommand<TRunOptions> command)
      {
         var logger = initializeLogger(command);
         if (command.LogCommandName)
            logger.AddInfo($"Starting {command.Name.ToLower()} run");

         logger.AddDebug($"Arguments:\n{command}");
         ApplicationStartup.Start();
         var runner = IoC.Resolve<IBatchRunner<TRunOptions>>();
         try
         {
            runner.RunBatchAsync(command.ToRunOptions()).Wait();
         }
         catch (Exception e)
         {
            logger.AddException(e);
            _valid = false;
         }

         if (command.LogCommandName)
            logger.AddInfo($"{command.Name} run finished");
      }

      private static IOSPLogger initializeLogger(CLICommand runCommand)
      {

         var loggerCreator = IoC.Resolve<ILoggerCreator>();

         loggerCreator.AddLoggingBuilderConfiguration(builder =>
           builder
             .SetMinimumLevel(runCommand.LogLevel)
             .AddConsole()
         );

         if (!string.IsNullOrEmpty(runCommand.LogFileFullPath))
            loggerCreator.AddLoggingBuilderConfiguration(builder =>
              builder
                .AddFile(runCommand.LogFileFullPath)
            );
         return IoC.Resolve<IOSPLogger>();
      }
   }
}