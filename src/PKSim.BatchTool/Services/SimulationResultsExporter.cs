using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility.Extensions;

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

   public interface ISimulationResultsToBatchSimulationExportMapper
   {
      BatchSimulationExport MapFrom(ISimulation simulation, DataRepository results);
   }

   public class SimulationResultsToBatchSimulationExportMapper : ISimulationResultsToBatchSimulationExportMapper
   {
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      private readonly IObjectPathFactory _objectPathFactory;

      public SimulationResultsToBatchSimulationExportMapper(IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, IObjectPathFactory objectPathFactory)
      {
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
         _objectPathFactory = objectPathFactory;
      }

      public BatchSimulationExport MapFrom(ISimulation simulation, DataRepository results)
      {
         var simulationExport = new BatchSimulationExport
         {
            Name = simulation.Name,
            Time = displayValuesFor(results.BaseGrid),
            ParameterValues = parameterValuesFor(simulation.Model)
         };

         results.AllButBaseGrid().Each(c=>simulationExport.OutputValues.Add(quantityResultsFrom(simulation, c)));

         return simulationExport;
      }

      private List<ParameterValue> parameterValuesFor(IModel simulationModel)
      {
         return  simulationModel.Root.GetAllChildren<IParameter>().Select(p => new ParameterValue
         {
            Path = _objectPathFactory.CreateAbsoluteObjectPath(p).PathAsString,
            Value = p.Value
         }).ToList();
      }

      private BatchOutputValues quantityResultsFrom(ISimulation simulation, DataColumn column)
      {
         return new BatchOutputValues
         {
            Path = _quantityDisplayPathMapper.DisplayPathAsStringFor(simulation, column),
            Threshold = 1.5f, //TODO
            Values = displayValuesFor(column)
         };
      }

      private float[] displayValuesFor(DataColumn column)
      {
         return column.ConvertToDisplayValues(column.Values).ToArray();
      }
   }

   public class BatchSimulationExport
   {
      public string Name { get; set; }
      public float[] Time { get; set; }
      public List<BatchOutputValues> OutputValues { get; set; } = new List<BatchOutputValues>();
      public List<ParameterValue> ParameterValues { get; set; } = new List<ParameterValue>();
   }

   public class BatchOutputValues
   {
      public string Path { get; set; }
      public float[] Values { get; set; }
      public double Threshold { get; set; }
   }

   public class ParameterValue
   {
      public string Path { get; set; }
      public double Value { get; set; }
   }
}