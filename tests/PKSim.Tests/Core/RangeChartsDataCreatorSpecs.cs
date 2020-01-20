using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_RangeChartsDataCreator : ContextSpecification<IRangeChartDataCreator>
   {
      protected readonly PopulationAnalysisCovariateField _genderField = new PopulationAnalysisCovariateField { Name = "GENDER", Covariate = "GENDER" };
      protected readonly PopulationAnalysisCovariateField _raceField = new PopulationAnalysisCovariateField { Name = "RACE", Covariate = "RACE" };
      protected readonly PopulationAnalysisParameterField _bmiField = new PopulationAnalysisParameterField { Name = "BMI", ParameterPath = "BMI" };
      protected readonly PopulationAnalysisPKParameterField _cmaxField = new PopulationAnalysisPKParameterField { Name = "Cmax", QuantityPath = "Path", PKParameter = "Cmax" };
      protected IPopulationAnalysisField _bmiClass;
      protected PopulationPivotAnalysis _pivotAnalysis;
      private IDimensionRepository _dimensionRepository;
      private IPivotResultCreator _pivotResultCreator;
      private IBinIntervalsCreator _binIntervalsCreator;

      protected override void Context()
      {
         _pivotAnalysis = new PopulationPivotAnalysis();
         _pivotAnalysis.Add(_genderField);
         _pivotAnalysis.Add(_raceField);
         _pivotAnalysis.Add(_bmiField);
         _pivotAnalysis.Add(_cmaxField);
         var grouping = new FixedLimitsGroupingDefinition(_bmiField.Name);
         grouping.SetLimits(new[] { 25d }.OrderBy(x => x));
         grouping.AddItems(PopulationAnalysisHelperForSpecs.BMIGroups);
         _bmiClass = new PopulationAnalysisGroupingField(grouping) { Name = "BMI class" };
         _pivotAnalysis.Add(_bmiClass);
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _pivotResultCreator = A.Fake<IPivotResultCreator>();
         var userSettings= A.Fake<ICoreUserSettings>();
         userSettings.NumberOfIndividualsPerBin = 1;
         userSettings.NumberOfBins = 2;
         _binIntervalsCreator = new BinIntervalsCreator(userSettings);
         sut = new RangeChartDataCreator(_dimensionRepository, _pivotResultCreator,_binIntervalsCreator);
      }
   }

   public class When_creating_the_range_chart_data_based_on_valid_data_containing_grouping_on_rows_and_columns : concern_for_RangeChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<RangeXValue, RangeYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis.SetPosition(_genderField, PivotArea.RowArea, 0);
         _pivotAnalysis.SetPosition(_raceField, PivotArea.RowArea, 1);
         _pivotAnalysis.SetPosition(_bmiClass, PivotArea.ColumnArea, 0);
         _pivotAnalysis.SetPosition(_cmaxField, PivotArea.DataArea, 0);
         _pivotAnalysis.SetPosition(_bmiField, PivotArea.DataArea, 1);

         _pivotResult = ChartDataHelperForSpecs.CreatePivotResult(_pivotAnalysis, AggregationFunctions.ValuesAggregation, _genderField, _raceField, _bmiField, _cmaxField);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_one_pane_for_each_unique_grouping_in_row_fields()
      {
         _chartData.ShouldNotBeNull();
         //{Male,US}, {Female, EU}, {Male, EU}
         _chartData.Panes.Count().ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_have_added_the_expected_series()
      {
         foreach (var pane in _chartData.Panes)
         {
            //only one series for each pane according to data
            pane.Curves.Count.ShouldBeEqualTo(1);
         }
      }
   }

   public class When_creating_the_range_chart_data_based_on_valid_data_containing_only_grouping_on_columns : concern_for_RangeChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<RangeXValue, RangeYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis.SetPosition(_bmiClass, PivotArea.ColumnArea, 0);
         _pivotAnalysis.SetPosition(_cmaxField, PivotArea.DataArea, 0);
         _pivotAnalysis.SetPosition(_bmiField, PivotArea.DataArea, 1);

         _pivotResult = ChartDataHelperForSpecs.CreatePivotResult(_pivotAnalysis, AggregationFunctions.ValuesAggregation, _genderField, _raceField, _bmiField, _cmaxField);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_a_default_pane()
      {
         _chartData.ShouldNotBeNull();
         _chartData.Panes.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_added_the_expected_series_to_the_default_plane()
      {
         var defaultPane = _chartData.Panes.First();
         //BMI Groups used are only {thin, thin, big}
         defaultPane.Curves.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_bridged_up_the_gap_between_grouping_for_each_pane()
      {
         var defaultPane = _chartData.Panes.First();
         var firstCurve = defaultPane.Curves.ElementAt(0);
         var secondCurve = defaultPane.Curves.ElementAt(1);
         firstCurve.XValues.Last().ShouldBeEqualTo(secondCurve.XValues.First());
         firstCurve.YValues.Last().ShouldBeEqualTo(secondCurve.YValues.First());
      }
   }


   public class When_creating_the_range_chart_data_based_on_valid_data_containing_only_grouping_on_columns_where_data_bridge_should_not_happen : concern_for_RangeChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<RangeXValue, RangeYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis.SetPosition(_bmiClass, PivotArea.ColorArea, 0);
         _pivotAnalysis.SetPosition(_cmaxField, PivotArea.DataArea, 0);
         _pivotAnalysis.SetPosition(_bmiField, PivotArea.DataArea, 1);

         _pivotResult = ChartDataHelperForSpecs.CreatePivotResult(_pivotAnalysis, 
            AggregationFunctions.ValuesAggregation, 
            _genderField, _raceField, _bmiField, _cmaxField,
            cmaxValues:new double[]{600,800,600});
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_not_bridged_up_the_gap_between_grouping_for_each_pane()
      {
         var defaultPane = _chartData.Panes.First();
         var firstCurve = defaultPane.Curves.ElementAt(0);
         var secondCurve = defaultPane.Curves.ElementAt(1);
         firstCurve.XValues.Last().ShouldNotBeEqualTo(secondCurve.XValues.First());
         firstCurve.YValues.Last().ShouldNotBeEqualTo(secondCurve.YValues.First());
      }
   }

   public class When_creating_the_range_chart_data_based_on_valid_data_containing_only_grouping_on_rows : concern_for_RangeChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<RangeXValue, RangeYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis.SetPosition(_genderField, PivotArea.RowArea, 0);
         _pivotAnalysis.SetPosition(_cmaxField, PivotArea.DataArea, 0);
         _pivotAnalysis.SetPosition(_bmiField, PivotArea.DataArea, 1);

         _pivotResult = ChartDataHelperForSpecs.CreatePivotResult(_pivotAnalysis, AggregationFunctions.ValuesAggregation, _genderField, _raceField, _bmiField, _cmaxField);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_one_pane_for_each_unique_grouping_value()
      {
         _chartData.ShouldNotBeNull();
         _chartData.Panes.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_added_a_default_series_for_each_pane_if_data_are_available()
      {
         foreach (var pane in _chartData.Panes)
         {
            //only one series for each pane according to data
            pane.Curves.Count.ShouldBeEqualTo(1);
         }
      }
   }
}