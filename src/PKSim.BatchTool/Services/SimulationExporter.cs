using System.IO;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Batch;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Mappers;

namespace PKSim.BatchTool.Services
{
   public interface ISimulationExporter
   {
      void RunAndExport(IndividualSimulation simulation, string outputFolder, string exportFileName, SimulationConfiguration simulationConfiguration);
   }

   public class SimulationExporter : ISimulationExporter
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      private readonly IBatchLogger _logger;
      private readonly IParametersReportCreator _parametersReportCreator;

      public SimulationExporter(ISimulationEngineFactory simulationEngineFactory, IDataRepositoryTask dataRepositoryTask,
                                IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, IBatchLogger logger, IParametersReportCreator parametersReportCreator)
      {
         _simulationEngineFactory = simulationEngineFactory;
         _dataRepositoryTask = dataRepositoryTask;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
         _logger = logger;
         _parametersReportCreator = parametersReportCreator;
      }

      public void RunAndExport(IndividualSimulation simulation, string outputFolder, string exportFileName, SimulationConfiguration simulationConfiguration)
      {
         var individualSimulation = simulation.DowncastTo<IndividualSimulation>();
         var fileName = Path.Combine(outputFolder, string.Format("{0}.csv", exportFileName));
         var parameterReportFileName = Path.Combine(outputFolder, string.Format("{0}_parameters.csv", exportFileName));
         _logger.AddInfo("------> Running simulation '{0}'".FormatWith(exportFileName));
         var engine = _simulationEngineFactory.Create<IndividualSimulation>();
         engine.RunForBatch(individualSimulation, simulationConfiguration.CheckForNegativeValues);

         _logger.AddDebug("------> Exporting simulation results to '{0}'".FormatWith(exportFileName));
         var dataTable = _dataRepositoryTask.ToDataTable(simulation.DataRepository, x => _quantityDisplayPathMapper.DisplayPathAsStringFor(simulation, x)).First();
         dataTable.ExportToCSV(fileName);

         _logger.AddDebug("------> Exporting simulation parameters to '{0}'".FormatWith(parameterReportFileName));
         _parametersReportCreator.ExportParametersTo(individualSimulation.Model, parameterReportFileName);
      }
   }
}