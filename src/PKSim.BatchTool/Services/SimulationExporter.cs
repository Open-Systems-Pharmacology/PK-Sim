using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.BatchTool.Services
{
   public enum BatchExportMode
   {
      Json = 1 << 0,
      Csv = 1 << 1,
      Xml = 1 << 2,
      All = Json | Csv | Xml
   }

   public interface ISimulationExporter
   {
      Task RunAndExport(IndividualSimulation simulation, string outputFolder, string exportFileName, SimulationConfiguration simulationConfiguration, BatchExportMode batchExportMode);
   }

   public class SimulationExporter : ISimulationExporter
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly IBatchLogger _logger;
      private readonly IParametersReportCreator _parametersReportCreator;
      private readonly ISimulationResultsExporter _simulationResultsExporter;
      private readonly ISimulationExportTask _simulationExportTask;

      public SimulationExporter(ISimulationEngineFactory simulationEngineFactory, IBatchLogger logger, IParametersReportCreator parametersReportCreator,
         ISimulationResultsExporter simulationResultsExporter, ISimulationExportTask simulationExportTask)
      {
         _simulationEngineFactory = simulationEngineFactory;
         _logger = logger;
         _parametersReportCreator = parametersReportCreator;
         _simulationResultsExporter = simulationResultsExporter;
         _simulationExportTask = simulationExportTask;
      }

      public async Task RunAndExport(IndividualSimulation simulation, string outputFolder, string exportFileName, SimulationConfiguration simulationConfiguration, BatchExportMode batchExportMode)
      {
         _logger.AddDebug($"------> Running simulation '{exportFileName}'");
         var engine = _simulationEngineFactory.Create<IndividualSimulation>();
         await engine.RunForBatch(simulation, simulationConfiguration.CheckForNegativeValues);

         await exportResults(simulation, outputFolder, exportFileName, batchExportMode);
      }

      private Task exportResults(IndividualSimulation simulation, string outputFolder, string exportFileName, BatchExportMode batchExportMode)
      {
         var tasks = new List<Task>();
         if (batchExportMode.HasFlag(BatchExportMode.Csv))
            tasks.Add(exportResultsToCsv(simulation, outputFolder, exportFileName));

         if (batchExportMode.HasFlag(BatchExportMode.Json))
            tasks.Add(exportResultsToJson(simulation, outputFolder, exportFileName));

         if (batchExportMode.HasFlag(BatchExportMode.Xml))
            tasks.Add(exportSimModelXmlAsync(simulation, outputFolder, exportFileName));

         return Task.WhenAll(tasks);
      }

      private async Task exportSimModelXmlAsync(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         var xmlOutputFolder = Path.Combine(outputFolder, "Xml");
         DirectoryHelper.CreateDirectory(xmlOutputFolder);
         var fileName = Path.Combine(xmlOutputFolder, $"{exportFileName}.xml");
         await _simulationExportTask.ExportSimulationToSimModelXmlAsync(simulation, fileName);
         _logger.AddDebug($"------> Exporting SimModel xml to '{fileName}'");
      }

      private async Task exportResultsToJson(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}.json");
         await _simulationResultsExporter.ExportToJsonAsync(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"------> Exporting simulation results to '{fileName}'");
      }

      private async Task exportResultsToCsv(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}.csv");
         await _simulationResultsExporter.ExportToCsvAsync(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"------> Exporting simulation results to '{fileName}'");
         exportParameters(outputFolder, exportFileName, simulation);
      }

      private void exportParameters(string outputFolder, string exportFileName, IndividualSimulation individualSimulation)
      {
         var parameterReportFileName = Path.Combine(outputFolder, $"{exportFileName}_parameters.csv");
         _logger.AddDebug($"------> Exporting simulation parameters to '{parameterReportFileName}'");
         _parametersReportCreator.ExportParametersTo(individualSimulation.Model, parameterReportFileName);
      }
   }
}