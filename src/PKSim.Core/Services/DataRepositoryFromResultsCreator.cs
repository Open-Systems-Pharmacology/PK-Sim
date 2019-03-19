using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public interface IDataRepositoryFromResultsCreator
   {
      DataRepository CreateResultsFor(IndividualSimulation simulation);
   }

   public class DataRepositoryFromResultsCreator : IDataRepositoryFromResultsCreator
   {
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDataRepositoryTask _dataRepositoryTask;

      public DataRepositoryFromResultsCreator(IDimensionRepository dimensionRepository, IObjectPathFactory objectPathFactory, IDataRepositoryTask dataRepositoryTask)
      {
         _dimensionRepository = dimensionRepository;
         _objectPathFactory = objectPathFactory;
         _dataRepositoryTask = dataRepositoryTask;
      }

      public DataRepository CreateResultsFor(IndividualSimulation simulation)
      {
         if (!simulation.Results.Any())
            return new NullDataRepository();

         var dataRepository = createDataRepositoryWithBaseGrid(simulation);
         var timeColumn = dataRepository.BaseGrid;

         var individualResults = simulation.Results.First();
         individualResults.Select(value => dataColumnFor(value, simulation, timeColumn))
            .Where(col=> col!=null).Each(dataRepository.Add);

         return dataRepository;
      }

      /// <summary>
      ///    Returns the quantity and the consolidated object path of that quantity in the simulation (adding the simulation name
      ///    at the beginning)
      /// </summary>
      /// <param name="simulation"></param>
      /// <param name="path"></param>
      /// <returns></returns>
      private Tuple<IQuantity, IObjectPath> quantityAndPathFrom(Simulation simulation, IEnumerable<string> path)
      {
         var objectPath = _objectPathFactory.CreateObjectPathFrom(path).AndAddAtFront(simulation.Name);
         var quantity = objectPath.TryResolve<IQuantity>(simulation.Model.Root);
         return new Tuple<IQuantity, IObjectPath>(quantity, objectPath);
      }

      private DataRepository createDataRepositoryWithBaseGrid(Simulation simulation)
      {
         var timeColumn = timeColumnFrom(simulation.Results);
         return new DataRepository {timeColumn}.WithName(simulation.Name);
      }

      private DataColumn dataColumnFor(QuantityValues quantityValues, Simulation simulation, BaseGrid timeColumn)
      {
         var quantityAndPath = quantityAndPathFrom(simulation, quantityValues.PathList);
         var quantity = quantityAndPath.Item1;
         if (quantity == null)
            return null;

         var dimension = quantity.Dimension ?? _dimensionRepository.MolarConcentration;

         var column = createDataColumn(dataColumnIdFrom(quantityValues), quantity.Name, quantityAndPath.Item2, quantity, dimension, timeColumn, simulation);
         column.Values = quantityValues.Values;
         return column;
      }

      /// <summary>
      ///    create a bare bone datacolumn with the given id, name, path and dimension
      ///    Also makes sure that molweight is being updated for the created column
      /// </summary>
      private DataColumn createDataColumn(string colummId, string columnName, IEnumerable<string> path, IQuantity quantity,
         IDimension dimension, BaseGrid timeColumn, Simulation simulation)
      {
         var newColumn = new DataColumn(colummId, columnName, dimension, timeColumn)
         {
            QuantityInfo = new QuantityInfo(columnName, path, quantity.QuantityType),
            DataInfo = newDataInfo(dimension),
         };

         _dataRepositoryTask.UpdateMolWeight(newColumn, quantity, simulation.Model);
         return newColumn;
      }

      private DataInfo newDataInfo(IDimension dimension)
      {
         return new DataInfo(ColumnOrigins.Calculation) {DisplayUnitName = dimension.DefaultUnitName};
      }

      private BaseGrid timeColumnFrom(SimulationResults simulationResults)
      {
         var time = simulationResults.Time;
         return new BaseGrid(dataColumnIdFrom(time), Constants.TIME, _dimensionRepository.Time)
         {
            QuantityInfo = new QuantityInfo(Constants.TIME, new[] { Constants.TIME }, QuantityType.Time),
            Values = time.Values
         };
      }

      private string dataColumnIdFrom(QuantityValues quantityValues)
      {
         return quantityValues.ColumnId ?? ShortGuid.NewGuid();
      }
   }
}