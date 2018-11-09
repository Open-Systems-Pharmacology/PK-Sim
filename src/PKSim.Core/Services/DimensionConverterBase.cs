using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;

namespace PKSim.Core.Services
{
   public abstract class DimensionConverterBase : IDimensionConverterFor
   {
      private readonly IDimension _sourceDimension;
      private readonly IDimension _targetDimension;

      protected DimensionConverterBase(IDimension sourceDimension, IDimension targetDimension)
      {
         _sourceDimension = sourceDimension;
         _targetDimension = targetDimension;
      }

      public abstract bool CanResolveParameters();

      public abstract double ConvertToTargetBaseUnit(double sourceBaseUnitValue);

      public abstract double ConvertToSourceBaseUnit(double targetBaseUnitValue);

      public virtual bool CanConvertTo(IDimension targetDimension) => _targetDimension == targetDimension;

      public virtual bool CanConvertFrom(IDimension sourceDimension) => _sourceDimension == sourceDimension;

      public virtual string UnableToResolveParametersMessage => PKSimConstants.Error.MolWeightNotAvailable;

      protected double ConvertToMass(double molar) => molar * MolWeight;

      public double ConvertToMolar(double mass) => mass / MolWeight;

      protected abstract double MolWeight { get; }
   }
}