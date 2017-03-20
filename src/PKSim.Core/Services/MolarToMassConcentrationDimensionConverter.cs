using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Converter should convert a value into the concentration
   /// </summary>
   public class MolarToMassConcentrationDimensionConverter : ConcentrationDimensionConverter
   {
      public MolarToMassConcentrationDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository) :
         base(column, dimensionRepository.MolarConcentration, dimensionRepository.MassConcentration)
      {
      }

      public override double ConvertToTargetBaseUnit(double molarConcentration)
      {
         return ConvertToMass(molarConcentration);
      }

      public override double ConvertToSourceBaseUnit(double massConcentration)
      {
         return ConvertToMolar(massConcentration);
      }
   }
}