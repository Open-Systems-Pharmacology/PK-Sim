using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI.Core.Services
{
   public class ExportSimulationRunner : IBatchRunner<ExportRunOptions>
   {
      private readonly ILogger _logger;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IWorkspace _workspace;
      private readonly ISimulationExporter _simulationExporter;
      private readonly ILazyLoadTask _lazyLoadTask;

      public ExportSimulationRunner(
         ILogger logger,
         IWorkspacePersistor workspacePersistor,
         IWorkspace workspace,
         ISimulationExporter simulationExporter, 
         ILazyLoadTask lazyLoadTask
      )
      {
         _logger = logger;
         _workspacePersistor = workspacePersistor;
         _workspace = workspace;
         _simulationExporter = simulationExporter;
         _lazyLoadTask = lazyLoadTask;
      }

      public Task RunBatchAsync(ExportRunOptions runOptions)
      {
         return Task.Run(() => exportProject(runOptions));
      }

      private async Task exportProject(ExportRunOptions exportRunOptions)
      {
         var projectFile = exportRunOptions.ProjectFile;
         if (!FileHelper.FileExists(projectFile))
            throw new OSPSuiteException($"Project file '{projectFile}' does not exist.");

         DirectoryHelper.CreateDirectory(exportRunOptions.OutputFolder);

         _logger.AddInSeparator($"Starting project export for '{projectFile}'", NotificationType.Info);

         _workspacePersistor.LoadSession(_workspace, projectFile);
         _logger.AddDebug($"Project loaded successfuly from '{projectFile}'");

         await exportSimulations(_workspace.Project, exportRunOptions);

         _logger.AddInSeparator($"Project export for '{projectFile}' terminated", NotificationType.Info);
      }

      private async Task exportSimulations(PKSimProject project, ExportRunOptions exportRunOptions)
      {
         var nameOfSimulationsToExport = (exportRunOptions.Simulations ?? Enumerable.Empty<string>()).ToList();
         if (!nameOfSimulationsToExport.Any())
            nameOfSimulationsToExport.AddRange(project.All<Simulation>().AllNames());

         //sequential for now
         foreach (var simulationName in nameOfSimulationsToExport)
         {
            var simulation = project.BuildingBlockByName<Simulation>(simulationName);
            if (simulation == null)
            {
               _logger.AddWarning($"Simulation '{simulationName}' was not found in project '{project.Name}'");
               continue;
            }

            await exportSimulation(simulation, exportRunOptions);
         }
      }

      private async Task exportSimulation(Simulation simulation, ExportRunOptions exportRunOptions)
      {
         var outputFolder = Path.Combine(exportRunOptions.OutputFolder, simulation.Name);
         DirectoryHelper.CreateDirectory(outputFolder);

         var simulationRunOptions = new SimulationRunOptions();

         _lazyLoadTask.Load(simulation);
         _lazyLoadTask.LoadResults(simulation);

         if (!simulation.OutputSelections.HasSelection)
         {
            _logger.AddWarning($"Simulation '{simulation.Name}' does not have any selected output and will not be exported");
            return;
         }

         if (exportRunOptions.RunSimulation)
            await _simulationExporter.RunAndExport(simulation, outputFolder, simulationRunOptions, exportRunOptions.ExportMode);

         else if (simulation.HasResults)
            await _simulationExporter.Export(simulation, outputFolder, exportRunOptions.ExportMode);

         else
            _logger.AddWarning($"Simulation '{simulation.Name}' does not have any results and will not be exported");
      }
   }
}