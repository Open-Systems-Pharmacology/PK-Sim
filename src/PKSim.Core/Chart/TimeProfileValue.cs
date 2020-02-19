using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class TimeProfileXValue : XValue
   {
      public TimeProfileXValue(float value) : base(value)
      {
      }
   }

   public interface ITimeProfileYValue : IYValue
   {
      float LowerValue { get; }

      float UpperValue { get; }
   }

   public class TimeProfileYValue : ITimeProfileYValue
   {
      public float Y { get; set; }
      public float LowerValue { get; set; }
      public float UpperValue { get; set; }

      public TimeProfileYValue()
      {
         Y = float.NaN;
         LowerValue = float.NaN;
         UpperValue = float.NaN;
      }

      public bool IsValid => Y.IsValid() || IsRange;

      public bool IsRange => LowerValue.IsValid() && UpperValue.IsValid();

      public string ToString(IWithDisplayUnit objectWithTargetUnit, IDimension valueDimension)
      {
         if (IsRange)
            return PKSimConstants.Information.TimeProfileYAsTooltip(
               objectWithTargetUnit.DisplayValueWithUnit(LowerValue, valueDimension),
               objectWithTargetUnit.DisplayValueWithUnit(UpperValue, valueDimension));

         return PKSimConstants.Information.ScatterYAsTooltip(objectWithTargetUnit.DisplayValueWithUnit(Y, valueDimension));
      }
   }
}