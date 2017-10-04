using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CommandLine;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool
{
   [Flags]
   enum ExitCodes
   {
      Success = 0,
      Error = 1 << 0,
   }

   static class Program
   {
      [DllImport("kernel32.dll")]
      private static extern bool AttachConsole(int dwProcessId);

      private const int ATTACH_PARENT_PROCESS = -1;

      /// <summary>
      ///    The main entry point for the application.
      /// </summary>
      [STAThread]
      static int Main(string[] args)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         try
         {
            if (args.Length == 0)
               startAsStandaloneApplication();
            else
            {
               AttachConsole(ATTACH_PARENT_PROCESS);

               //starting batch tool with arguments
               var valid = true;

               Parser.Default.ParseArguments<ProjectComparisonOptions, JsonRunOptions, ProjectOverviewOptions, TrainingMaterialsOptions, SnapshotRunOptions>(args)
                  .WithParsed<ProjectComparisonOptions>(startProjectComparison)
                  .WithParsed<JsonRunOptions>(startJsonRun)
                  .WithParsed<ProjectOverviewOptions>(generateProjectOverviews)
                  .WithParsed<TrainingMaterialsOptions>(generateTrainingMaterials)
                  .WithParsed<SnapshotRunOptions>(startSnapshotsRun)
                  .WithNotParsed(err => { valid = false; });

               if (!valid)
                  return (int) ExitCodes.Error;
            }

            return (int) ExitCodes.Success;
         }
         catch (Exception e)
         {
            e.LogError();
            return (int) ExitCodes.Error;
         }
      }

      private static void startSnapshotsRun(SnapshotRunOptions snapshotRunOptions)
      {
         startCommand("Snapshot run", snapshotRunOptions, x => x.StartSnapshotsRun);
      }

      private static void startJsonRun(JsonRunOptions jsonRunOptions)
      {
         startCommand("Batch run", jsonRunOptions, x => x.StartBatchRun);
      }

      private static void startProjectComparison(ProjectComparisonOptions comparisonOptions)
      {
         startCommand("Project comparison", comparisonOptions, x => x.StartProjectComparison);
      }

      private static void generateProjectOverviews(ProjectOverviewOptions projectOverviewOptions)
      {
         startCommand("Project overview", projectOverviewOptions, x => x.GenerateProjectOverview);
      }

      private static void generateTrainingMaterials(TrainingMaterialsOptions trainingMaterialsOptions)
      {
         startCommand("Training materials", trainingMaterialsOptions, x => x.GenerateTrainingMaterial);
      }

      private static void startCommand<TStartOptions>(string command, TStartOptions startOptions, Func<IBatchMainPresenter, Action<TStartOptions>> commandAction)
      {
         Console.WriteLine($"Starting {command.ToLowerInvariant()} with arguments:");
         Console.WriteLine(startOptions.ToString());
         BatchStarter.Start();
         var mainPresenter = IoC.Resolve<IBatchMainPresenter>();
         commandAction(mainPresenter)(startOptions);
         Application.Run(new ApplicationContext());
         Console.WriteLine($"{command} finished");
      }

      private static void startAsStandaloneApplication()
      {
         BatchStarter.Start();
         var mainPresenter = IoC.Resolve<IBatchMainPresenter>();
         Application.Run(mainPresenter.BaseView.DowncastTo<Form>());
      }
   }
}