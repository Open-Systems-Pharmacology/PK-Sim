using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI.Core.Services
{
   public class SimulationExport : IReferencingSimulation
   {
      //Name of project where simulation belongs
      public string Project { get; set; }

      //Name of exported simulation
      public string Simulation { get; set; }

      //Folder where simulation will be exported
      public string SimulationFolder { get; set; }
   }

   public interface IExportSimulationRunner : IBatchRunner<ExportRunOptions>
   {
      Task<SimulationExport[]> ExportSimulationsIn(PKSimProject project, ExportRunOptions exportRunOptions);
      Task<SimulationExport> ExportSimulation(Simulation simulation, ExportRunOptions exportRunOptions, PKSimProject project);
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

      public async Task<SimulationExport[]> ExportSimulationsIn(PKSimProject project, ExportRunOptions exportRunOptions)
      {
         var nameOfSimulationsToExport = (exportRunOptions.Simulations ?? Enumerable.Empty<string>()).ToList();
         if (!nameOfSimulationsToExport.Any())
            nameOfSimulationsToExport.AddRange(project.All<Simulation>().AllNames());

         var simulationExports = new List<SimulationExport>();

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

      public async Task<SimulationExport> ExportSimulation(Simulation simulation, ExportRunOptions exportRunOptions, PKSimProject project)
      {
         var projectName = project.Name;
         var simulationName = simulation.Name;
         var simulationFolder = Path.Combine(exportRunOptions.OutputFolder, FileHelper.RemoveIllegalCharactersFrom(simulationName));
         DirectoryHelper.CreateDirectory(simulationFolder);

         var simulationExport = new SimulationExport
         {
            Project = projectName,
            SimulationFolder = simulationFolder,
            Simulation = simulationName
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