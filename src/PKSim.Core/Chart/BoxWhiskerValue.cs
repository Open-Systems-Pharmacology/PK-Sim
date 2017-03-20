using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class BoxWhiskerYValue : IYValue
   {
      public float LowerWhisker { get; set; }
      public float LowerBox { get; set; }
      public float Median { get; set; }
      public float UpperBox { get; set; }
      public float UpperWhisker { get; set; }
      public float[] Outliers { get; set; }

      //use lower whisker because this is the first value of the box
      public float Y
      {
         get { return LowerWhisker; }
      }

      public bool IsValid
      {
         get { return LowerWhisker.IsValid() && LowerBox.IsValid() && Median.IsValid() && UpperBox.IsValid() && UpperWhisker.IsValid(); }
      }

      public string ToString(IWithDisplayUnit unitConverter)
      {
         var outliers = Outliers.Select(o => unitConverter.DisplayValue(o)).ToArray();

         return PKSimConstants.Information.BoxWhiskerYAsTooltip(
            unitConverter.DisplayValue(LowerWhisker),
            unitConverter.DisplayValue(LowerBox),
            unitConverter.DisplayValue(Median),
            unitConverter.DisplayValue(UpperBox),
            unitConverter.DisplayValue(UpperWhisker),
            outliers);
      }

      public void ClearOutliers()
      {
         Outliers = new float[]{};
      }
   }

   public class BoxWhiskerXValue : IXValue, IReadOnlyList<string>
   {
      public float X { get; set; }
      private readonly List<string> _values;

      public BoxWhiskerXValue(IEnumerable<string> values)
      {
         _values = values.ToList();
         X = float.NaN;
      }

      public IEnumerator<string> GetEnumerator()
      {
         return _values.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public int Count
      {
         get { return _values.Count; }
      }

      public string this[int index]
      {
         get { return _values[index]; }
      }

      public string ToString(IWithDisplayUnit unitConverter)
      {
         return _values.ToString(Constants.DISPLAY_PATH_SEPARATOR);
      }

      public int CompareTo(IXValue valueToCompareTo)
      {
         return X.CompareTo(valueToCompareTo.X);
      }
   }
}