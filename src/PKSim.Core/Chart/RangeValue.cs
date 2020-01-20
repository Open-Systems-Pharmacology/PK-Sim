using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class RangeXValue : XValue
   {
      public float Minimum { get; set; }
      public float Maximum { get; set; }

      /// <summary>
      /// Number of elements defined in interval [Minimum, Maximum]
      /// </summary>
      public int NumberOfItems { get; set; }

      public RangeXValue(float value) : base(value)
      {
         Minimum = float.NaN;
         Maximum = float.NaN;
         NumberOfItems = 0;
      }

      public override string ToString(IWithDisplayUnit unitConverter)
      {
         return PKSimConstants.Information.RangeXAsTooltip(
            unitConverter.DisplayValue(Minimum),
            unitConverter.DisplayValue(X),
            unitConverter.DisplayValue(Maximum), 
            NumberOfItems);
      }
   }

   public class RangeYValue : IYValue
   {
      public float LowerPercentile { get; set; }
      public float UpperPercentile { get; set; }
      public float Median { get; set; }

      public bool IsValid => LowerPercentile.IsValid() && UpperPercentile.IsValid() && Median.IsValid();

      public string ToString(IWithDisplayUnit unitConverter)
      {
         return PKSimConstants.Information.RangeYAsTooltip(
            unitConverter.DisplayValue(LowerPercentile),
            unitConverter.DisplayValue(Median),
            unitConverter.DisplayValue(UpperPercentile));
      }

      public float[] Values => new[] {Median};

      public float Y => Median;
   }
}