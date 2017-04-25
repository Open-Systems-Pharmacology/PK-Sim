using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Mappers;

namespace PKSim.BatchTool.Services
{
   public interface ISimulationResultsExporter
   {
      void ExportToCsv(ISimulation simulation, DataRepository results, string fileName);
      void ExportToJson(ISimulation simulation, DataRepository results, string fileName);
   }

   public class SimulationResultsExporter : ISimulationResultsExporter
   {
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      private readonly ISimulationResultsToBatchSimulationExportMapper _simulationExportMapper;

      public SimulationResultsExporter(IDataRepositoryTask dataRepositoryTask, IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, ISimulationResultsToBatchSimulationExportMapper simulationExportMapper)
      {
         _dataRepositoryTask = dataRepositoryTask;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
         _simulationExportMapper = simulationExportMapper;
      }

      public void ExportToCsv(ISimulation simulation, DataRepository results, string fileName)
      {
         var dataTable = _dataRepositoryTask.ToDataTable(results, x => _quantityDisplayPathMapper.DisplayPathAsStringFor(simulation, x)).First();
         dataTable.ExportToCSV(fileName);
      }

      public void ExportToJson(ISimulation simulation, DataRepository results, string fileName)
      {
         var exportResults = _simulationExportMapper.MapFrom(simulation, results);
         // serialize JSON directly to a file
         using (var file = File.CreateText(fileName))
         {
            var serializer = new JsonSerializer();
            serializer.Serialize(file, exportResults);
         }
      }
   }

}