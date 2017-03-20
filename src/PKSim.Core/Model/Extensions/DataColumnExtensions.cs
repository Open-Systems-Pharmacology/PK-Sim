using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Model.Extensions
{
   public static class DataColumnExtensions
   {
      public static Simulation ParentSimulationIn(this DataColumn dataColumn, IEnumerable<Simulation> simulations)
      {
         return simulations.FirstOrDefault(dataColumn.BelongsTo);
      }

      public static bool BelongsTo(this DataColumn dataColumn, Simulation simulation)
      {
         if (simulation == null || !simulation.HasResults)
            return false;

         if (dataColumn.DataInfo.Origin != ColumnOrigins.Calculation)
            return false;

         if (individualSimulationContainsColumn(dataColumn, simulation))
            return true;

         return simulation.IsAnImplementationOf<PopulationSimulation>();
      }

      private static bool individualSimulationContainsColumn(DataColumn dataColumn, Simulation simulation)
      {
         var individualSimulation = simulation as IndividualSimulation;
         return individualSimulation != null && Equals(dataColumn.Repository, individualSimulation.DataRepository);
      }
   }
}