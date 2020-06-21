using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public abstract class MolarToMassColumnDimensionConverter : ConcentrationDimensionConverter
   {
      protected MolarToMassColumnDimensionConverter(DataColumn column, IDimension molarDimension, IDimension massDimension) : base(column, molarDimension, massDimension)
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

   /// <summary>
   ///    Converter should convert a value into the concentration
   /// </summary>
   public class MolarToMassConcentrationDimensionConverter : MolarToMassColumnDimensionConverter
   {
      public MolarToMassConcentrationDimensionConverter(DataColumn column, IDimensionRepository dimensionRepository) :
         base(column, dimensionRepository.MolarConcentration, dimensionRepository.MassConcentration)
      {
      }
   }
}