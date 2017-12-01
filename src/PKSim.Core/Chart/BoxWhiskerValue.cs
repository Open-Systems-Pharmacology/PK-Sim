using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class ValueWithIndvividualId
   {
      public float Value { get; set; }
      public int IndividualId { get; set; }

      public ValueWithIndvividualId(float value)
      {
         Value = value;
      }

      public bool IsValid => Value.IsValid();

      public static implicit operator float(ValueWithIndvividualId valueWithIndvividualId)
      {
         return valueWithIndvividualId.Value;
      }
   }

   public class BoxWhiskerYValue : IYValue
   {
      public ValueWithIndvividualId LowerWhisker { get; set; }
      public ValueWithIndvividualId LowerBox { get; set; }
      public ValueWithIndvividualId Median { get; set; }
      public ValueWithIndvividualId UpperBox { get; set; }
      public ValueWithIndvividualId UpperWhisker { get; set; }
      public ValueWithIndvividualId[] Outliers { get; set; }

      //use lower whisker because this is the first value of the box
      public float Y => LowerWhisker.Value;

      public bool IsValid => LowerWhisker.IsValid && LowerBox.IsValid && Median.IsValid && UpperBox.IsValid && UpperWhisker.IsValid;

      public string ToString(IWithDisplayUnit unitConverter)
      {
         var outlierValues = Outliers.Select(o => unitConverter.DisplayValue(o.Value)).ToArray();
         var outlierIndividualIds = Outliers.Select(o => o.IndividualId).ToArray();

         return PKSimConstants.Information.BoxWhiskerYAsTooltip(
            unitConverter.DisplayValue(LowerWhisker.Value),
            LowerWhisker.IndividualId,
            unitConverter.DisplayValue(LowerBox.Value),
            LowerBox.IndividualId,
            unitConverter.DisplayValue(Median.Value),
            Median.IndividualId,
            unitConverter.DisplayValue(UpperBox.Value),
            UpperBox.IndividualId,
            unitConverter.DisplayValue(UpperWhisker.Value),
            UpperWhisker.IndividualId,
            outlierValues,
            outlierIndividualIds);
      }

      public IEnumerable<ValueWithIndvividualId> AllValues
      {
         get
         {
            yield return LowerWhisker;
            yield return LowerBox;
            yield return Median;
            yield return UpperBox;
            yield return UpperWhisker;

            if (Outliers == null)
               yield break;

            foreach (var outlier in Outliers)
            {
               yield return outlier;
            }
         }
      }

      public void ClearOutliers()
      {
         Outliers = new ValueWithIndvividualId[] { };
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

      public int Count => _values.Count;

      public string this[int index] => _values[index];

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