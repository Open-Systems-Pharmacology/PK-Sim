using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public class MolarToMassAmoutDimensionConverter : ColumnWithMolWeightDimensionConverter
   {
      public MolarToMassAmoutDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository) :
         base(column, dimensionRepository.Amount, dimensionRepository.Mass)
      {
      }

      public override double ConvertToTargetBaseUnit(double molarAmout)
      {
         return ConvertToMass(molarAmout);
      }

      public override double ConvertToSourceBaseUnit(double massAmount)
      {
         return ConvertToMolar(massAmount);
      }
   }
}