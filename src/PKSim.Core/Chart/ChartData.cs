using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Chart
{
   public class ChartData<TX, TY> :  IComparer<PaneData<TX, TY>> where TX : IXValue where TY : IYValue
   {
      private readonly IReadOnlyList<IComparer<object>> _fieldValueComparers;
      public AxisData Axis { get; private set; }
      private readonly IComparer<TX> _xValueComparer;
      private SortedList<TX, int> _allXValues;
      private readonly Cache<string, PaneData<TX, TY>> _allPanes;
      public IReadOnlyList<string> XFieldNames { get; private set; }

      public IEnumerable<KeyValuePair<TX, int>> AllXValues
      {
         get { return _allXValues.All(); }
      }

      public ChartData(AxisData axis, IReadOnlyList<IComparer<object>> fieldValueComparers):this(axis, fieldValueComparers,new List<string>(),null)
      {
 
      }

      public ChartData(AxisData axis, IReadOnlyList<IComparer<object>> fieldValueComparers, IReadOnlyList<string> xFieldNames, IComparer<TX> xValueComparer)
      {
         Axis = axis;
         XFieldNames = xFieldNames ?? new List<string>();
         _xValueComparer = xValueComparer;
         _fieldValueComparers = fieldValueComparers;
         _allPanes = new Cache<string, PaneData<TX, TY>>(x => x.Id, x => null);
      }

      public ICache<string,PaneData<TX, TY>> Panes
      {
         get { return _allPanes; }
      }

      public int Compare(PaneData<TX, TY> x, PaneData<TX, TY> y)
      {
         // compare x and y for each valueField by valueComparers
         var compareResults = _fieldValueComparers.Select((comparer, i) => comparer.Compare(x.FieldValues[i], y.FieldValues[i]));
         return compareResults.FirstOrDefault(compareResult => compareResult != 0);
      }

      public void CreatePaneOrder()
      {
         //without ToList sortedPanes is empty after Clear
         var sortedPanes = _allPanes.OrderBy(x => x, this).ToList(); 
         _allPanes.Clear();
         _allPanes.AddRange(sortedPanes);
      }

      public void CreateXOrder()
      {
         if (_xValueComparer == null)
            return;

         _allXValues = new SortedList<TX, int>(_xValueComparer);

         // build sorted List of all distinct xValues
         foreach (var xValue in allPossibleXValues)
         {
            if (!_allXValues.ContainsKey(xValue))
               _allXValues.Add(xValue, -1);
         }

         // create index values
         for (int i = 0; i < _allXValues.Count; i++)
            _allXValues[_allXValues.Keys[i]] = i;

         // assign index values
         foreach (var xValue in allPossibleXValues)
            xValue.X = _allXValues[xValue];
      }

      private IEnumerable<TX> allPossibleXValues =>
         from pane in _allPanes
         from curve in pane.Curves
         from xValue in curve.XValues
         select xValue;

      public void AddPane(PaneData<TX, TY>  pane)
      {
         _allPanes.Add(pane);
         pane.Chart = this;
      }
   }
}