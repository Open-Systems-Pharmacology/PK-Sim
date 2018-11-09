using OSPSuite.Core.Domain.Data;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public class MassToAmoundDimensionConverter : ColumnWithMolWeightDimensionConverter
   {
      public MassToAmoundDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository) :
         base(column, dimensionRepository.Mass, dimensionRepository.Amount)
      {
      }

      public override double ConvertToTargetBaseUnit(double massAmount) => ConvertToMolar(massAmount);

      public override double ConvertToSourceBaseUnit(double molarAmount) => ConvertToMass(molarAmount);
   }
}
