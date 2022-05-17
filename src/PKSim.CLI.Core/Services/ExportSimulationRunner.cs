using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI.Core.Services
{
   public interface IExportSimulationRunner : IBatchRunner<ExportRunOptions>
   {
      Task<SimulationMapping[]> ExportSimulationsIn(PKSimProject project, ExportRunOptions exportRunOptions);
      Task<SimulationMapping> ExportSimulation(Simulation simulation, ExportRunOptions exportRunOptions, PKSimProject project);
   }

   public class ExportSimulationRunner : IExportSimulationRunner
   {
      private readonly IOSPSuiteLogger _logger;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly ICoreWorkspace _workspace;
      private readonly ISimulationExporter _simulationExporter;
      private readonly ILazyLoadTask _lazyLoadTask;

      public ExportSimulationRunner(
         IOSPSuiteLogger logger,
         IWorkspacePersistor workspacePersistor,
         ICoreWorkspace workspace,
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

      private async Task exportProject(ExportRunOptions runOptions)
      {
         var projectFile = runOptions.ProjectFile;
         if (!FileHelper.FileExists(projectFile))
            throw new OSPSuiteException($"Project file '{projectFile}' does not exist.");

         DirectoryHelper.CreateDirectory(runOptions.OutputFolder);

         _logger.AddInfo($"Starting project export for '{projectFile}'");

         _workspacePersistor.LoadSession(_workspace, projectFile);
         _logger.AddDebug($"Project loaded successfully from '{projectFile}'");

         await ExportSimulationsIn(_workspace.Project, runOptions);

         _logger.AddInfo($"Project export for '{projectFile}' terminated");
      }

      public async Task<SimulationMapping[]> ExportSimulationsIn(PKSimProject project, ExportRunOptions exportRunOptions)
      {
         var nameOfSimulationsToExport = (exportRunOptions.Simulations ?? Enumerable.Empty<string>()).ToList();
         if (!nameOfSimulationsToExport.Any() && exportRunOptions.ExportAllSimulationsIfListIsEmpty)
            nameOfSimulationsToExport.AddRange(project.All<Simulation>().AllNames());

         var simulationExports = new List<SimulationMapping>();

         //sequential for now
         foreach (var simulationName in nameOfSimulationsToExport)
         {
            var simulation = project.BuildingBlockByName<Simulation>(simulationName);
            if (simulation == null)
            {
               _logger.AddWarning($"Simulation '{simulationName}' was not found in project '{project.Name}'", project.Name);
               continue;
            }

            simulationExports.Add((await ExportSimulation(simulation, exportRunOptions, project)));
         }

         return simulationExports.ToArray();
      }

      public async Task<SimulationMapping> ExportSimulation(Simulation simulation, ExportRunOptions exportRunOptions, PKSimProject project)
      {
         var projectName = project.Name;
         var simulationName = simulation.Name;
         var simulationFile = FileHelper.RemoveIllegalCharactersFrom(simulationName);
         var simulationFolder = Path.Combine(exportRunOptions.OutputFolder, simulationFile);
         DirectoryHelper.CreateDirectory(simulationFolder);

         var simulationExport = new SimulationMapping
         {
            Project = projectName,
            Path = simulationFolder,
            Simulation = simulationName,
            SimulationFile = simulationFile
         };

         var simulationRunOptions = new SimulationRunOptions();

         _lazyLoadTask.Load(simulation);
         _lazyLoadTask.LoadResults(simulation);

         var simulationExportOptions = new SimulationExportOptions
         {
            OutputFolder = simulationFolder,
            ExportMode = exportRunOptions.ExportMode,
            ProjectName = projectName,
         };

         if (exportRunOptions.RunSimulation)
            await _simulationExporter.RunAndExport(simulation,  simulationRunOptions, simulationExportOptions);

         else 
            await _simulationExporter.Export(simulation,  simulationExportOptions);

         _logger.AddDebug($"Simulation '{simulationName}' exported to '{simulationFolder}'", projectName);
         return simulationExport;
      }
   }
}