using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateBoxWhiskerAnalysisPresenter : ContextSpecification<CreateBoxWhiskerAnalysisPresenter>
   {
      protected ICreatePopulationAnalysisView _view;
      protected ISubPresenterItemManager<IPopulationAnalysisItemPresenter> _subPresenterManager;
      private IDialogCreator _dialogCreator;
      protected IPopulationAnalysisParameterSelectionPresenter _parameterSelectionPresenter;
      protected IPopulationAnalysisPKParameterSelectionPresenter _pkParameterSpecificationPresenter;
      protected IPopulationAnalysisResultsPresenter _chartPresenter;
      protected IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      protected IPopulationAnalysisChartFactory _populationAnalysisChartFactory;
      protected PopulationBoxWhiskerAnalysis _analysis;
      protected BoxWhiskerAnalysisChart _boxWiskerAnalysisChart;
      protected IPopulationAnalysisTask _populationAnalysisTask;

      protected override void Context()
      {
         _view = A.Fake<ICreatePopulationAnalysisView>();
         _subPresenterManager = A.Fake<ISubPresenterItemManager<IPopulationAnalysisItemPresenter>>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _populationAnalysisTemplateTask = A.Fake<IPopulationAnalysisTemplateTask>();
         _populationAnalysisChartFactory = A.Fake<IPopulationAnalysisChartFactory>();
         _subPresenterManager = SubPresenterHelper.Create<IPopulationAnalysisItemPresenter>();
         _parameterSelectionPresenter = _subPresenterManager.CreateFake(BoxWhiskerItems.ParameterSelection);
         _pkParameterSpecificationPresenter = _subPresenterManager.CreateFake(BoxWhiskerItems.PKParameterSpecification);
         _chartPresenter = _subPresenterManager.CreateFake(BoxWhiskerItems.Chart);
         _analysis = new PopulationBoxWhiskerAnalysis();
         _boxWiskerAnalysisChart = new BoxWhiskerAnalysisChart {PopulationAnalysis = _analysis};
         A.CallTo(_populationAnalysisChartFactory).WithReturnType<BoxWhiskerAnalysisChart>().Returns(_boxWiskerAnalysisChart);
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();

         sut = new CreateBoxWhiskerAnalysisPresenter(_view, _subPresenterManager, _dialogCreator, _populationAnalysisTemplateTask, _populationAnalysisChartFactory, _populationAnalysisTask);
      }
   }

   public class When_creating_a_BoxWhisker_for_a_given_population_simulation : concern_for_CreateBoxWhiskerAnalysisPresenter
   {
      private IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
      }

      protected override void Because()
      {
         sut.Create(_populationDataCollector);
      }

      [Observation]
      public void should_set_the_population_simulation()
      {
         A.CallTo(() => _parameterSelectionPresenter.StartAnalysis(_populationDataCollector, _analysis)).MustHaveHappened();
         A.CallTo(() => _pkParameterSpecificationPresenter.StartAnalysis(_populationDataCollector, _analysis)).MustHaveHappened();
         A.CallTo(() => _chartPresenter.StartAnalysis(_populationDataCollector, _analysis)).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_population_chart_factory_to_create_a_new_population_analysis_chart()
      {
         sut.PopulationAnalysisChart.ShouldBeEqualTo(_boxWiskerAnalysisChart);
      }

      [Observation]
      public void should_set_analysis_and_the_analyzable_into_the_chart()
      {
         sut.PopulationAnalysisChart.Analysable.ShouldBeEqualTo(_populationDataCollector);
         sut.PopulationAnalysisChart.PopulationAnalysis.ShouldBeEqualTo(_analysis);
      }

      [Observation]
      public void should_update_the_origin_text()
      {
         A.CallTo(() => _populationAnalysisTask.SetOriginText(_boxWiskerAnalysisChart, _populationDataCollector.Name)).MustHaveHappened();
      }
   }

   public class When_creating_a_BoxWhisker_for_a_given_population_simulation_and_the_user_cancels_the_action : concern_for_CreateBoxWhiskerAnalysisPresenter
   {
      private IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         sut.Create(_populationDataCollector);
      }

      [Observation]
      public void should_not_update_the_origin_text()
      {
         A.CallTo(() => _populationAnalysisTask.SetOriginText(A<PopulationAnalysisChart>._, A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_loading_a_population_analysis : concern_for_CreateBoxWhiskerAnalysisPresenter
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationBoxWhiskerAnalysis _loadedAnalysis;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _loadedAnalysis = A.Fake<PopulationBoxWhiskerAnalysis>();
         sut.Create(_populationDataCollector);
         A.CallTo(() => _populationAnalysisTemplateTask.LoadPopulationAnalysisFor<PopulationBoxWhiskerAnalysis>(_populationDataCollector)).Returns(_loadedAnalysis);
      }

      protected override void Because()
      {
         sut.LoadAnalysis();
      }

      [Observation]
      public void should_leverate_the_templating_task_to_retrieve_a_population_analysis_from_template()
      {
         A.CallTo(() => _populationAnalysisTemplateTask.LoadPopulationAnalysisFor<PopulationBoxWhiskerAnalysis>(_populationDataCollector)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_loaded_analyses_as_current_analysis_in_the_underlying_chart()
      {
         sut.PopulationAnalysisChart.PopulationAnalysis.ShouldBeEqualTo(_loadedAnalysis);
      }
   }
}