using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class ValueWithIndividualId
   {
      public float Value { get; set; }
      public int IndividualId { get; set; }

      public ValueWithIndividualId(float value)
      {
         Value = value;
      }

      public bool IsValid => Value.IsValid();

      public static implicit operator float(ValueWithIndividualId valueWithIndividualId)
      {
         return valueWithIndividualId.Value;
      }
   }

   public class BoxWhiskerYValue : IYValue
   {
      public ValueWithIndividualId LowerWhisker { get; set; }
      public ValueWithIndividualId LowerBox { get; set; }
      public ValueWithIndividualId Median { get; set; }
      public ValueWithIndividualId UpperBox { get; set; }
      public ValueWithIndividualId UpperWhisker { get; set; }
      public ValueWithIndividualId[] Outliers { get; set; }

   
      //use lower whisker because this is the first value of the box
      public float Y => LowerWhisker.Value;

      public bool IsValid => LowerWhisker.IsValid && LowerBox.IsValid && Median.IsValid && UpperBox.IsValid && UpperWhisker.IsValid;

      public string ToString(IWithDisplayUnit objectWithTargetUnit, IDimension valueDimension)
      {
         var outlierValues = Outliers.Select(o => objectWithTargetUnit.DisplayValueWithUnit(o.Value, valueDimension)).ToArray();
         var outlierIndividualIds = Outliers.Select(o => o.IndividualId).ToArray();

         return PKSimConstants.Information.BoxWhiskerYAsTooltip(
            objectWithTargetUnit.DisplayValueWithUnit(LowerWhisker.Value, valueDimension),
            LowerWhisker.IndividualId,
            objectWithTargetUnit.DisplayValueWithUnit(LowerBox.Value, valueDimension),
            LowerBox.IndividualId,
            objectWithTargetUnit.DisplayValueWithUnit(Median.Value, valueDimension),
            Median.IndividualId,
            objectWithTargetUnit.DisplayValueWithUnit(UpperBox.Value, valueDimension),
            UpperBox.IndividualId,
            objectWithTargetUnit.DisplayValueWithUnit(UpperWhisker.Value, valueDimension),
            UpperWhisker.IndividualId,
            outlierValues,
            outlierIndividualIds);
      }

      public IEnumerable<ValueWithIndividualId> AllValues
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
         Outliers = new ValueWithIndividualId[] { };
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