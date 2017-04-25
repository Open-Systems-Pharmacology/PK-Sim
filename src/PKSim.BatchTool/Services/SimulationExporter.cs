using System.IO;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.BatchTool.Services
{
   public enum BatchExportMode
   {
      Json = 1 << 0,
      Csv = 1 << 1
   }

   public interface ISimulationExporter
   {
      void RunAndExport(IndividualSimulation simulation, string outputFolder, string exportFileName, SimulationConfiguration simulationConfiguration, BatchExportMode batchExportMode);
   }

   public class SimulationExporter : ISimulationExporter
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly IBatchLogger _logger;
      private readonly IParametersReportCreator _parametersReportCreator;
      private readonly ISimulationResultsExporter _simulationResultsExporter;

      public SimulationExporter(ISimulationEngineFactory simulationEngineFactory, IBatchLogger logger, IParametersReportCreator parametersReportCreator, ISimulationResultsExporter simulationResultsExporter)
      {
         _simulationEngineFactory = simulationEngineFactory;
         _logger = logger;
         _parametersReportCreator = parametersReportCreator;
         _simulationResultsExporter = simulationResultsExporter;
      }

      public void RunAndExport(IndividualSimulation simulation, string outputFolder, string exportFileName, SimulationConfiguration simulationConfiguration, BatchExportMode batchExportMode)
      {
         _logger.AddInfo($"------> Running simulation '{exportFileName}'");
         var engine = _simulationEngineFactory.Create<IndividualSimulation>();
         engine.RunForBatch(simulation, simulationConfiguration.CheckForNegativeValues);

         exportResults(simulation, outputFolder, exportFileName, batchExportMode);
      }

      private void exportResults(IndividualSimulation simulation, string outputFolder, string exportFileName, BatchExportMode batchExportMode)
      {
         if (batchExportMode.HasFlag(BatchExportMode.Csv))
            exportResultsToCsv(simulation, outputFolder, exportFileName);

         if (batchExportMode.HasFlag(BatchExportMode.Json))
            exportResultsToJson(simulation, outputFolder, exportFileName);
      }

      private void exportResultsToJson(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}.json");
         _simulationResultsExporter.ExportToJson(simulation, simulation.DataRepository, fileName);
         _logger.AddDebug($"------> Exporting simulation results to '{fileName}'");

      }

      private void exportResultsToCsv(IndividualSimulation simulation, string outputFolder, string exportFileName)
      {
         var fileName = Path.Combine(outputFolder, $"{exportFileName}.csv");
         _simulationResultsExporter.ExportToCsv(simulation, simulation.DataRepository, fileName);
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