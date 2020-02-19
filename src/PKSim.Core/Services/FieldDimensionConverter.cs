using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public abstract class FieldDimensionConverter : DimensionConverterBase
   {
      private readonly IQuantityField _quantityField;
      private readonly IPopulationDataCollector _populationDataCollector;
      private bool _resolved;
      private double? _molWeight;

      protected FieldDimensionConverter(IQuantityField quantityField, IPopulationDataCollector populationDataCollector, IDimension sourceDimension, IDimension targetDimension)
         : base(sourceDimension, targetDimension)
      {
         _quantityField = quantityField;
         _populationDataCollector = populationDataCollector;
      }

      public override bool CanResolveParameters()
      {
         if (!_resolved)
         {
            _molWeight = _populationDataCollector?.MolWeightFor(_quantityField.QuantityPath);
            _resolved = true;
         }

         return _molWeight != null;
      }

      public override string UnableToResolveParametersMessage
      {
         get
         {
            if (!_populationDataCollector.IsAnImplementationOf<PopulationSimulationComparison>())
               return base.UnableToResolveParametersMessage;

            return PKSimConstants.Error.MolWeightNotAvailableForPopulationSimulationComparison;
         }
      }

      protected override double MolWeight => _molWeight.GetValueOrDefault(double.NaN);
   }
}