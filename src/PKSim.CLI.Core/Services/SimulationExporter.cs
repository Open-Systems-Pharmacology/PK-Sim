﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI.Core.Services
{
   [Flags]
   public enum SimulationExportMode
   {
      Json = 1 << 0,
      Csv = 1 << 1,
      Xml = 1 << 2,
      Pkml = 1 << 3,
      All = Json | Csv | Xml | Pkml
   }

   public interface ISimulationExporter
   {
      Task RunAndExport(Simulation simulation, string outputFolder, SimulationRunOptions simulationRunOptions, SimulationExportMode simulationExportMode, string projectName = null, string category = null);

      Task Export(Simulation simulation, string outputFolder, SimulationExportMode simulationExportMode, string projectName = null, string category = null);
   }

   public class SimulationExporter : ISimulationExporter
   {
      private readonly ISimulationRunner _simulationRunner;
      private readonly ILogger _logger;
      private readonly IParametersReportCreator _parametersReportCreator;
      private readonly ISimulationResultsExporter _simulationResultsExporter;
      private readonly ISimulationExportTask _simulationExportTask;
      private readonly IPopulationExportTask _populationExportTask;
      private readonly IMoBiExportTask _moBiExportTask;

      public SimulationExporter(
         ISimulationRunner simulationRunner,
         ILogger logger,
         IParametersReportCreator parametersReportCreator,
         ISimulationResultsExporter simulationResultsExporter,
         ISimulationExportTask simulationExportTask,
         IPopulationExportTask populationExportTask,
         IMoBiExportTask moBiExportTask
      )
      {
         _simulationRunner = simulationRunner;
         _logger = logger;
         _parametersReportCreator = parametersReportCreator;
         _simulationResultsExporter = simulationResultsExporter;
         _simulationExportTask = simulationExportTask;
         _populationExportTask = populationExportTask;
         _moBiExportTask = moBiExportTask;
      }

      public async Task RunAndExport(Simulation simulation, string outputFolder, SimulationRunOptions simulationRunOptions, SimulationExportMode simulationExportMode, string projectName = null, string category = null)
      {
         var logCategory = logCategoryFrom(projectName, category);
         _logger.AddDebug($"Running simulation '{simulation.Name}'", logCategory);
         await _simulationRunner.RunSimulation(simulation, simulationRunOptions);

         await Export(simulation, outputFolder, simulationExportMode, projectName, logCategory);
      }

      public Task Export(Simulation simulation, string outputFolder, SimulationExportMode simulationExportMode, string projectName = null, string category = null)
      {
         var logCategory = logCategoryFrom(projectName, category);
         var tasks = new List<Task>();
         var individualSimulation = simulation as IndividualSimulation;
         var populationSimulation = simulation as PopulationSimulation;

         if (simulationExportMode.HasFlag(SimulationExportMode.Csv) && individualSimulation != null)
            tasks.Add(exporIndividualSimulationToCsvAsync(individualSimulation, outputFolder, projectName, logCategory));

         if (simulationExportMode.HasFlag(SimulationExportMode.Csv) && populationSimulation != null)
            tasks.Add(exportPopulationSimulationToCsvAsync(populationSimulation, outputFolder, projectName, logCategory));

         if (simulationExportMode.HasFlag(SimulationExportMode.Json) && individualSimulation != null)
            tasks.Add(exportResultsToJsonAsync(individualSimulation, outputFolder, projectName, logCategory));

         if (simulationExportMode.HasFlag(SimulationExportMode.Xml))
            tasks.Add(exportSimModelXmlAsync(simulation, outputFolder, projectName, logCategory));

         if (simulationExportMode.HasFlag(SimulationExportMode.Pkml))
            tasks.Add(exportSimulationPkmlAsync(simulation, outputFolder, projectName, logCategory));

         return Task.WhenAll(tasks);
      }

      private async Task exportSimulationPkmlAsync(Simulation simulation, string outputFolder, string projectName, string logCategory)
      {
         var fileName = pathUnder(outputFolder, simulation.Name, Constants.Filter.PKML_EXTENSION, projectName);
         await _moBiExportTask.SaveSimulationToFileAsync(simulation, fileName);
         _logger.AddDebug($"Exporting simulation pkml to '{fileName}'", logCategory);
      }

      private async Task exportSimModelXmlAsync(Simulation simulation, string outputFolder, string projectName, string logCategory)
      {
         var fileName = pathUnder(outputFolder, simulation.Name, Constants.Filter.XML_EXTENSION, projectName);
         await _simulationExportTask.ExportSimulationToSimModelXmlAsync(simulation, fileName);
         _logger.AddDebug($"Exporting simulation SimModel xml to '{fileName}'", logCategory);
      }

      private async Task exportResultsToJsonAsync(IndividualSimulation simulation, string outputFolder, string projectName, string logCategory)
      {
         var fileName = pathUnder(outputFolder, simulation.Name, Constants.Filter.JSON_EXTENSION, projectName);
         await _simulationResultsExporter.ExportToJsonAsync(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"Exporting simulation results to '{fileName}'", logCategory);
      }

      private async Task exporIndividualSimulationToCsvAsync(IndividualSimulation simulation, string outputFolder, string projectName, string logCategory)
      {
         await exportSimulationResultsToCsv(simulation, outputFolder, projectName, logCategory);
         exportParameters(outputFolder, simulation, projectName, logCategory);
      }

      private async Task exportPopulationSimulationToCsvAsync(PopulationSimulation populationSimulation, string outputFolder, string projectName, string logCategory)
      {
         var populationFile = CoreConstants.DefaultPopulationExportNameFor(populationSimulation.Name);
         var populationFileFullPath = csvPathUnder(outputFolder, populationFile, projectName);
         _populationExportTask.ExportToCSV(populationSimulation, populationFileFullPath);

         await exportSimulationResultsToCsv(populationSimulation, outputFolder, projectName, logCategory);

         var populationPKAnalysesFile = CoreConstants.DefaultPKAnalysesExportNameFor(populationSimulation.Name);
         var populationPKAnalysesFullPath = csvPathUnder(outputFolder, populationPKAnalysesFile, projectName);
         await _simulationExportTask.ExportPKAnalysesToCSVAsync(populationSimulation, populationPKAnalysesFullPath);
      }

      private async Task exportSimulationResultsToCsv(Simulation simulation, string outputFolder, string projectName, string logCategory)
      {
         var simulationResultFileFullPath = csvPathUnder(outputFolder, simulation.Name, projectName);
         await _simulationExportTask.ExportResultsToCSVAsync(simulation, simulationResultFileFullPath);
         _logger.AddDebug($"Exporting simulation results to '{simulationResultFileFullPath}'", logCategory);
      }

      private void exportParameters(string outputFolder, IndividualSimulation individualSimulation, string projectName, string logCategory)
      {
         var parameterReportFileName = csvPathUnder(outputFolder, $"{individualSimulation.Name}_parameters", projectName);
         _parametersReportCreator.ExportParametersTo(individualSimulation.Model, parameterReportFileName);
         _logger.AddDebug($"Exporting simulation parameters to '{parameterReportFileName}'", logCategory);
      }

      private string csvPathUnder(string outputFolder, string fileNameWithoutExtension, string projectName) => pathUnder(outputFolder, fileNameWithoutExtension, Constants.Filter.CSV_EXTENSION, projectName);

      private string pathUnder(string outputFolder, string fileName, string extension, string projectName)
      {
         var fileNameWithPrefix = fileName;
         if (!string.IsNullOrEmpty(projectName))
            fileNameWithPrefix = $"{projectName}-{fileName}";

         return Path.Combine(outputFolder, $"{FileHelper.RemoveIllegalCharactersFrom(fileNameWithPrefix)}{extension}");
      }

      private string logCategoryFrom(string projectName, string category) => category ?? projectName;
   }
}