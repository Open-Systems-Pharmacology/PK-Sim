using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public abstract class MolarToMassFieldDimensionConverter : FieldDimensionConverter
   {
      protected MolarToMassFieldDimensionConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimension molarDimension, IDimension massDimension) :
         base(quantityField,populationDataCollector, molarDimension, massDimension)
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

   public class MolarToMassConcentrationDimensionForFieldConverter : MolarToMassFieldDimensionConverter
   {
      public MolarToMassConcentrationDimensionForFieldConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimensionRepository dimensionRepository) :
         base(quantityField, populationDataCollector, dimensionRepository.MolarConcentration, dimensionRepository.MassConcentration)
      {
      }
   }

   public class MolarToMassAmountDimensionForFieldConverter : MolarToMassFieldDimensionConverter
   {
      public MolarToMassAmountDimensionForFieldConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimensionRepository dimensionRepository) :
         base(quantityField, populationDataCollector, dimensionRepository.Amount, dimensionRepository.Mass)
      {
      }
   }

   public class AucMolarToAucMassDimensionForFieldConverter : MolarToMassFieldDimensionConverter
   {
      public AucMolarToAucMassDimensionForFieldConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimensionRepository dimensionRepository) :
         base(quantityField, populationDataCollector, dimensionRepository.AucMolar, dimensionRepository.Auc)
      {
      }
   }

}