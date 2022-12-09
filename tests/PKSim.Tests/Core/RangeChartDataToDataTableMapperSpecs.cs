using System.Collections.Generic;
using System.Data;
using PKSim.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Core
{
   public abstract class concern_for_RangeChartDataToDataTableMapper : ContextSpecification<RangeChartDataToDataTableMapper>
   {
      protected override void Context()
      {
         sut = new RangeChartDataToDataTableMapper();
      }
   }

   public class When_mapping_a_range_chart_data_to_a_datatable : concern_for_RangeChartDataToDataTableMapper
   {
      private ChartData<RangeXValue, RangeYValue> _chartData;
      private DataTable _dataTable;
      private AxisData _xAxis;
      private AxisData _yAxis;
      private IReadOnlyList<DataTable> _tables;

      protected override void Context()
      {
         base.Context();
         var timeDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         var concDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _xAxis = new AxisData(timeDimension, timeDimension.DefaultUnit, Scalings.Linear) {Caption = "X"};
         _yAxis = new AxisData(concDimension, concDimension.DefaultUnit, Scalings.Linear) {Caption = "Y"};
         _chartData = new ChartData<RangeXValue, RangeYValue>(_xAxis, null);
         var pane1 = new PaneData<RangeXValue, RangeYValue>(_yAxis) {Caption = "Male"};
         var pane2 = new PaneData<RangeXValue, RangeYValue>(_yAxis) {Caption = "Female"};
         _chartData.AddPane(pane1);
         _chartData.AddPane(pane2);
         var curve1 = new CurveData<RangeXValue, RangeYValue> {Caption = "Liver"};
         curve1.Add(new RangeXValue(1) {Minimum = 0, Maximum = 1.5f, NumberOfItems = 5}, new RangeYValue {LowerPercentile = 10, Median = 20, UpperPercentile = 30});
         curve1.Add(new RangeXValue(2) {Minimum = 1.8f, Maximum = 2.5f, NumberOfItems = 10}, new RangeYValue {LowerPercentile = 20, Median = 30, UpperPercentile = 40});
         pane1.AddCurve(curve1);

         var curve2 = new CurveData<RangeXValue, RangeYValue> {Caption = "Kidney"};
         curve2.Add(new RangeXValue(3) {Minimum = 2f, Maximum = 4f, NumberOfItems = 15}, new RangeYValue {LowerPercentile = 30, Median = 40, UpperPercentile = 50});
         pane2.AddCurve(curve2);
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

         _dataTable.Columns[_xAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[CompositeNameFor(PKSimConstants.UI.Minimum, _xAxis.Caption)].ShouldNotBeNull();
         _dataTable.Columns[CompositeNameFor(PKSimConstants.UI.Maximum, _xAxis.Caption)].ShouldNotBeNull();
         _dataTable.Columns[PKSimConstants.UI.NumberOfIndividuals].ShouldNotBeNull();
         _dataTable.Columns[_yAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[CompositeNameFor(PKSimConstants.UI.LowerPercentile, _yAxis.Caption)].ShouldNotBeNull();
         _dataTable.Columns[CompositeNameFor(PKSimConstants.UI.UpperPercentile, _yAxis.Caption)].ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_number_of_rows_in_the_datatable()
      {
         _dataTable.Rows.Count.ShouldBeEqualTo(3);
         _dataTable.AllValuesInColumn<float>(_xAxis.Caption).ShouldOnlyContain(1, 2, 3);
         _dataTable.AllValuesInColumn<float>(CompositeNameFor(PKSimConstants.UI.Minimum, _xAxis.Caption)).ShouldOnlyContain(0, 1.8f, 2f);
         _dataTable.AllValuesInColumn<float>(CompositeNameFor(PKSimConstants.UI.LowerPercentile, _yAxis.Caption)).ShouldOnlyContain(10, 20, 30);
         _dataTable.AllValuesInColumn<float>(_yAxis.Caption).ShouldOnlyContain(20, 30, 40);
         _dataTable.AllValuesInColumn<float>(CompositeNameFor(PKSimConstants.UI.UpperPercentile, _yAxis.Caption)).ShouldOnlyContain(30, 40, 50);
         _dataTable.AllValuesInColumn<float>(CompositeNameFor(PKSimConstants.UI.Maximum, _xAxis.Caption)).ShouldOnlyContain(1.5f, 2.5f, 4f);
      }
   }

   public class When_mapping_an_undefined_chart_data_to_a_datatable : concern_for_RangeChartDataToDataTableMapper
   {
      private DataTable _dataTable;

      protected override void Because()
      {
         var tables = sut.MapFrom(null);
         _dataTable = tables[0];
      }

      [Observation]
      public void should_return_two_empty_datatables()
      {
         _dataTable.ShouldNotBeNull();
         _dataTable.Rows.Count.ShouldBeEqualTo(0);
      }
   }
}