using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public class MassToMolarConcentrationDimensionConverter : ConcentrationDimensionConverter
   {
      public MassToMolarConcentrationDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository)
         : base(column, dimensionRepository.MassConcentration, dimensionRepository.MolarConcentration)
      {
      }

      public override double ConvertToTargetBaseUnit(double massConcentration)
      {
         return ConvertToMolar(massConcentration);
      }

      public override double ConvertToSourceBaseUnit(double molarConcentration)
      {
         return ConvertToMass(molarConcentration);
      }
   }
}