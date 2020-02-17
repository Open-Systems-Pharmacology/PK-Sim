using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Chart
{
   
   public interface ICurveDataSettings : IWithId
   {
      Color Color { get; set; }
      Symbols Symbol { get; set; }
      string Caption { get; set; }
      LineStyles LineStyle { get; set; }
      string PaneCaption { get; }
   }

   // for RangeArea, IndividualCurves: TX = double , XValues is reference to time grid
   // for Scatter: TX = double , XValues is specific for each Curve
   // for BoxWhisker: TX = string[], TX could be reference to the same (complete) List of grouping value tuples, empty y-values are expressed by NaN
   //                 alternatively TX could be specific for each curve, this would require an chart internal creation of all used x-Values within one pane/chart 
   //                 and using string as X-Axis DataType or mapping to (numeric) index of x-Values

   // for RangeArea: TY = double[3] for median+/-stdDev or double[N] for quantile
   // for IndividualCurves: TY = double[NIndividuals] (all individual curves <-> Curve)  or   double (each individual curve <-> Curve)
   // for Scatter: TY = double 
   // for BoxWhisker: TY = double[5] for lowerWhisker, lowerQuantile, median, upperQuantile, upperWhisker
   public class CurveData<TX, TY> : ICurveDataSettings
      where TX : IXValue
      where TY : IYValue
   {
      public virtual string Id { get; set; }
      /// <summary>
      /// Dictionary containing all fields {Name, Value} that where used to create this curve based on the population data
      /// </summary>
      public virtual IReadOnlyDictionary<string, string> FieldKeyValues { get; }
      public virtual PaneData Pane { get; set; }
      public virtual Color Color { get; set; }
      public virtual LineStyles LineStyle { get; set; }
      public virtual Symbols Symbol { get; set; }
      public virtual string Caption { get; set; }
      private readonly List<TX> _xValues;
      private readonly List<TY> _yValues;
      private readonly List<string> _fieldValues;
      private readonly List<XYValue> _xyValues;

      /// <summary>
      /// Path of underlying quantity being displayed. This is only set if the field is an IQuantityField
      /// </summary>
      public virtual string QuantityPath { get; set; }

      /// <summary>
      /// Dimension in which the y values are stored. 
      /// </summary>
      public virtual IDimension YDimension { get; set; }

      /// <summary>
      ///    Convenience constructor especially useful for tests
      /// </summary>
      public CurveData() : this(new Dictionary<string, string>())
      {
      }

      public CurveData(IReadOnlyDictionary<string, string> fieldKeyValues)
      {
         Id = ShortGuid.NewGuid();
         FieldKeyValues = fieldKeyValues;
         Color = Color.Black;
         Symbol = Symbols.Circle;
         LineStyle = LineStyles.Solid;
         _xValues = new List<TX>();
         _yValues = new List<TY>();
         _xyValues = new List<XYValue>();
         _fieldValues = fieldKeyValues.Values.ToList();
      }

      public IEnumerable<string> FieldNames => FieldKeyValues.Keys;

      public IReadOnlyList<string> FieldValues => _fieldValues;

      public virtual void Add(TX xValue, TY yValue)
      {
         _xValues.Add(xValue);
         _yValues.Add(yValue);
         _xyValues.Add(new XYValue(_xValues.Count - 1, xValue, yValue));
      }

      public virtual IReadOnlyList<TX> XValues => _xValues;

      public virtual IReadOnlyList<TY> YValues => _yValues;

      public virtual string PaneCaption => Pane.Caption;

      public virtual int GetPointIndexForDisplayValues(double xValueInDisplayUnit, double yValueInDisplayUnit)
      {
         var xBaseUnit = XAxis.ConvertToBaseUnit(xValueInDisplayUnit);
         var yBaseUnit = YAxis.ConvertToBaseUnit(yValueInDisplayUnit);

         return _xyValues.OrderBy(x => x.DistanceTo(xBaseUnit, yBaseUnit)).First().Index;
      }

      public virtual AxisData XAxis => Pane.ChartAxis;

      public virtual AxisData YAxis => Pane.Axis;

      public virtual string XDisplayValueAt(int pointIndex)
      {
         return XValues[pointIndex].ToString(XAxis);
      }

      public virtual string YDisplayValueAt(int pointIndex)
      {
         return YValues[pointIndex].ToString(YAxis, YDimension);
      }

      private class XYValue
      {
         private readonly IXValue _xValue;
         private readonly IYValue _yValue;
         public int Index { get; }

         public XYValue(int index, IXValue xValue, IYValue yValue)
         {
            Index = index;
            _xValue = xValue;
            _yValue = yValue;
         }

         private double calcDistance(double baseUnitValue, double value)
         {
            return Math.Abs(baseUnitValue - value);
         }

         public double DistanceTo(double xBaseUnit, double yBaseUnit)
         {
            var distanceToX = calcDistance(xBaseUnit, _xValue.X);
            if (shouldCalculateDistanceOnlyBasedOnXValue(yBaseUnit))
               return distanceToX;

            return distanceToX + calcDistance(yBaseUnit, _yValue.Y);
         }

         private bool shouldCalculateDistanceOnlyBasedOnXValue(double yBaseUnit)
         {
            return double.IsNaN(_yValue.Y) || double.IsNaN(yBaseUnit) || _xValue.IsAnImplementationOf<BoxWhiskerXValue>();
         }
      }
   }
}