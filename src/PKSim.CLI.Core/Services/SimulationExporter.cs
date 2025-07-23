using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI.Core.Services
{
   public interface ISimulationExporter
   {
      Task RunAndExport(Simulation simulation, SimulationRunOptions simulationRunOptions, SimulationExportOptions simulationExportOptions);

      Task Export(Simulation simulation, SimulationExportOptions simulationExportOptions);
   }

   public class SimulationExporter : ISimulationExporter
   {
      private readonly ISimulationRunner _simulationRunner;
      private readonly IOSPSuiteLogger _logger;
      private readonly ISimulationResultsExporter _simulationResultsExporter;
      private readonly ISimulationExportTask _simulationExportTask;
      private readonly IPopulationExportTask _populationExportTask;
      private readonly IMoBiExportTask _moBiExportTask;

      public SimulationExporter(
         ISimulationRunner simulationRunner,
         IOSPSuiteLogger logger,
         ISimulationResultsExporter simulationResultsExporter,
         ISimulationExportTask simulationExportTask,
         IPopulationExportTask populationExportTask,
         IMoBiExportTask moBiExportTask
      )
      {
         _simulationRunner = simulationRunner;
         _logger = logger;
         _simulationResultsExporter = simulationResultsExporter;
         _simulationExportTask = simulationExportTask;
         _populationExportTask = populationExportTask;
         _moBiExportTask = moBiExportTask;
      }

      public async Task RunAndExport(Simulation simulation, SimulationRunOptions simulationRunOptions, SimulationExportOptions simulationExportOptions)
      {
         _logger.AddDebug($"Running simulation '{simulation.Name}'", simulationExportOptions.LogCategory);
         await _simulationRunner.RunSimulation(simulation, simulationRunOptions);

         await Export(simulation, simulationExportOptions);
      }

      public Task Export(Simulation simulation, SimulationExportOptions simulationExportOptions)
      {
         var tasks = new List<Task>();
         var individualSimulation = simulation as IndividualSimulation;
         var populationSimulation = simulation as PopulationSimulation;

         var exportMode = simulationExportOptions.ExportMode;

         if (exportMode.HasFlag(SimulationExportMode.Csv) && individualSimulation != null)
            tasks.Add(exportSimulationResultsToCsv(individualSimulation, simulationExportOptions));

         // Population always exported no matter what
         if (populationSimulation != null)
            tasks.Add(exportPopulationToCsvAsync(populationSimulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Csv) && populationSimulation != null)
            tasks.Add(exportPopulationSimulationResultsToCsvAsync(populationSimulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Json) && individualSimulation != null)
            tasks.Add(exportSimulationResultsToJsonAsync(individualSimulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Xlsx) && individualSimulation != null)
            tasks.Add(exportIndividualSimulationResultsToExcelAsync(individualSimulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Xml))
            tasks.Add(exportSimModelXmlAsync(simulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Pkml))
            tasks.Add(exportSimulationPkmlAsync(simulation, simulationExportOptions));

         return Task.WhenAll(tasks);
      }

      private async Task exportSimulationPkmlAsync(Simulation simulation, SimulationExportOptions simulationExportOptions)
      {
         var fileName = simulationExportOptions.TargetPathFor(simulation, Constants.Filter.PKML_EXTENSION);
         await _moBiExportTask.ExportSimulationToPkmlFileAsync(simulation, fileName);
         _logger.AddDebug($"Exporting simulation pkml to '{fileName}'", simulationExportOptions.LogCategory);
      }

      private async Task exportSimModelXmlAsync(Simulation simulation, SimulationExportOptions simulationExportOptions)
      {
         var fileName = simulationExportOptions.TargetPathFor(simulation, Constants.Filter.XML_EXTENSION);
         await _simulationExportTask.ExportSimulationToSimModelXmlAsync(simulation, fileName);
         _logger.AddDebug($"Exporting simulation SimModel xml to '{fileName}'", simulationExportOptions.LogCategory);
      }

      private async Task exportSimulationResultsToJsonAsync(IndividualSimulation simulation, SimulationExportOptions simulationExportOptions)
      {
         if (!simulation.HasResults)
         {
            _logger.AddWarning($"Simulation '{simulation.Name}' does not have any results and will not be exported to Json", simulationExportOptions.ProjectName);
            return;
         }

         var fileName = simulationExportOptions.TargetPathFor(simulation, Constants.Filter.JSON_EXTENSION);
         await _simulationResultsExporter.ExportToJsonAsync(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"Exporting simulation results to '{fileName}'", simulationExportOptions.LogCategory);
      }

      private Task exportPopulationToCsvAsync(PopulationSimulation populationSimulation, SimulationExportOptions simulationExportOptions)
      {
         var populationFile = CoreConstants.DefaultPopulationExportNameFor(populationSimulation.Name);
         var populationFileFullPath = simulationExportOptions.TargetCSVPathFor(populationFile);
         _populationExportTask.ExportToCSV(populationSimulation, populationFileFullPath);
         return Task.CompletedTask;
      }

      private async Task exportPopulationSimulationResultsToCsvAsync(PopulationSimulation populationSimulation, SimulationExportOptions simulationExportOptions)
      {
         await exportSimulationResultsToCsv(populationSimulation, simulationExportOptions);

         var populationPKAnalysesFile = CoreConstants.DefaultPKAnalysesExportNameFor(populationSimulation.Name);
         var populationPKAnalysesFullPath = simulationExportOptions.TargetCSVPathFor(populationPKAnalysesFile);
         await _simulationExportTask.ExportPKAnalysesToCSVAsync(populationSimulation, populationPKAnalysesFullPath);
      }

      private async Task exportSimulationResultsToCsv(Simulation simulation, SimulationExportOptions simulationExportOptions)
      {
         if (!simulation.HasResults)
         {
            _logger.AddWarning($"Simulation '{simulation.Name}' does not have any results and will not be exported to CSV", simulationExportOptions.ProjectName);
            return;
         }

         var resultFileName = CoreConstants.DefaultResultsExportNameFor(simulation.Name);
         var simulationResultFileFullPath = simulationExportOptions.TargetCSVPathFor(resultFileName);
         await _simulationExportTask.ExportResultsToCSVAsync(simulation, simulationResultFileFullPath);
         _logger.AddDebug($"Exporting simulation results to '{simulationResultFileFullPath}'", simulationExportOptions.LogCategory);
      }

      private async Task exportIndividualSimulationResultsToExcelAsync(IndividualSimulation simulation, SimulationExportOptions simulationExportOptions)
      {
         if (!simulation.HasResults)
         {
            _logger.AddWarning($"Simulation '{simulation.Name}' does not have any results and will not be exported to Excel", simulationExportOptions.ProjectName);
            return;
         }

         var resultFileName = CoreConstants.DefaultResultsExportNameFor(simulation.Name);
         var simulationResultFileFullPath = simulationExportOptions.TargetPathFor(resultFileName, Constants.Filter.XLSX_EXTENSION);
         await _simulationExportTask.ExportResultsToExcelAsync(simulation, simulationResultFileFullPath, launchExcel: false);
         _logger.AddDebug($"Exporting simulation results to '{simulationResultFileFullPath}'", simulationExportOptions.LogCategory);
      }
   }
}