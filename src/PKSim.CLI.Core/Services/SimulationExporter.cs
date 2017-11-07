using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
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
      Task RunAndExport(Simulation simulation, string outputFolder, string exportFileName, SimulationRunOptions simulationRunOptions, SimulationExportMode simulationExportMode);

      Task Export(Simulation simulation, string outputFolder, string exportFileName, SimulationExportMode simulationExportMode);
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

      public async Task RunAndExport(Simulation simulation, string outputFolder, string exportFileName, SimulationRunOptions simulationRunOptions, SimulationExportMode simulationExportMode)
      {
         _logger.AddDebug($"Running simulation '{exportFileName}'");
         await _simulationRunner.RunSimulation(simulation, simulationRunOptions);

         await Export(simulation, outputFolder, exportFileName, simulationExportMode);
      }

      public Task Export(Simulation simulation, string outputFolder, string exportFileName, SimulationExportMode simulationExportMode)
      {
         var tasks = new List<Task>();

         var individualSimulation = simulation as IndividualSimulation;
         var populationSimulation = simulation as PopulationSimulation;

         if (simulationExportMode.HasFlag(SimulationExportMode.Csv) && individualSimulation != null)
            tasks.Add(exporIndividualSimulationToCsvAsync(individualSimulation, outputFolder, exportFileName));

         if (simulationExportMode.HasFlag(SimulationExportMode.Csv) && populationSimulation != null)
            tasks.Add(exportPopulationSimulationToCsvAsync(populationSimulation, outputFolder, exportFileName));

         if (simulationExportMode.HasFlag(SimulationExportMode.Json) && individualSimulation != null)
            tasks.Add(exportResultsToJsonAsync(individualSimulation, outputFolder, exportFileName));

         if (simulationExportMode.HasFlag(SimulationExportMode.Xml))
            tasks.Add(exportSimModelXmlAsync(simulation, outputFolder, exportFileName));

         if (simulationExportMode.HasFlag(SimulationExportMode.Pkml))
            tasks.Add(exportSimulationPkmlAsync(simulation, outputFolder, exportFileName));

         return Task.WhenAll(tasks);
      }

      private async Task exportSimulationPkmlAsync(Simulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}{Constants.Filter.PKML_EXTENSION}");
         await _moBiExportTask.SaveSimulationToFileAsync(simulation, fileName);
         _logger.AddDebug($"Exporting simulation pkml to '{fileName}'");
      }

      private async Task exportSimModelXmlAsync(Simulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}{Constants.Filter.XML_EXTENSION}");
         await _simulationExportTask.ExportSimulationToSimModelXmlAsync(simulation, fileName);
         _logger.AddDebug($"Exporting simulation SimModel xml to '{fileName}'");
      }

      private async Task exportResultsToJsonAsync(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}{Constants.Filter.JSON_EXTENSION}");
         await _simulationResultsExporter.ExportToJsonAsync(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"Exporting simulation results to '{fileName}'");
      }

      private async Task exporIndividualSimulationToCsvAsync(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         await exportSimulationResultsToCsv(simulation, outputFolder, exportFileName);

//         var fileName = csvPathUnder(outputFolder, exportFileName);
//         await _simulationResultsExporter.ExportToCsvAsync(simulation, simulation.DataRepository, fileName);
//
//         _logger.AddDebug($"Exporting simulation results to '{fileName}'");
         exportParameters(outputFolder, exportFileName, simulation);
      }

      private async Task exportPopulationSimulationToCsvAsync(PopulationSimulation populationSimulation, string outputFolder, string exportFileName)
      {
         var populationFile = CoreConstants.DefaultPopulationExportNameFor(exportFileName);
         var populationFileFullPath = csvPathUnder(outputFolder, populationFile);
         _populationExportTask.ExportToCSV(populationSimulation, populationFileFullPath);

         await exportSimulationResultsToCsv(populationSimulation, outputFolder, exportFileName);

         var populationPKAnalysesFile = CoreConstants.DefaultPKAnalysesExportNameFor(exportFileName);
         var populationPKAnalysesFullPath = csvPathUnder(outputFolder, populationPKAnalysesFile);
         await _simulationExportTask.ExportPKAnalysesToCSVAsync(populationSimulation, populationPKAnalysesFullPath);
      }

      private async Task exportSimulationResultsToCsv(Simulation simulation, string outputFolder, string exportFileName)
      {
         var simulationResultFile = CoreConstants.DefaultResultsExportNameFor(exportFileName);
         var simulationResultFileFullPath = csvPathUnder(outputFolder, simulationResultFile);
         await _simulationExportTask.ExportResultsToCSVAsync(simulation, simulationResultFileFullPath);
         _logger.AddDebug($"Exporting simulation results to '{simulationResultFileFullPath}'");
      }

      private void exportParameters(string outputFolder, string exportFileName, IndividualSimulation individualSimulation)
      {
         var parameterReportFileName = csvPathUnder(outputFolder, $"{exportFileName}-parameters");
         _parametersReportCreator.ExportParametersTo(individualSimulation.Model, parameterReportFileName);
         _logger.AddDebug($"Exporting simulation parameters to '{parameterReportFileName}'");
      }

      private string csvPathUnder(string outputFolder, string fileNameWithoutExtension)
      {
         return Path.Combine(outputFolder, $"{fileNameWithoutExtension}{Constants.Filter.CSV_EXTENSION}");
      }
   }
}