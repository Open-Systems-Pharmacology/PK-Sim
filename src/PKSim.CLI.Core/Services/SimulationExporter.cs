using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
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
      /// <summary>
      ///    Results as json
      /// </summary>
      Json = 1 << 0,

      /// <summary>
      ///    Results as CSV
      /// </summary>
      Csv = 1 << 1,

      /// <summary>
      ///    Sim Model file (simulation used by sim model for Matlab and R)
      /// </summary>
      Xml = 1 << 2,

      /// <summary>
      ///    pkml Model (simulation used by mobi and pksim)
      /// </summary>
      Pkml = 1 << 3,

      /// <summary>
      ///    Export simulation results to excel (Individual simulations only)
      /// </summary>
      Xlsx = 1 << 4,

      All = Json | Csv | Xml | Pkml | Xlsx
   }

   public class SimulationExportOptions
   {
      private string _logCategory;
      public SimulationExportMode ExportMode { get; set; }
      public string ProjectName { get; set; }
      public string Category { get; set; }
      public bool UseDefaultExportName { get; set; } = true;
      public string OutputFolder { get; set; }
      public bool PrependProjectName { get; set; } = false;

      public string LogCategory
      {
         set => _logCategory = value;
         get => _logCategory ?? ProjectName;
      }

      public string TargetPathFor(Simulation simulation, string extension) =>
         TargetPathFor(simulation.Name, extension);

      public string TargetPathFor(string fileName, string extension) =>
         pathUnder(fileName, extension);

      public string TargetCSVPathFor(string fileName) => TargetPathFor(fileName, Constants.Filter.CSV_EXTENSION);

      private string pathUnder(string fileName, string extension)
      {
         var fileNameWithPrefix = fileName;
         if (PrependProjectName && !string.IsNullOrEmpty(ProjectName))
            fileNameWithPrefix = $"{ProjectName}-{fileName}";

         return Path.Combine(OutputFolder, $"{FileHelper.RemoveIllegalCharactersFrom(fileNameWithPrefix)}{extension}");
      }
   }

   public interface ISimulationExporter
   {
      Task RunAndExport(Simulation simulation, SimulationRunOptions simulationRunOptions, SimulationExportOptions simulationExportOptions);

      Task Export(Simulation simulation, SimulationExportOptions simulationExportOptions);
   }

   public class SimulationExporter : ISimulationExporter
   {
      private readonly ISimulationRunner _simulationRunner;
      private readonly IOSPLogger _logger;
      private readonly ISimulationResultsExporter _simulationResultsExporter;
      private readonly ISimulationExportTask _simulationExportTask;
      private readonly IPopulationExportTask _populationExportTask;
      private readonly IMoBiExportTask _moBiExportTask;

      public SimulationExporter(
         ISimulationRunner simulationRunner,
         IOSPLogger logger,
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
            tasks.Add(exportIndividualSimulationToCsvAsync(individualSimulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Csv) && populationSimulation != null)
            tasks.Add(exportPopulationSimulationToCsvAsync(populationSimulation, simulationExportOptions));

         if (exportMode.HasFlag(SimulationExportMode.Json) && individualSimulation != null)
            tasks.Add(exportResultsToJsonAsync(individualSimulation, simulationExportOptions));

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

      private async Task exportResultsToJsonAsync(IndividualSimulation simulation, SimulationExportOptions simulationExportOptions)
      {
         var fileName = simulationExportOptions.TargetPathFor(simulation, Constants.Filter.JSON_EXTENSION);
         await _simulationResultsExporter.ExportToJsonAsync(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"Exporting simulation results to '{fileName}'", simulationExportOptions.LogCategory);
      }

      private Task exportIndividualSimulationToCsvAsync(IndividualSimulation simulation, SimulationExportOptions simulationExportOptions) => 
         exportSimulationResultsToCsv(simulation, simulationExportOptions);

      private async Task exportPopulationSimulationToCsvAsync(PopulationSimulation populationSimulation, SimulationExportOptions simulationExportOptions)
      {
         var populationFile = CoreConstants.DefaultPopulationExportNameFor(populationSimulation.Name);
         var populationFileFullPath = simulationExportOptions.TargetCSVPathFor(populationFile);
         _populationExportTask.ExportToCSV(populationSimulation, populationFileFullPath);

         await exportSimulationResultsToCsv(populationSimulation, simulationExportOptions);

         var populationPKAnalysesFile = CoreConstants.DefaultPKAnalysesExportNameFor(populationSimulation.Name);
         var populationPKAnalysesFullPath = simulationExportOptions.TargetCSVPathFor(populationPKAnalysesFile);
         await _simulationExportTask.ExportPKAnalysesToCSVAsync(populationSimulation, populationPKAnalysesFullPath);
      }

      private async Task exportSimulationResultsToCsv(Simulation simulation, SimulationExportOptions simulationExportOptions)
      {
         var resultFileName = CoreConstants.DefaultResultsExportNameFor(simulation.Name);
         var simulationResultFileFullPath = simulationExportOptions.TargetCSVPathFor(resultFileName);
         await _simulationExportTask.ExportResultsToCSVAsync(simulation, simulationResultFileFullPath);
         _logger.AddDebug($"Exporting simulation results to '{simulationResultFileFullPath}'", simulationExportOptions.LogCategory);
      }

      private async Task exportIndividualSimulationResultsToExcelAsync(IndividualSimulation simulation, SimulationExportOptions simulationExportOptions)
      {
         var resultFileName = CoreConstants.DefaultResultsExportNameFor(simulation.Name);
         var simulationResultFileFullPath = simulationExportOptions.TargetPathFor(resultFileName, Constants.Filter.XLSX_EXTENSION);
         await _simulationExportTask.ExportResultsToExcelAsync(simulation, simulationResultFileFullPath, launchExcel:false);
         _logger.AddDebug($"Exporting simulation results to '{simulationResultFileFullPath}'", simulationExportOptions.LogCategory);
      }
   }
}