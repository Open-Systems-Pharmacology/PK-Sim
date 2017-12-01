using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Services;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationComparisonCreator : ContextSpecification<ISimulationComparisonCreator>
   {
      protected IPKSimChartFactory _chartFactory;
      private IContainerTask _containerTask;
      protected PKSimProject _project;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IApplicationController _applicationController;
      protected ISingleStartPresenterTask _singleStartPresenterTask;
      protected IExecutionContext _executionContext;
      protected ISimulationAnalysisCreator _simulationAnalysisCreator;

      protected override void Context()
      {
         _chartFactory = A.Fake<IPKSimChartFactory>();
         _containerTask = A.Fake<IContainerTask>();
         _applicationController = A.Fake<IApplicationController>();
         _project = new PKSimProject();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _singleStartPresenterTask = A.Fake<ISingleStartPresenterTask>();
         _executionContext = A.Fake<IExecutionContext>();
         _simulationAnalysisCreator = A.Fake<ISimulationAnalysisCreator>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         sut = new SimulationComparisonCreator(_chartFactory, _containerTask, _objectBaseFactory,
            _applicationController, _singleStartPresenterTask, _executionContext, _simulationAnalysisCreator);
      }
   }

   public class When_creating_a_summary_chart : concern_for_SimulationComparisonCreator
   {
      private IndividualSimulationComparison _individualSimulationComparison;
      private SimulationComparisonCreatedEvent _event;

      protected override void Context()
      {
         base.Context();
         _individualSimulationComparison = new IndividualSimulationComparison();
         A.CallTo(() => _chartFactory.CreateSummaryChart()).Returns(_individualSimulationComparison);
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationComparisonCreatedEvent>.Ignored)).Invokes(
            x => _event = x.GetArgument<SimulationComparisonCreatedEvent>(0));
      }

      protected override void Because()
      {
         sut.CreateIndividualSimulationComparison();
      }

      [Observation]
      public void should_create_a_new_summary_chart_and_add_it_to_the_project()
      {
         _project.AllSimulationComparisons.ShouldContain(_individualSimulationComparison);
      }

      [Observation]
      public void should_notify_that_a_summary_chart_was_created()
      {
         _event.SimulationComparison.ShouldBeEqualTo(_individualSimulationComparison);
         _event.Project.ShouldBeEqualTo(_project);
      }
   }

   public class When_creating_a_population_simulation_comparison : concern_for_SimulationComparisonCreator
   {
      private PopulationSimulationComparison _result;
      private ISimulationSelectionForComparisonPresenter _selectionPresenter;
      private PopulationSimulationComparison _populationSimulationComparison;

      protected override void Context()
      {
         base.Context();
         _selectionPresenter = A.Fake<ISimulationSelectionForComparisonPresenter>();
         A.CallTo(() => _applicationController.Start<ISimulationSelectionForComparisonPresenter>()).Returns(_selectionPresenter);
         _populationSimulationComparison = new PopulationSimulationComparison();
         A.CallTo(() => _objectBaseFactory.Create<PopulationSimulationComparison>()).Returns(_populationSimulationComparison);
         A.CallTo(() => _selectionPresenter.Edit(_populationSimulationComparison)).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.CreatePopulationSimulationComparison().DowncastTo<PopulationSimulationComparison>();
      }

      [Observation]
      public void should_create_and_edit_a_new_comparison()
      {
         A.CallTo(() => _selectionPresenter.Edit(_populationSimulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_created_comparison_to_the_project()
      {
         _project.AllSimulationComparisons.ShouldContain(_populationSimulationComparison);
      }

      [Observation]
      public void should_return_the_created_simulation()
      {
         _result.ShouldBeEqualTo(_populationSimulationComparison);
      }

      [Observation]
      public void should_create_a_population_analysis_for_the_comparison()
      {
         A.CallTo(() => _simulationAnalysisCreator.CreatePopulationAnalysisFor(_populationSimulationComparison)).MustHaveHappened();
      }
   }

   public class When_configuring_a_population_simulation_comparison_that_was_being_edited : concern_for_SimulationComparisonCreator
   {
      private PopulationSimulationComparison _populationSimulationComparison;
      private ISimulationSelectionForComparisonPresenter _selectionPresenter;

      protected override void Context()
      {
         base.Context();
         _selectionPresenter = A.Fake<ISimulationSelectionForComparisonPresenter>();
         A.CallTo(() => _applicationController.Start<ISimulationSelectionForComparisonPresenter>()).Returns(_selectionPresenter);
         _populationSimulationComparison = new PopulationSimulationComparison();
         A.CallTo(() => _selectionPresenter.Edit(_populationSimulationComparison)).Returns(true);
         A.CallTo(() => _applicationController.HasPresenterOpenedFor(_populationSimulationComparison)).Returns(true);
      }

      protected override void Because()
      {
         sut.ConfigurePopulationSimulationComparison(_populationSimulationComparison);
      }

      [Observation]
      public void should_edit_the_given_population_comparison()
      {
         A.CallTo(() => _selectionPresenter.Edit(_populationSimulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_a_project_change()
      {
         A.CallTo(() => _executionContext.ProjectChanged()).MustHaveHappened();
      }

      [Observation]
      public void should_close_the_edited_configuration()
      {
         A.CallTo(() => _applicationController.Close(_populationSimulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_comparison_again()
      {
         A.CallTo(() => _singleStartPresenterTask.StartForSubject(_populationSimulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_comparison()
      {
         A.CallTo(() => _executionContext.Load(_populationSimulationComparison)).MustHaveHappened();
      }
   }
}