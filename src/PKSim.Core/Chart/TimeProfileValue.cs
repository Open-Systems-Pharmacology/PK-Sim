using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

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

      public bool IsValid
      {
         get
         {
            if (Y.IsValid())
               return true;

            return IsRange;
         }
      }

      public bool IsRange
      {
         get { return LowerValue.IsValid() && UpperValue.IsValid(); }
      }

      public string ToString(IWithDisplayUnit unitConverter)
      {
         if (IsRange)
            return PKSimConstants.Information.TimeProfileYAsTooltip(unitConverter.DisplayValue(LowerValue), unitConverter.DisplayValue(UpperValue));

         return PKSimConstants.Information.ScatterYAsTooltip(unitConverter.DisplayValue(Y));
      }
   }
}