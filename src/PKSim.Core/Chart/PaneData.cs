using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Chart
{
   public abstract class PaneData : IWithId
   {
      public virtual string Id { get; set; }
      private readonly List<ObservedCurveData> _observedCurveData;
      public virtual string Caption { get; set; }
      public virtual AxisData Axis { get; private set; }
      public abstract AxisData ChartAxis { get; }

      protected PaneData(AxisData axis)
      {
         _observedCurveData = new List<ObservedCurveData>();
         Axis = axis;
      }

      public IReadOnlyList<ObservedCurveData> VisibleObservedCurveData
      {
         get { return _observedCurveData.Where(c => c.Visible).ToList(); }
      }

      public virtual IReadOnlyList<ObservedCurveData> ObservedCurveData => _observedCurveData;

      public void AddObservedCurve(ObservedCurveData observedCurveData)
      {
         _observedCurveData.Add(observedCurveData);
         observedCurveData.Pane = this;
      }
   }

   public class PaneData<TX, TY> : PaneData, IComparer<CurveData<TX, TY>>
      where TX : IXValue
      where TY : IYValue
   {
      private readonly IReadOnlyList<IComparer<object>> _fieldValueComparers;
      private readonly Cache<string, CurveData<TX, TY>> _allCurves;
      private readonly List<string> _fieldValues;
      public virtual ChartData<TX, TY> Chart { get; set; }
      public virtual IReadOnlyDictionary<string, string> FieldKeyValues { get; private set; }

      public PaneData(AxisData axis) : this(axis, new Dictionary<string, string>(), new List<IComparer<object>>())
      {
      }

      public PaneData(AxisData axis, IReadOnlyDictionary<string, string> fieldKeyValues, IReadOnlyList<IComparer<object>> fieldValueComparers) : base(axis)
      {
         Id = ShortGuid.NewGuid();
         FieldKeyValues = fieldKeyValues;
         _fieldValueComparers = fieldValueComparers;
         _allCurves = new Cache<string, CurveData<TX, TY>>(x => x.Id, x => null);
         _fieldValues = fieldKeyValues.Values.ToList();
      }

      public ICache<string, CurveData<TX, TY>> Curves => _allCurves;

      public void AddCurve(CurveData<TX, TY> curve)
      {
         _allCurves.Add(curve);
         curve.Pane = this;
      }

      public virtual int Compare(CurveData<TX, TY> x, CurveData<TX, TY> y)
      {
         // compare x and y for each valueField by valueComparers
         return _fieldValueComparers
            .Select((comparer, i) => comparer.Compare(x.FieldValues[i], y.FieldValues[i]))
            .FirstOrDefault(compareResult => compareResult != 0);
      }

      public virtual void CreateCurveOrder()
      {
         //without ToList sortedCurves is empty after Clear
         var sortedCurves = _allCurves.OrderBy(x => x, this).ToList();
         _allCurves.Clear();
         _allCurves.AddRange(sortedCurves);
      }

      public override AxisData ChartAxis => Chart.Axis;

      public virtual IEnumerable<string> FieldNames => FieldKeyValues.Keys;

      public virtual IReadOnlyList<string> FieldValues => _fieldValues;
   }
}