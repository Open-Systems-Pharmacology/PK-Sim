using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public abstract class MassToMolarFieldDimensionConverter : FieldDimensionConverter
   {
      protected MassToMolarFieldDimensionConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimension sourceDimension, IDimension targetDimension) :
         base(quantityField, populationDataCollector, sourceDimension, targetDimension)
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

   public class MassToMolarConcentrationDimensionForFieldConverter : MassToMolarFieldDimensionConverter
   {
      public MassToMolarConcentrationDimensionForFieldConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimensionRepository dimensionRepository)
         : base(quantityField, populationDataCollector, dimensionRepository.MassConcentration, dimensionRepository.MolarConcentration)
      {
      }
   }
}