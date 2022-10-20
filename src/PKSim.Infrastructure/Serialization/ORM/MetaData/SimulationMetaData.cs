using System.Collections.Generic;
using NHibernate;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   public enum SimulationMode
   {
      Individual,
      Population
   }

   public class SimulationMetaData : BuildingBlockMetaData
   {
      public virtual ICollection<UsedBuildingBlockMetaData> BuildingBlocks { get; set; }
      public virtual ICollection<string> UsedObservedData { get; set; }
      public virtual SimulationResults SimulationResults { get; set; }
      public virtual SimulationAnalysesMetaData SimulationAnalyses { get; set; }
      public virtual ICollection<SimulationChartMetaData> Charts { get; set; }

      public virtual SimulationMode SimulationMode { get; set; }

      //Legacy code
      public virtual string DataRepositoryId { get; set; }

      public SimulationMetaData()
      {
         BuildingBlocks = new HashSet<UsedBuildingBlockMetaData>();
         UsedObservedData = new HashSet<string>();
         Charts = new HashSet<SimulationChartMetaData>();
      }

      public override void UpdateFrom(BuildingBlockMetaData sourceChild, ISession session)
      {
         var sourceSimulation = sourceChild as SimulationMetaData;
         if (sourceSimulation == null) return;
         base.UpdateFrom(sourceChild, session);
         SimulationMode = sourceSimulation.SimulationMode;

         BuildingBlocks.UpdateFrom<string, UsedBuildingBlockMetaData>(sourceSimulation.BuildingBlocks, session);
         updateObservedDataFrom(sourceSimulation.UsedObservedData);


         //Update results and charts only if simulation is loaded
         if (!sourceChild.IsLoaded) return;

         updateResultsFrom(sourceSimulation.SimulationResults, session);
         updateChartsFrom(sourceSimulation.Charts, session);
         updateSimulationAnalysesFrom(sourceSimulation.SimulationAnalyses, session);
      }

      private void updateSimulationAnalysesFrom(SimulationAnalysesMetaData simulationAnalyses, ISession session)
      {
         if (simulationAnalyses == null)
            return;

         if (SimulationAnalyses == null)
            SimulationAnalyses = simulationAnalyses;
         else
            //Either update the reference or update. 
            SimulationAnalyses.UpdateFrom(simulationAnalyses, session);
      }

      private void updateResultsFrom(SimulationResults sourceResults, ISession session)
      {
         //not loaded
         if (sourceResults == null)
            return;

         if (SimulationResults != null)
            session.Delete(SimulationResults);

         SimulationResults = sourceResults;
      }

      private void updateObservedDataFrom(IEnumerable<string> usedObservedData)
      {
         UsedObservedData.Clear();
         usedObservedData.Each(UsedObservedData.Add);
      }

      private void updateChartsFrom(ICollection<SimulationChartMetaData> sourceSimulationCharts, ISession session)
      {
         Charts.UpdateFrom<string, SimulationChartMetaData>(sourceSimulationCharts, session);
      }

      public virtual void AddChart(SimulationChartMetaData chart)
      {
         Charts.Add(chart);
      }

      public virtual void AddBuildingBlock(UsedBuildingBlockMetaData blockMetaData)
      {
         BuildingBlocks.Add(blockMetaData);
      }

      public virtual void AddObservedData(string observedDataId)
      {
         UsedObservedData.Add(observedDataId);
      }
   }
}