using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    The service is used to update the data repository of a simulation after the calcultion was performed.
   ///    Columns that already exists would be updated, new columns will be added, and other columns will be removed
   /// </summary>
   public interface ISimulationResultsSynchronizer
   {
      /// <summary>
      ///    Synchronize the data in the simulation with the newly calculated data
      /// </summary>
      /// <param name="simulation">simulation containing the data </param>
      /// <param name="newResults">new results obtained after the simulation run</param>
      void Synchronize(IndividualSimulation simulation, DataRepository newResults);

      /// <summary>
      ///    Synchronise the data in the population simulation as well as all underlying results saved in the charts
      /// </summary>
      /// <param name="populationSimulation">population Simulation</param>
      /// <param name="newResults">new results of simulation</param>
      void Synchronize(PopulationSimulation populationSimulation, SimulationResults newResults);

   }

   public class SimulationResultsSynchronizer : ISimulationResultsSynchronizer
   {
      private readonly ISimulationResultsCreator _simulationResultsCreator;
      private readonly IDisplayUnitUpdater _displayUnitUpdater;
      private readonly IDataRepositoryFromResultsCreator _dataRepositoryFromResultsCreator;
      private readonly IPKAnalysesTask _pkAnalysesTask;

      public SimulationResultsSynchronizer(
         ISimulationResultsCreator simulationResultsCreator,
         IDisplayUnitUpdater displayUnitUpdater,
         IDataRepositoryFromResultsCreator dataRepositoryFromResultsCreator,
         IPKAnalysesTask pkAnalysesTask)
      {
         _simulationResultsCreator = simulationResultsCreator;
         _displayUnitUpdater = displayUnitUpdater;
         _dataRepositoryFromResultsCreator = dataRepositoryFromResultsCreator;
         _pkAnalysesTask = pkAnalysesTask;
      }

      public void Synchronize(IndividualSimulation simulation, DataRepository newResults)
      {
         if (!simulation.HasResults || newResults == null)
            simulation.DataRepository = newResults;
         else
            simulation.DataRepository = synchronize(simulation.DataRepository, newResults);

         _dataRepositoryFromResultsCreator.UpdateColumnInternalUse(simulation);

         //once the data repository was updated, we need to update the underlying results as well
         _displayUnitUpdater.UpdateDisplayUnitsIn(simulation.DataRepository);

         simulation.Results = _simulationResultsCreator.CreateResultsFrom(simulation.DataRepository);
         updateSequence(simulation.DataRepository);
      }


      public void Synchronize(PopulationSimulation populationSimulation, SimulationResults newResults)
      {
         populationSimulation.Results = newResults;
         populationSimulation.PKAnalyses = _pkAnalysesTask.CalculateFor(populationSimulation);
      }

      private void updateSequence(DataRepository results)
      {
         if (results == null) return;
         int sequence = 0;
         foreach (var column in results.AllButBaseGrid())
         {
            column.QuantityInfo.OrderIndex = sequence++;
         }
      }

      private DataRepository synchronize(DataRepository oldResultsToUpdate, DataRepository newResults)
      {
         var newResultsCache = new Cache<QuantityInfo, DataColumn>(col => col.QuantityInfo);
         newResultsCache.AddRange(newResults);

         //remove column that do not exist in the new results
         removeDeletedColumns(oldResultsToUpdate, newResultsCache);

         //first update base grid columns
         var baseGridColumns = oldResultsToUpdate.Where(col => col.IsBaseGrid());
         baseGridColumns.Each(baseGrid => updateColumnValues(baseGrid, newResultsCache));

         //then update non base grid columns
         var columns = oldResultsToUpdate.AllButBaseGrid();
         columns.Each(col => updateColumnValues(col, newResultsCache));

         //finally for all new columns, create a copy with the accurate base grid reference
         var oldResultsCache = new Cache<QuantityInfo, DataColumn>(col => col.QuantityInfo);
         oldResultsCache.AddRange(oldResultsToUpdate);

         var newColumns = newResults.Where(col => !oldResultsCache.Contains(col.QuantityInfo));
         newColumns.Each(col => oldResultsToUpdate.Add(cloneColumn(oldResultsCache, col)));

         return oldResultsToUpdate;
      }

      private void removeDeletedColumns(DataRepository oldResults, ICache<QuantityInfo, DataColumn> newResults)
      {
         var deletedColumns = oldResults.Where(col => !newResults.Contains(col.QuantityInfo)).ToList();
         deletedColumns.Each(oldResults.Remove);
      }

      private DataColumn cloneColumn(ICache<QuantityInfo, DataColumn> oldResultsCache, DataColumn columnToAdd)
      {
         var baseGrid = oldResultsCache[columnToAdd.BaseGrid.QuantityInfo].DowncastTo<BaseGrid>();
         return new DataColumn(columnToAdd.Id, columnToAdd.Name, columnToAdd.Dimension, baseGrid)
         {
            QuantityInfo = columnToAdd.QuantityInfo,
            DataInfo = columnToAdd.DataInfo,
            Values = columnToAdd.InternalValues
         };
      }

      private void updateColumnValues(DataColumn columnToUpdate, ICache<QuantityInfo, DataColumn> newResults)
      {
         var newColumn = newResults[columnToUpdate.QuantityInfo];
         columnToUpdate.Values = newColumn.InternalValues;
         columnToUpdate.DataInfo.MolWeight = newColumn.DataInfo.MolWeight;
         columnToUpdate.Name = newColumn.Name;
      }
   }
}