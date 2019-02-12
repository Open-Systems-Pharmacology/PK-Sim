﻿using System.Collections.Generic;
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
   public class SimulationExport
   {
      public string ProjectName { get; set; }
      public string SimulationName { get; set; }
      public string SimulationFolder { get; set; }
   }

   public interface IExportSimulationRunner : IBatchRunner<ExportRunOptions>
   {
      Task<SimulationExport[]> ExportSimulationsIn(PKSimProject project, ExportRunOptions exportRunOptions);
      Task<SimulationExport> ExportSimulation(Simulation simulation, ExportRunOptions exportRunOptions, PKSimProject project);
   }

   public class ExportSimulationRunner : IExportSimulationRunner
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

      private async Task exportProject(ExportRunOptions runOptions)
      {
         var projectFile = runOptions.ProjectFile;
         if (!FileHelper.FileExists(projectFile))
            throw new OSPSuiteException($"Project file '{projectFile}' does not exist.");

         DirectoryHelper.CreateDirectory(runOptions.OutputFolder);

         _logger.AddInfo($"Starting project export for '{projectFile}'");

         _workspacePersistor.LoadSession(_workspace, projectFile);
         _logger.AddDebug($"Project loaded successfuly from '{projectFile}'");

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
            ProjectName = projectName,
            SimulationFolder = simulationFolder,
            SimulationName = simulationName
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

         if (!simulation.OutputSelections.HasSelection)
         {
            _logger.AddWarning($"Simulation '{simulation.Name}' does not have any selected output and will not be exported", projectName);
            return simulationExport;
         }

         if (exportRunOptions.RunSimulation)
            await _simulationExporter.RunAndExport(simulation,  simulationRunOptions, simulationExportOptions);

         else if (simulation.HasResults)
            await _simulationExporter.Export(simulation,  simulationExportOptions);

         else
         {
            _logger.AddWarning($"Simulation '{simulationName}' does not have any results and will not be exported", projectName);
            return simulationExport;
         }

         _logger.AddDebug($"Simulation '{simulationName}' exported to '{simulationFolder}'", projectName);
         return simulationExport;
      }
   }
}