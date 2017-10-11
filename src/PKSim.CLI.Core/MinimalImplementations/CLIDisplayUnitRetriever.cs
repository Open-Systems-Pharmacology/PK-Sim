using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDisplayUnitRetriever : IDisplayUnitRetriever
   {
      public Unit PreferredUnitFor(IWithDimension withDimension, Unit defaultUnit = null)
      {
         return PreferredUnitFor(withDimension.Dimension);
      }

      public Unit PreferredUnitFor(IWithDisplayUnit withDisplayUnit)
      {
         return PreferredUnitFor(withDisplayUnit.Dimension);
      }

      public Unit PreferredUnitFor(IDimension dimension)
      {
         return dimension.DefaultUnit;
      }
   }
}