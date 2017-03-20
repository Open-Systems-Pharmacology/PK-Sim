using System.Collections.Generic;
using System.Data;
using OSPSuite.Core.Chart;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Chart;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   internal abstract class CurveChartReporter<TCurveChart> : OSPSuiteTeXReporter<TCurveChart> where TCurveChart:ICurveChart
   {
      public override IReadOnlyCollection<object> Report(TCurveChart chart, OSPSuiteTracker buildTracker)
      {
         var listToReport = new List<object>();

         if (chart.Curves.Count == 0) return listToReport;

         listToReport.Add(new Chapter(PKSimConstants.UI.Chart));
         if (!string.IsNullOrEmpty(chart.Name))
            listToReport.Add(new Section(chart.Name));

         listToReport.Add(chart);

         AddSpecificObjectsToReport(chart, listToReport);
         return listToReport;
      }

      protected virtual void AddSpecificObjectsToReport(TCurveChart chart, List<object> listToReport)
      {

      }
   }

   internal class SimulationChartReporter : CurveChartReporter<SimulationTimeProfileChart>
   {
      
   }
   internal class ConcentrationChartReporter : CurveChartReporter<CurveChart>
   {
      protected override void AddSpecificObjectsToReport(CurveChart chart, List<object> listToReport)
      {
         foreach (var curve in chart.Curves)
         {
            listToReport.Add(dataTableFor(chart, curve));
         }
      }

      private DataTable dataTableFor(ICurveChart chart, ICurve curve)
      {
         var xName = chart.Axes[AxisTypes.X].Caption;
         var yName = chart.Axes[AxisTypes.Y].Caption;

         var xUnit = chart.Axes[AxisTypes.X].UnitName;
         var yUnit = chart.Axes[AxisTypes.Y].UnitName;

         var dt = new DataTable(curve.Name);
         dt.Columns.Add(xName, typeof(float));
         dt.Columns.Add(yName, typeof(float));

         dt.BeginLoadData();
         for (var i = 0; i < curve.xData.Values.Count; i++)
         {
            var newRow = dt.NewRow();
            newRow[xName] = TEXHelper.ValueInDisplayUnit(curve.XDimension, curve.XDimension.Unit(xUnit), curve.xData.Values[i]);
            newRow[yName] = TEXHelper.ValueInDisplayUnit(curve.YDimension, curve.YDimension.Unit(yUnit), curve.yData.Values[i]);
            dt.Rows.Add(newRow);
         }
         dt.EndLoadData();

         return dt;
      }
   }
}