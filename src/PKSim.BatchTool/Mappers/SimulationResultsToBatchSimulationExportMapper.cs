using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Services;

namespace PKSim.BatchTool.Mappers
{
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
            //TODO READ THRESHOLD FROM COLUMN EXTENDED PROPERTIES WHEN READY
            Threshold = 1.5f, 
            Values = displayValuesFor(column)
         };
      }

      private float[] displayValuesFor(DataColumn column)
      {
         return column.ConvertToDisplayValues(column.Values).ToArray();
      }
   }
}