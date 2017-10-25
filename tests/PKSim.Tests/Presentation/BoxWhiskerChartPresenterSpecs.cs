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
      private IObjectTypeResolver _objetTypeResolver;
      private ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;
      protected BoxWhiskerAnalysisChart _boxWiskerAnalysisChart;
      private IApplicationSettings _applicationSettings;

      protected override void Context()
      {
         _view = A.Fake<IBoxWhiskerChartView>();
         _chartSettingsPresenter = A.Fake<IPopulationAnalysisChartSettingsPresenter>();
         _individualExtractor = A.Fake<IIndividualExtractor>();
         _objetTypeResolver = A.Fake<IObjectTypeResolver>();
         _applicationSettings= A.Fake<IApplicationSettings>();
         sut = new BoxWhiskerChartPresenter(_view, _chartSettingsPresenter, _applicationSettings, _individualExtractor, _objetTypeResolver);

         _chartData = A.Fake<ChartData<BoxWhiskerXValue, BoxWhiskerYValue>>();
         _boxWiskerAnalysisChart = new BoxWhiskerAnalysisChart();

         sut.Show(_chartData, _boxWiskerAnalysisChart);
      }
   }

   public class When_the_box_whisker_chart_presenter_is_extracting_individuals_based_on_some_box_whisker_curve_data : concern_for_BoxWhiskerChartPresenter
   {
      private BoxWhiskerYValue _boxWiskerYValue;
      private List<int> _allIndividualIds;
      private Population _population;

      protected override void Context()
      {
         base.Context();
         _boxWiskerYValue = new BoxWhiskerYValue
         {
            UpperWhisker = new ValueWithIndvividualId(12) {IndividualId = 8},
            UpperBox = new ValueWithIndvividualId(10) {IndividualId = 2},
            Median = new ValueWithIndvividualId(8) {IndividualId = 14},
            LowerBox = new ValueWithIndvividualId(7) {IndividualId = 22},
            LowerWhisker = new ValueWithIndvividualId(2) {IndividualId = 44}
         };

         _population = new ImportPopulation();
         var populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => populationSimulation.Population).Returns(_population);
         _boxWiskerAnalysisChart.Analysable = populationSimulation;

         A.CallTo(() => _individualExtractor.ExtractIndividualsFrom(_population, A<IEnumerable<int>>._))
            .Invokes(x => _allIndividualIds = x.GetArgument<IEnumerable<int>>(1).ToList());
      }

      protected override void Because()
      {
         sut.ExtractIndividuals(_boxWiskerYValue);
      }

      [Observation]
      public void should_extract_the_individual_ids_and_call_the_individual_extracter_with_those_ids()
      {
         _allIndividualIds.ShouldOnlyContain(8, 2, 14, 22, 44);
      }
   }

   public class When_the_box_whisker_chart_presenter_is_extracting_individuals_based_on_some_box_whisker_curve_data_for_an_analysis_that_is_not_a_population_simulation : concern_for_BoxWhiskerChartPresenter
   {
      private BoxWhiskerYValue _boxWiskerYValue;

      protected override void Context()
      {
         base.Context();
         _boxWiskerYValue = new BoxWhiskerYValue();
         _boxWiskerAnalysisChart.Analysable = A.Fake<IAnalysable>();
      }

      [Observation]
      public void should_thrown_an_exception_informing_the_user_that_this_feature_is_only_available_for_population_simulation_analysis()
      {
         The.Action(() => sut.ExtractIndividuals(_boxWiskerYValue)).ShouldThrowAn<PKSimException>();
      }
   }
}