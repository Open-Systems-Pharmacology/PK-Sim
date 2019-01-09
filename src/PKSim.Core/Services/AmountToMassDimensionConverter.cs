using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public class AmountToMassDimensionConverter : ColumnWithMolWeightDimensionConverter
   {
      public AmountToMassDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository) :
         base(column, dimensionRepository.Amount, dimensionRepository.Mass)
      {
      }

      public override double ConvertToTargetBaseUnit(double molarAmout) => ConvertToMass(molarAmout);

      public override double ConvertToSourceBaseUnit(double massAmount) => ConvertToMolar(massAmount);
   }
}