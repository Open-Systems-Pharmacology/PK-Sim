using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Data;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_BoxWhiskerChartsDataCreator : ContextSpecification<IBoxWhiskerChartDataCreator>
   {
      protected Aggregate<BoxWhiskerYValue> _aggregate;
      protected PopulationBoxWhiskerAnalysis _pivotAnalysis;

      protected readonly PopulationAnalysisCovariateField _genderField = new PopulationAnalysisCovariateField {Name = "GENDER", Covariate = "GENDER"};
      protected readonly PopulationAnalysisCovariateField _raceField = new PopulationAnalysisCovariateField {Name = "RACE", Covariate = "RACE"};
      protected readonly PopulationAnalysisParameterField _bmiField = new PopulationAnalysisParameterField {Name = "BMI", ParameterPath = "BMI"};
      protected readonly PopulationAnalysisPKParameterField _cmaxField = new PopulationAnalysisPKParameterField {Name = "Cmax", QuantityPath = "Path", PKParameter = "Cmax"};
      private IPopulationAnalysisField _bmiClass;
      private IDimensionRepository _dimensionRepository;
      private IPivotResultCreator _pivotResultCreator;

      protected override void Context()
      {
         _pivotAnalysis = new PopulationBoxWhiskerAnalysis();
         _aggregate = AggregationFunctions.BoxWhisker90Aggregation;
         _pivotAnalysis.Add(_genderField);
         _pivotAnalysis.Add(_raceField);
         _pivotAnalysis.Add(_bmiField);
         _pivotAnalysis.Add(_cmaxField);
         var grouping = new FixedLimitsGroupingDefinition(_bmiField.Name);
         grouping.SetLimits(new[] {25d}.OrderBy(x => x));
         grouping.AddItems(PopulationAnalysisHelperForSpecs.BMIGroups);
         _bmiClass = new PopulationAnalysisGroupingField(grouping) {Name = "BMI class"};
         _pivotAnalysis.Add(_bmiClass);
         _pivotAnalysis.SetPosition(_genderField, PivotArea.RowArea, 0);
         _pivotAnalysis.SetPosition(_raceField, PivotArea.RowArea, 1);
         _pivotAnalysis.SetPosition(_bmiClass, PivotArea.ColumnArea, 0);
         _pivotAnalysis.SetPosition(_cmaxField, PivotArea.DataArea, 0);
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _pivotResultCreator= A.Fake<IPivotResultCreator>();
         sut = new BoxWhiskerChartDataCreator(_dimensionRepository,_pivotResultCreator);
      }

      protected void CheckSeriesItem(CurveData<BoxWhiskerXValue, BoxWhiskerYValue> curve, int index, string[] xValue, float median)
      {
         var boxWhiskerXValue = curve.XValues[index];
         boxWhiskerXValue.Count.ShouldBeEqualTo(xValue.Length);
         for (int i = 0; i < boxWhiskerXValue.Count; i++)
         {
            boxWhiskerXValue[i].ShouldBeEqualTo(xValue[i]);
         }

         curve.YValues[index].Median.ShouldBeEqualTo(median);
      }
   }

   public class When_creating_the_box_whisker_chart_data_based_on_row_data : concern_for_BoxWhiskerChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _pivotResult = ChartDataHelperForSpecs.CreatePivotResult(_pivotAnalysis, _aggregate, _genderField, _raceField, _bmiField, _cmaxField);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void the_returned_plot_should_have_the_x_grouping_set_according_to_the_fields_defined_in_the_row_area()
      {
         _chartData.XFieldNames.ShouldOnlyContain(_genderField.Name, _raceField.Name);
      }

      [Observation]
      public void each_plot_item_created_should_have_the_expected_series_name()
      {
         var pane = _chartData.Panes["Cmax"];
         pane.Curves.Count.ShouldBeEqualTo(2);

         //Check ordering accorting to field odering thin then big
         var thinSerie = pane.Curves.ElementAt(0);
         thinSerie.Id.ShouldBeEqualTo("Thin");

         var bigSerie = pane.Curves.ElementAt(1);
         bigSerie.Id.ShouldBeEqualTo("Big");

         //Only {Big, Male, EU}
         bigSerie.XValues.Count.ShouldBeEqualTo(1);

         //{Thin, Male, US} and {Thin, Female, EU}
         thinSerie.XValues.Count.ShouldBeEqualTo(2);

         CheckSeriesItem(bigSerie, 0, new[] {"Male", "EU"}, 1000);

         CheckSeriesItem(thinSerie, 0, new[] {"Male", "US"}, 900);
         CheckSeriesItem(thinSerie, 1, new[] {"Female", "EU"}, 600);
      }
   }

   public class When_creating_a_box_whisker_analysis_removing_the_outliers : concern_for_BoxWhiskerChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis.ShowOutliers = false;
         _pivotResult = ChartDataHelperForSpecs.CreatePivotResult(_pivotAnalysis, _aggregate, _genderField, _raceField, _bmiField, _cmaxField);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_remove_the_outliers_from_the_calculated_values()
      {
         var pane = _chartData.Panes["Cmax"];
         var thinSeries = pane.Curves.ElementAt(0);

         foreach (var value in thinSeries.YValues)
         {
            value.Outliers.Length.ShouldBeEqualTo(0);
         }
      }
   }
}