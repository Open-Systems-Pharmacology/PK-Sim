using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

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
      private readonly IPKAnalysesTask _populationPKAnalysesTask;
      private readonly ISimulationResultsCreator _simulationResultsCreator;
      private readonly IDisplayUnitUpdater _displayUnitUpdater;

      public SimulationResultsSynchronizer(IPKAnalysesTask populationPKAnalysesTask, ISimulationResultsCreator simulationResultsCreator, IDisplayUnitUpdater displayUnitUpdater)
      {
         _populationPKAnalysesTask = populationPKAnalysesTask;
         _simulationResultsCreator = simulationResultsCreator;
         _displayUnitUpdater = displayUnitUpdater;
      }

      public void Synchronize(IndividualSimulation simulation, DataRepository newResults)
      {
         if (!simulation.HasResults || newResults == null)
            simulation.DataRepository = newResults;
         else
            simulation.DataRepository = synchronize(simulation.DataRepository, newResults);

         updateColumnInternalUse(simulation);

         //once the data repository was updated, we need to update the underlying results as well
         _displayUnitUpdater.UpdateDisplayUnitsIn(simulation.DataRepository);

         simulation.Results = _simulationResultsCreator.CreateResultsFrom(simulation.DataRepository);
         updateSequence(simulation.DataRepository);
      }

      private void updateColumnInternalUse(IndividualSimulation simulation)
      {
         var outputSelections = simulation.OutputSelections;
         foreach (var column in simulation.DataRepository.AllButBaseGrid())
         {
            column.IsInternal = !columnIsSelected(simulation.Name, column, outputSelections);
         }
      }

      private bool columnIsSelected(string simulationName, DataColumn column, OutputSelections outputSelections)
      {
         //skip the first entry that corresponds to the simulation name
         var columnPath = column.QuantityInfo.Path.Skip(1).ToPathString();
         return outputSelections.Any(x => string.Equals(x.Path, columnPath));
      }

      public void Synchronize(PopulationSimulation populationSimulation, SimulationResults newResults)
      {
         populationSimulation.Results = newResults;
         populationSimulation.PKAnalyses = _populationPKAnalysesTask.CalculateFor(populationSimulation);
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

         //then update non base grid columnds
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
         var clone = new DataColumn(columnToAdd.Id, columnToAdd.Name, columnToAdd.Dimension, baseGrid);
         clone.QuantityInfo = columnToAdd.QuantityInfo;
         clone.DataInfo = columnToAdd.DataInfo;
         clone.Values = columnToAdd.InternalValues;
         return clone;
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