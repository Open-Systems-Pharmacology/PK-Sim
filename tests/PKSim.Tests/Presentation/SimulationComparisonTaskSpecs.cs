using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Services;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationComparisonTask : ContextSpecification<ISimulationComparisonTask>
   {
      protected IPKSimChartFactory _chartFactory;
      private IContainerTask _containerTask;
      protected PKSimProject _project;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IApplicationController _applicationController;
      protected ISingleStartPresenterTask _singleStartPresenterTask;
      protected IExecutionContext _executionContext;
      protected ISimulationAnalysisCreator _simulationAnalysisCreator;
      protected IDialogCreator _dialogCreator;
      protected IndividualSimulationComparison _individualSimulationComparison;
      protected SimulationComparisonMapper _simulationComparisonMapper;

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
         _dialogCreator = A.Fake<IDialogCreator>();
         _simulationComparisonMapper = A.Fake<SimulationComparisonMapper>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);

         _individualSimulationComparison = new IndividualSimulationComparison().WithName("chart");

         sut = new SimulationComparisonTask(
            _chartFactory,
            _containerTask,
            _objectBaseFactory,
            _applicationController,
            _singleStartPresenterTask,
            _executionContext,
            _simulationAnalysisCreator,
            _dialogCreator,
            _simulationComparisonMapper
         );
      }
   }

   public class When_creating_an_individual_simulation_comparison : concern_for_SimulationComparisonTask
   {
      private SimulationComparisonCreatedEvent _event;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _chartFactory.CreateIndividualSimulationComparison()).Returns(_individualSimulationComparison);
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

   public class When_creating_a_population_simulation_comparison : concern_for_SimulationComparisonTask
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

   public class When_configuring_a_population_simulation_comparison_that_was_being_edited : concern_for_SimulationComparisonTask
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

   public class When_the_user_decides_to_delete_some_simulation_comparisons : concern_for_SimulationComparisonTask
   {
      protected override void Because()
      {
         sut.Delete(new[] {_individualSimulationComparison,});
      }

      [Observation]
      public void the_user_should_be_asked_to_confirm_the_deletion()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(new[] {_individualSimulationComparison.Name}), ViewResult.Yes)).MustHaveHappened();
      }
   }

   public class When_the_user_decides_to_delete_some_simulation_comparisons_and_cancel_the_deletion : concern_for_SimulationComparisonTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(new[] {_individualSimulationComparison.Name}), ViewResult.Yes)).Returns(ViewResult.No);
         _project.AddSimulationComparison(_individualSimulationComparison);
      }

      protected override void Because()
      {
         sut.Delete(new[] {_individualSimulationComparison,});
      }

      [Observation]
      public void should_not_delete_the_summary_chart()
      {
         _project.AllSimulationComparisons.ShouldContain(_individualSimulationComparison);
      }
   }

   public class When_the_user_decides_to_delete_some_simulation_comparisons_and_confirm_the_deletion : concern_for_SimulationComparisonTask
   {
      private SimulationComparisonDeletedEvent _comparisonDeletedEvent;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(new[] {_individualSimulationComparison.Name}), ViewResult.Yes)).Returns(ViewResult.Yes);
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationComparisonDeletedEvent>.Ignored)).Invokes(
            x => _comparisonDeletedEvent = x.GetArgument<SimulationComparisonDeletedEvent>(0));
         _project.AddSimulationComparison(_individualSimulationComparison);
      }

      protected override void Because()
      {
         sut.Delete(new[] {_individualSimulationComparison,});
      }

      [Observation]
      public void should_delete_the_simulation_comparison()
      {
         _project.AllSimulationComparisons.ShouldNotContain(_individualSimulationComparison);
      }

      [Observation]
      public void should_notify_the_summary_chart_deletion()
      {
         _comparisonDeletedEvent.Chart.ShouldBeEqualTo(_individualSimulationComparison);
      }

      [Observation]
      public void should_unregister_the_comparison()
      {
         A.CallTo(() => _executionContext.Unregister(_individualSimulationComparison)).MustHaveHappened();
      }
   }

   public class When_cloning_a_simulation_comparison : concern_for_SimulationComparisonTask
   {
      private ISimulationComparison _clone;

      protected override void Context()
      {
         base.Context();
         var snapshot  =new SimulationComparison();
         _clone =new IndividualSimulationComparison();
         A.CallTo(() => _simulationComparisonMapper.MapToSnapshot(_individualSimulationComparison)).Returns(snapshot);
         A.CallTo(() => _simulationComparisonMapper.MapToModel(snapshot, A<SnapshotContext>._)).Returns(_clone);
      }

      [Observation]
      public async Task should_use_the_snapshot_to_create_a_snapshot_and_a_brand_new_instance_of_the_comparison()
      {
         var clone =  await sut.CloneSimulationComparision(_individualSimulationComparison);
         clone.ShouldBeEqualTo(_clone);
      }
   }
}