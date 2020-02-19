using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class ScatterXValue : XValue
   {
      public ScatterXValue(float value) : base(value)
      {
      }
   }

   public class ScatterYValue : IYValue
   {
      public float Value { get; }

      public ScatterYValue(float value)
      {
         Value = value;
      }

      public bool IsValid => Value.IsValid();

      public string ToString(IWithDisplayUnit objectWithTargetUnit, IDimension valueDimension)
      {
         return PKSimConstants.Information.ScatterYAsTooltip(objectWithTargetUnit.DisplayValueWithUnit(Value, valueDimension));
      }

      public float Y => Value;
   }
}