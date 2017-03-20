using System.Collections.Generic;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_ScatterChartDataToDataTableMapper : ContextSpecification<ScatterChartDataToDataTableMapper>
   {
      protected override void Context()
      {
         sut = new ScatterChartDataToDataTableMapper();
      }
   }

   public class When_mapping_a_scatter_chart_data_to_a_datatable : concern_for_ScatterChartDataToDataTableMapper
   {
      private ChartData<ScatterXValue, ScatterYValue> _chartData;
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
         _chartData = new ChartData<ScatterXValue, ScatterYValue>(_xAxis, null, null, null);
         var pane1 = new PaneData<ScatterXValue, ScatterYValue>(_yAxis) {Caption = "Male"};
         var pane2 = new PaneData<ScatterXValue, ScatterYValue>(_yAxis) {Caption = "Female"};
         _chartData.AddPane(pane1);
         _chartData.AddPane(pane2);
         var curve1 = new CurveData<ScatterXValue, ScatterYValue> {Caption = "Liver"};
         curve1.Add(new ScatterXValue(1), new ScatterYValue(10));
         curve1.Add(new ScatterXValue(2), new ScatterYValue(20));
         pane1.AddCurve(curve1);

         var curve2 = new CurveData<ScatterXValue, ScatterYValue> {Caption = "Kidney"};
         curve2.Add(new ScatterXValue(3), new ScatterYValue(30));
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
         _dataTable.Columns.Count.ShouldBeEqualTo(4);
         _dataTable.Columns[_xAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[_yAxis.Caption].ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_number_of_rows_in_the_datatable()
      {
         _dataTable.Rows.Count.ShouldBeEqualTo(3);
         _dataTable.AllValuesInColumn<float>(_xAxis.Caption).ShouldOnlyContain(1, 2, 3);
         ;
         _dataTable.AllValuesInColumn<float>(_yAxis.Caption).ShouldOnlyContain(10, 20, 30);
         ;
      }
   }
}