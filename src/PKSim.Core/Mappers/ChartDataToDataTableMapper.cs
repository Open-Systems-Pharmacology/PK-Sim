using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Data;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Mappers
{
   public interface IChartDataToTableMapperFactory
   {
      IChartDataToDataTableMapper<TXValue, TYValue> Create<TXValue, TYValue>() where TXValue : IXValue where TYValue : IYValue;
   }

   /// <summary>
   ///    Maps Chart Data to data table. Returns two tables, one containing the row data, and one containing the pivoted data
   /// </summary>
   public interface IChartDataToDataTableMapper<TXValue, TYValue> : IMapper<ChartData<TXValue, TYValue>, IReadOnlyList<DataTable>>
      where TXValue : IXValue
      where TYValue : IYValue
   {
   }

   public abstract class ChartDataToDataTableMapper<TXValue, TYValue> : IChartDataToDataTableMapper<TXValue, TYValue>
      where TXValue : IXValue
      where TYValue : IYValue
   {
      private readonly bool _canExportPivotedValues;
      private const string PANE_CAPTION = "Pane Caption";
      private const string CURVE_CAPTION = "Curve Caption";

      protected ChartDataToDataTableMapper(bool canExportPivotedValues = false)
      {
         _canExportPivotedValues = canExportPivotedValues;
      }

      public IReadOnlyList<DataTable> MapFrom(ChartData<TXValue, TYValue> chartData)
      {
         var dataTables = new List<DataTable>();
         var firstCurve = firstCurveDefinedIn(chartData);
         var dataTable = getRawTableDataBasedOn(chartData, firstCurve, exportForPivot: false);
         dataTables.Add(dataTable);

         if (_canExportPivotedValues)
         {
            var dataTableForPivot = getRawTableDataBasedOn(chartData, firstCurve, exportForPivot: true);
            dataTables.Add(getPivotedTable(dataTableForPivot, firstCurve));
         }

         return dataTables;
      }

      private DataTable getRawTableDataBasedOn(ChartData<TXValue, TYValue> chartData, CurveData<TXValue, TYValue> firstCurve, bool exportForPivot)
      {
         var dataTable = new DataTable();

         //not a single curve in the given chart data. nothing to export
         if (firstCurve == null)
            return dataTable;

         dataTable.AddColumn(PANE_CAPTION);
         dataTable.AddColumn(CURVE_CAPTION);

         AddSpecificChartColumns(dataTable, firstCurve, exportForPivot);

         dataTable.BeginLoadData();
         foreach (var pane in chartData.Panes)
         {
            AddPanesToTable(dataTable, pane, exportForPivot);
            AddObservedDataToTable(dataTable, pane, exportForPivot);
         }
         dataTable.EndLoadData();

         return dataTable;
      }

      protected virtual void AddPanesToTable(DataTable dataTable, PaneData<TXValue, TYValue> pane, bool exportForPivot)
      {
         AddCurvesToTable(dataTable, pane, pane.Curves, exportForPivot, AddSpecificChartValues);
      }

      protected void AddCurvesToTable<TX, TY>(DataTable dataTable, PaneData<TXValue, TYValue> pane, IEnumerable<CurveData<TX, TY>> curves, bool exportForPivot, Func<DataRow, CurveData<TX, TY>, bool, IEnumerable<DataRow>> specificValueRetriever) where TX : IXValue where TY : IYValue
      {
         curves.Each(curve => AddCurveToTable(dataTable, pane, curve, exportForPivot, specificValueRetriever));
      }

      protected void AddCurveToTable<TX, TY>(DataTable dataTable, PaneData<TXValue, TYValue> pane, CurveData<TX, TY> curve, bool exportForPivot, Func<DataRow, CurveData<TX, TY>, bool, IEnumerable<DataRow>> specificValueRetriever)
         where TX : IXValue
         where TY : IYValue
      {
         var row = dataTable.NewRow();
         row[PANE_CAPTION] = pane.Caption;
         row[CURVE_CAPTION] = curve.Caption;
         specificValueRetriever(row, curve, exportForPivot).Each(dataTable.Rows.Add);
      }

      protected virtual void AddObservedDataToTable(DataTable dataTable, PaneData<TXValue, TYValue> pane, bool exportForPivot)
      {
      }

      private DataTable getPivotedTable(DataTable table, CurveData<TXValue, TYValue> firstCurve)
      {
         const string tableNameSuffix = " Pivoted";

         if (firstCurve == null)
            return new DataTable(tableNameSuffix);

         var pivoter = new Pivoter();
         var dataFields = GetDataFields(firstCurve);
         var aggregate = new Aggregate<float, float>
         {
            Aggregation = floats => floats.FirstOrDefault(),
            Name = dataFields.Count == 1 ? dataFields[0] : string.Empty
         };

         var pivotInfo = new PivotInfo(rowFields: GetRowFields(firstCurve),
            dataFields: dataFields,
            columnFields: new[] {PANE_CAPTION, CURVE_CAPTION},
            aggregates: new[] {aggregate});

         var pivotTable = pivoter.PivotData(table.DefaultView, pivotInfo);
         pivotTable.TableName += tableNameSuffix;

         //purge empty columns
         foreach (var column in getEmptyColumns(pivotTable))
            pivotTable.Columns.Remove(column);

         PerformSpecificTransformationOnPivotedTable(pivotTable);
         return pivotTable;
      }

      protected virtual void PerformSpecificTransformationOnPivotedTable(DataTable pivotTable)
      {
         //nothing to do here
      }

      private IEnumerable<DataColumn> getEmptyColumns(DataTable table)
      {
         return table.Columns.Cast<DataColumn>().ToArray().Where(column => table.Rows.Cast<DataRow>().All(row => row.IsNull(column)));
      }

      private CurveData<TXValue, TYValue> firstCurveDefinedIn(ChartData<TXValue, TYValue> chartData)
      {
         return chartData?.Panes.SelectMany(x => x.Curves).FirstOrDefault();
      }

      protected abstract void AddSpecificChartColumns(DataTable dataTable, CurveData<TXValue, TYValue> curveData, bool exportForPivot);
      protected abstract IEnumerable<DataRow> AddSpecificChartValues(DataRow row, CurveData<TXValue, TYValue> curveData, bool exportForPivot);

      protected virtual IReadOnlyList<string> GetDataFields(CurveData<TXValue, TYValue> curveData)
      {
         return new List<string>();
      }

      protected virtual IReadOnlyList<string> GetRowFields(CurveData<TXValue, TYValue> curveData)
      {
         return new List<string>();
      }

      protected object ValueForDataTableFor(IWithDisplayUnit withDisplayUnit, double value)
      {
         return double.IsNaN(value) ? DBNull.Value : (object) withDisplayUnit.ConvertToDisplayUnit(value);
      }
   }
}