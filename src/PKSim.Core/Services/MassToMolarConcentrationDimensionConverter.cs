using OSPSuite.Core.Domain.Data;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public class MassToMolarConcentrationDimensionConverter : ConcentrationDimensionConverter
   {
      public MassToMolarConcentrationDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository)
         : base(column, dimensionRepository.MassConcentration, dimensionRepository.MolarConcentration)
      {
      }

      public override double ConvertToTargetBaseUnit(double massConcentration) => ConvertToMolar(massConcentration);

      public override double ConvertToSourceBaseUnit(double molarConcentration) => ConvertToMass(molarConcentration);
   }
}