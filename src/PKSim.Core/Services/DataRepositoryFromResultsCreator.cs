using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IDataRepositoryFromResultsCreator
   {
      /// <summary>
      ///    Returns a new <see cref="DataRepository" /> containing the results of the <paramref name="simulation" />
      /// </summary>
      /// <param name="simulation">Simulation whose results should be converted to <see cref="DataRepository" /></param>
      DataRepository CreateResultsFor(IndividualSimulation simulation);

      /// <summary>
      ///    Adjusts the Internal flag for the columns defined in <paramref name="dataRepository" /> based on the selected output
      ///    values in <paramref name="simulation" />
      /// </summary>
      void UpdateColumnInternalUse(IndividualSimulation simulation, DataRepository dataRepository = null);
   }

   public class DataRepositoryFromResultsCreator : IDataRepositoryFromResultsCreator
   {
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDataRepositoryTask _dataRepositoryTask;

      public DataRepositoryFromResultsCreator(
         IDimensionRepository dimensionRepository,
         IObjectPathFactory objectPathFactory,
         IDataRepositoryTask dataRepositoryTask)
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
            .Where(col => col != null).Each(dataRepository.Add);

         UpdateColumnInternalUse(simulation, dataRepository);
         return dataRepository;
      }

      public void UpdateColumnInternalUse(IndividualSimulation simulation, DataRepository dataRepository = null)
      {
         var dataRepositoryToUse = dataRepository ?? simulation.DataRepository;
         if (dataRepositoryToUse == null)
            return;

         var outputSelections = simulation.OutputSelections;
         foreach (var column in dataRepositoryToUse.AllButBaseGrid())
         {
            column.IsInternal = !columnIsSelected(column, outputSelections);
         }
      }

      private bool columnIsSelected(DataColumn column, OutputSelections outputSelections)
      {
         //skip the first entry that corresponds to the simulation name
         var columnPath = column.QuantityInfo.Path.Skip(1).ToPathString();
         return outputSelections.Any(x => string.Equals(x.Path, columnPath));
      }

      /// <summary>
      ///    Returns the quantity and the consolidated object path of that quantity in the simulation (adding the simulation name
      ///    at the beginning)
      /// </summary>
      /// <param name="simulation"></param>
      /// <param name="path"></param>
      /// <returns></returns>
      private Tuple<IQuantity, ObjectPath> quantityAndPathFrom(Simulation simulation, IReadOnlyList<string> path)
      {
         var objectPath = _objectPathFactory.CreateObjectPathFrom(path).AndAddAtFront(simulation.Name);
         var quantity = objectPath.TryResolve<IQuantity>(simulation.Model.Root);
         return new Tuple<IQuantity, ObjectPath>(quantity, objectPath);
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
      private DataColumn createDataColumn(string columnId, string columnName, IEnumerable<string> path, IQuantity quantity,
         IDimension dimension, BaseGrid timeColumn, Simulation simulation)
      {
         var newColumn = new DataColumn(columnId, columnName, dimension, timeColumn)
         {
            QuantityInfo = new QuantityInfo(path, quantity.QuantityType),
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
            QuantityInfo = new QuantityInfo(new[] {Constants.TIME}, QuantityType.Time),
            Values = time.Values
         };
      }

      private string dataColumnIdFrom(QuantityValues quantityValues)
      {
         return quantityValues.ColumnId ?? ShortGuid.NewGuid();
      }
   }
}