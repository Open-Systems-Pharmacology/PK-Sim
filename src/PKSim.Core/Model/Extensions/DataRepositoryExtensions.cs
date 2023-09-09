using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model.Extensions
{
   public static class DataRepositoryExtensions
   {
      /// <summary>
      ///    tries to find peripheral venous blod plasma if defined in the repository. returns null otherwise
      /// </summary>
      public static DataColumn PeripheralVenousBloodColumn(this DataRepository dataRepository, string compoundName)
      {
         return dataRepository.drugColumnFor(CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD, compoundName);
      }

      /// <summary>
      ///    tries to find venous blood plasma if defined in the repository. returns null otherwise
      /// </summary>
      public static DataColumn VenousBloodColumn(this DataRepository dataRepository,string compoundName)
      {
         return dataRepository.drugColumnFor(CoreConstants.Organ.VENOUS_BLOOD, CoreConstants.Compartment.PLASMA, CoreConstants.Observer.CONCENTRATION_IN_CONTAINER, compoundName);
      }

      private static DataColumn drugColumnFor(this IEnumerable<DataColumn> dataRepository, string organ, string compartment, string columnName, string compoundName)
      {
         return columnsFor(dataRepository, organ, compartment, columnName, QuantityType.Drug)
            .FirstOrDefault(col => col.QuantityInfo.Path.Contains(compoundName));
      }

      private static IEnumerable<DataColumn> columnsFor(this IEnumerable<DataColumn> dataRepository, string organ, string compartment, string columnName, QuantityType quantityType)
      {
         return dataRepository.Where(col => col.DataInfo.Origin == ColumnOrigins.Calculation)
            .Where(col => col.QuantityInfo.Type.Is(quantityType))
            .Where(col => col.QuantityInfo.Path.Contains(organ))
            .Where(col => col.QuantityInfo.Path.Contains(compartment))
            .Where(col => string.Equals(col.Name, columnName));
      }

      public static DataColumn FabsOral(this DataRepository dataRepository,string compoundName)
      {
         return dataRepository.drugColumnFor(Constants.Organ.LUMEN, CoreConstants.Observer.FABS_ORAL, CoreConstants.Observer.FABS_ORAL, compoundName);
      }

      public static IEnumerable<DataColumn> BelongingTo(this IEnumerable<DataColumn> dataColumns, Simulation simulation)
      {
         return dataColumns.Where(c => c.BelongsTo(simulation));
      }

      public static string MoleculeName(this DataColumn column)
      {
         return column.QuantityInfo.Path.ToList().MoleculeName();
      }
   }
}