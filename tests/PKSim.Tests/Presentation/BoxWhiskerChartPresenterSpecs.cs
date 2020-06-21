using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;


namespace PKSim.Presentation
{
   public abstract class concern_for_BoxWhiskerChartPresenter : ContextSpecification<BoxWhiskerChartPresenter>
   {
      private IBoxWhiskerChartView _view;
      private IPopulationAnalysisChartSettingsPresenter _chartSettingsPresenter;
      protected IIndividualExtractor _individualExtractor;
      private IObjectTypeResolver _objectTypeResolver;
      private ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;
      protected BoxWhiskerAnalysisChart _boxWhiskerAnalysisChart;
      private IApplicationSettings _applicationSettings;

      protected override void Context()
      {
         _view = A.Fake<IBoxWhiskerChartView>();
         _chartSettingsPresenter = A.Fake<IPopulationAnalysisChartSettingsPresenter>();
         _individualExtractor = A.Fake<IIndividualExtractor>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _applicationSettings= A.Fake<IApplicationSettings>();
         sut = new BoxWhiskerChartPresenter(_view, _chartSettingsPresenter, _applicationSettings, _individualExtractor, _objectTypeResolver);

         _chartData = A.Fake<ChartData<BoxWhiskerXValue, BoxWhiskerYValue>>();
         _boxWhiskerAnalysisChart = new BoxWhiskerAnalysisChart();

         sut.Show(_chartData, _boxWhiskerAnalysisChart);
      }
   }

   public class When_the_box_whisker_chart_presenter_is_extracting_individuals_based_on_some_box_whisker_curve_data : concern_for_BoxWhiskerChartPresenter
   {
      private BoxWhiskerYValue _boxWhiskerYValue;
      private List<int> _allIndividualIds;
      private Population _population;

      protected override void Context()
      {
         base.Context();
         _boxWhiskerYValue = new BoxWhiskerYValue
         {
            UpperWhisker = new ValueWithIndividualId(12) {IndividualId = 8},
            UpperBox = new ValueWithIndividualId(10) {IndividualId = 2},
            Median = new ValueWithIndividualId(8) {IndividualId = 14},
            LowerBox = new ValueWithIndividualId(7) {IndividualId = 22},
            LowerWhisker = new ValueWithIndividualId(2) {IndividualId = 44}
         };

         _population = new ImportPopulation();
         var populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => populationSimulation.Population).Returns(_population);
         _boxWhiskerAnalysisChart.Analysable = populationSimulation;

         A.CallTo(() => _individualExtractor.ExtractIndividualsFrom(_population, A<IEnumerable<int>>._))
            .Invokes(x => _allIndividualIds = x.GetArgument<IEnumerable<int>>(1).ToList());
      }

      protected override void Because()
      {
         sut.ExtractIndividuals(_boxWhiskerYValue);
      }

      [Observation]
      public void should_extract_the_individual_ids_and_call_the_individual_extractor_with_those_ids()
      {
         _allIndividualIds.ShouldOnlyContain(8, 2, 14, 22, 44);
      }
   }

   public class When_the_box_whisker_chart_presenter_is_extracting_individuals_based_on_some_box_whisker_curve_data_for_an_analysis_that_is_not_a_population_simulation : concern_for_BoxWhiskerChartPresenter
   {
      private BoxWhiskerYValue _boxWhiskerYValue;

      protected override void Context()
      {
         base.Context();
         _boxWhiskerYValue = new BoxWhiskerYValue();
         _boxWhiskerAnalysisChart.Analysable = A.Fake<IAnalysable>();
      }

      [Observation]
      public void should_thrown_an_exception_informing_the_user_that_this_feature_is_only_available_for_population_simulation_analysis()
      {
         The.Action(() => sut.ExtractIndividuals(_boxWhiskerYValue)).ShouldThrowAn<PKSimException>();
      }
   }
}