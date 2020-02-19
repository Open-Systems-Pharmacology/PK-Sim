using System;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.Mappers;

using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_TimeProfileChartDataToDataTableMapper : ContextSpecification<TimeProfileChartDataToDataTableMapper>
   {
      protected override void Context()
      {
         sut = new TimeProfileChartDataToDataTableMapper();
      }
   }

   public class When_mapping_a_time_profile_chart_data_to_a_datatable : concern_for_TimeProfileChartDataToDataTableMapper
   {
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private DataTable _dataTable;
      private DataTable _pivotedDataTable;
      private AxisData _xAxis;
      private AxisData _yAxis;

      protected override void Context()
      {
         base.Context();
         var timeDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         var concDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _xAxis = new AxisData(timeDimension, timeDimension.DefaultUnit, Scalings.Linear) {Caption = "X"};
         _yAxis = new AxisData(concDimension, concDimension.DefaultUnit, Scalings.Linear) {Caption = "Y"};
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(_xAxis, null, null, null);
         var pane1 = new PaneData<TimeProfileXValue, TimeProfileYValue>(_yAxis) {Caption = "Male"};
         var pane2 = new PaneData<TimeProfileXValue, TimeProfileYValue>(_yAxis) {Caption = "Female"};
         _chartData.AddPane(pane1);
         _chartData.AddPane(pane2);
         var curve1 = new CurveData<TimeProfileXValue, TimeProfileYValue> {Caption = "Liver"};
         curve1.Add(new TimeProfileXValue(1), new TimeProfileYValue {Y = 10});
         curve1.Add(new TimeProfileXValue(2), new TimeProfileYValue {LowerValue = 20, UpperValue = 30});
         pane1.AddCurve(curve1);

         var curve2 = new CurveData<TimeProfileXValue, TimeProfileYValue> {Caption = "Kidney"};
         curve2.Add(new TimeProfileXValue(3), new TimeProfileYValue {Y = 40});
         pane2.AddCurve(curve2);
      }

      protected override void Because()
      {
         var tables = sut.MapFrom(_chartData);
         _dataTable = tables[0];
         _pivotedDataTable = tables[1];
      }

      [Observation]
      public void should_have_created_the_expected_number_of_columns_in_the_pivoted_datatable()
      {
         _pivotedDataTable.Columns.Count.ShouldBeEqualTo(5);
         _pivotedDataTable.Columns.Contains(_xAxis.Caption).ShouldBeTrue();
         _pivotedDataTable.Columns[_xAxis.Caption].DataType.ShouldBeEqualTo(typeof(float));
         _pivotedDataTable.Columns.Contains(string.Format("Male.Liver({0})", CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerValue, _yAxis.Caption))).ShouldBeTrue();
         _pivotedDataTable.Columns.Contains(string.Format("Male.Liver({0})", _yAxis.Caption)).ShouldBeTrue();
         _pivotedDataTable.Columns.Contains(string.Format("Male.Liver({0})", CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperValue, _yAxis.Caption))).ShouldBeTrue();
         _pivotedDataTable.Columns.Contains(string.Format("Female.Kidney({0})", _yAxis.Caption)).ShouldBeTrue();
      }

      [Observation]
      public void should_have_created_the_expected_number_of_rows_in_the_pivoted_datatable()
      {
         _pivotedDataTable.Rows.Count.ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_have_created_the_expected_number_of_columns_in_the_datatable()
      {
         _dataTable.Columns.Count.ShouldBeEqualTo(6);

         _dataTable.Columns[_xAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerValue, _yAxis.Caption)].ShouldNotBeNull();
         _dataTable.Columns[_yAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperValue, _yAxis.Caption)].ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_number_of_rows_in_the_datatable()
      {
         _dataTable.Rows.Count.ShouldBeEqualTo(3);
         _dataTable.AllValuesInColumn<float>(_xAxis.Caption).ShouldOnlyContain(1, 2, 3);
         var values = _dataTable.AllValuesInColumn<object>(CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerValue, _yAxis.Caption));
         values[0].ShouldBeEqualTo(DBNull.Value);
         values[1].ShouldBeEqualTo(20);
         values[2].ShouldBeEqualTo(DBNull.Value);

         values = _dataTable.AllValuesInColumn<object>(_yAxis.Caption);
         values[0].ShouldBeEqualTo(10);
         values[1].ShouldBeEqualTo(DBNull.Value);
         values[2].ShouldBeEqualTo(40);

         values = _dataTable.AllValuesInColumn<object>(CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperValue, _yAxis.Caption));
         values[0].ShouldBeEqualTo(DBNull.Value);
         values[1].ShouldBeEqualTo(30);
         values[2].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_mapping_a_time_profile_chart_data_with_observed_data_to_a_datatable : concern_for_TimeProfileChartDataToDataTableMapper
   {
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private DataTable _dataTable;
      private DataTable _pivotedDataTable;
      private AxisData _xAxis;
      private AxisData _yAxis;
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         var timeDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         var concDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _xAxis = new AxisData(timeDimension, timeDimension.DefaultUnit, Scalings.Linear) {Caption = "X"};
         _yAxis = new AxisData(concDimension, concDimension.DefaultUnit, Scalings.Linear) {Caption = "Y"};
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(_xAxis, null, null, null);
         var pane1 = new PaneData<TimeProfileXValue, TimeProfileYValue>(_yAxis) {Caption = "Male"};
         var pane2 = new PaneData<TimeProfileXValue, TimeProfileYValue>(_yAxis) {Caption = "Female"};
         _chartData.AddPane(pane1);
         _chartData.AddPane(pane2);
         var curve1 = new CurveData<TimeProfileXValue, TimeProfileYValue> {Caption = "Liver"};
         curve1.Add(new TimeProfileXValue(1), new TimeProfileYValue {Y = 10});
         curve1.Add(new TimeProfileXValue(2), new TimeProfileYValue {LowerValue = 20, UpperValue = 30});
         pane1.AddCurve(curve1);

         var curve2 = new CurveData<TimeProfileXValue, TimeProfileYValue> {Caption = "Kidney"};
         curve2.Add(new TimeProfileXValue(3), new TimeProfileYValue {Y = 40});
         pane2.AddCurve(curve2);

         _observedData = DomainHelperForSpecs.ObservedData();
         var displayPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         var dimensionRepository = A.Fake<IDimensionRepository>();
         var observedDataMapper = new DataRepositoryToObservedCurveDataMapper(displayPathMapper, dimensionRepository);
         var observedDataCurves = observedDataMapper.MapFrom(_observedData, new ObservedDataCollection());
         observedDataCurves.Each(pane1.AddObservedCurve);
         observedDataMapper.MapFrom(_observedData, new ObservedDataCollection()).Each(curve =>
         {
            curve.Visible = false;
            pane1.AddObservedCurve(curve);
         });
      }

      protected override void Because()
      {
         var tables = sut.MapFrom(_chartData);
         _dataTable = tables[0];
         _pivotedDataTable = tables[1];
      }

      [Observation]
      public void should_have_created_the_expected_number_of_columns_in_the_datatable()
      {
         _dataTable.Columns.Count.ShouldBeEqualTo(6);

         _dataTable.Columns[_xAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerValue, _yAxis.Caption)].ShouldNotBeNull();
         _dataTable.Columns[_yAxis.Caption].ShouldNotBeNull();
         _dataTable.Columns[CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperValue, _yAxis.Caption)].ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_expected_number_of_rows_in_the_datatable()
      {
         _dataTable.Rows.Count.ShouldBeEqualTo(6);
         _dataTable.AllValuesInColumn<float>(_xAxis.Caption).ShouldOnlyContain(1, 2, 3, 1, 2, 3);
         var values = _dataTable.AllValuesInColumn<object>(CoreConstants.CompositeNameFor(PKSimConstants.UI.LowerValue, _yAxis.Caption));
         values[0].ShouldBeEqualTo(DBNull.Value);
         values[1].ShouldBeEqualTo(20);
         values[2].ShouldBeEqualTo(10);
         values[3].ShouldBeEqualTo(20);
         values[4].ShouldBeEqualTo(30);
         values[5].ShouldBeEqualTo(DBNull.Value);

         values = _dataTable.AllValuesInColumn<object>(_yAxis.Caption);
         values[0].ShouldBeEqualTo(10);
         values[1].ShouldBeEqualTo(DBNull.Value);
         values[2].ShouldBeEqualTo(10);
         values[3].ShouldBeEqualTo(20);
         values[4].ShouldBeEqualTo(30);
         values[5].ShouldBeEqualTo(40);

         values = _dataTable.AllValuesInColumn<object>(CoreConstants.CompositeNameFor(PKSimConstants.UI.UpperValue, _yAxis.Caption));
         values[0].ShouldBeEqualTo(DBNull.Value);
         values[1].ShouldBeEqualTo(30);
         values[2].ShouldBeEqualTo(10);
         values[3].ShouldBeEqualTo(20);
         values[4].ShouldBeEqualTo(30);
         values[5].ShouldBeEqualTo(DBNull.Value);
      }
   }
}