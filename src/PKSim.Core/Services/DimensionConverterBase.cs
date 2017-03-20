using PKSim.Assets;
using OSPSuite.Core.Domain.UnitSystem;

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

      public virtual bool CanConvertTo(IDimension targetDimension)
      {
         return _targetDimension == targetDimension;
      }

      public virtual bool CanConvertFrom(IDimension sourceDimension)
      {
         return _sourceDimension == sourceDimension;
      }

      public virtual string UnableToResolveParametersMessage
      {
         get { return PKSimConstants.Error.MolWeightNotAvailable; }
      }

      protected double ConvertToMass(double molar)
      {
         return molar * MolWeight;
      }

      public double ConvertToMolar(double mass)
      {
         return mass / MolWeight;
      }

      protected abstract double MolWeight { get; }
   }
}