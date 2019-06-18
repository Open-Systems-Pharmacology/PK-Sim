using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using OSPSuite.Core.Domain;
using Helper = PKSim.Core.ChartDataHelperForSpecs;


namespace PKSim.Core
{
   public abstract class concern_for_BoxWhiskerChartDataToDataTableMapper : ContextSpecification<ChartDataToDataTableMapper<BoxWhiskerXValue, BoxWhiskerYValue>>
   {
      protected override void Context()
      {
         sut = new BoxWhiskerChartDataToDataTableMapper();
      }
   }

   public class When_mapping_a_box_whisker_chart_data_to_a_datatable : concern_for_BoxWhiskerChartDataToDataTableMapper
   {
      private DataTable _dataTable;
      private ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;
      private PaneData<BoxWhiskerXValue, BoxWhiskerYValue> _paneData1;
      private PaneData<BoxWhiskerXValue, BoxWhiskerYValue> _paneData2;
      private IReadOnlyList<DataTable> _tables;

      protected override void Context()
      {
         base.Context();
         _chartData = new ChartData<BoxWhiskerXValue, BoxWhiskerYValue>(null, ChartDataHelperForSpecs.FieldValueComparersR1(), Helper.XValueNames, Helper.XValueComparer());
         _paneData1 = Helper.CreateBoxWhiskerPaneData11(_chartData);
         _paneData2 = Helper.CreateBoxWhiskerPaneData12(_chartData);
         _chartData.AddPane(_paneData2);
         _chartData.AddPane(_paneData1);
      }

      protected override void Because()
      {
         _tables = sut.MapFrom(_chartData);
         _dataTable = _tables[0];
      }

      [Observation]
      public void should_not_have_created_a_pivot_table()
      {
         _tables.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_created_the_expected_number_of_columns_in_the_datatable()
      {
         _dataTable.Columns.Count.ShouldBeEqualTo(9);

         _dataTable.Columns.Contains("Pane Caption").ShouldBeTrue();
         _dataTable.Columns.Contains("Curve Caption").ShouldBeTrue();
         _dataTable.Columns.Contains("X Value").ShouldBeTrue();
         _dataTable.Columns.Contains("Lower Whisker").ShouldBeTrue();
         _dataTable.Columns.Contains("Lower Box").ShouldBeTrue();
         _dataTable.Columns.Contains("Median").ShouldBeTrue();
         _dataTable.Columns.Contains("Upper Box").ShouldBeTrue();
         _dataTable.Columns.Contains("Upper Whisker").ShouldBeTrue();
         _dataTable.Columns.Contains("Variable").ShouldBeTrue();
      }

      [Observation]
      public void should_have_created_the_expected_number_of_rows_in_the_datatable()
      {
         _dataTable.Rows.Count.ShouldBeEqualTo(7);
      }

      [Observation]
      public void should_have_created_a_table_with_the_correct_data()
      {
         var testRow = _dataTable.Rows[0];
         testRow["Pane Caption"].ToString().ShouldBeEqualTo(_paneData2.Caption);
         var curveData = _paneData2.Curves.First();
         testRow["Curve Caption"].ToString().ShouldBeEqualTo(curveData.Caption);
         testRow["X Value"].ToString().ShouldBeEqualTo(curveData.XValues[0].ToString(curveData.XAxis));
         testRow["Lower Whisker"].ToString().ShouldBeEqualTo(((float) curveData.YAxis.ConvertToDisplayUnit(curveData.YValues[0].LowerWhisker)).ToString());
         testRow["Lower Box"].ToString().ShouldBeEqualTo(((float) curveData.YAxis.ConvertToDisplayUnit(curveData.YValues[0].LowerBox)).ToString());
         testRow["Median"].ToString().ShouldBeEqualTo(((float) curveData.YAxis.ConvertToDisplayUnit(curveData.YValues[0].Median)).ToString());
         testRow["Upper Box"].ToString().ShouldBeEqualTo(((float) curveData.YAxis.ConvertToDisplayUnit(curveData.YValues[0].UpperBox)).ToString());
         testRow["Upper Whisker"].ToString().ShouldBeEqualTo(((float) curveData.YAxis.ConvertToDisplayUnit(curveData.YValues[0].UpperWhisker)).ToString());
      }
   }

   public class When_mapping_an_undefined_chart_data_to_datatable : concern_for_BoxWhiskerChartDataToDataTableMapper
   {
      private IReadOnlyList<DataTable> _dataTables;

      protected override void Because()
      {
         _dataTables = sut.MapFrom(null);
      }

      [Observation]
      public void should_return_two_empty_tables()
      {
         _dataTables.Count.ShouldBeEqualTo(1);
         _dataTables[0].Rows.Count.ShouldBeEqualTo(0);
         _dataTables[0].Columns.Count.ShouldBeEqualTo(0);
      }
   }
}