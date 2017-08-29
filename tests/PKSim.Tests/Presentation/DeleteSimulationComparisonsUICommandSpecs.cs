using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.UICommands;

using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_DeleteSimulationComparisonsUICommand : ContextSpecification<DeleteSimulationComparisonsUICommand>
   {
      protected IDialogCreator _dialogCreator;
      private IApplicationController _applicationController;
      private IWorkspace _workspace;
      protected IEventPublisher _eventPublisher;
      protected IndividualSimulationComparison _individualSimulationComparison;
      protected PKSimProject _project;
      protected IRegistrationTask _registrationTask;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _applicationController = A.Fake<IApplicationController>();
         _workspace = A.Fake<IWorkspace>();
         _project = A.Fake<PKSimProject>();
         _registrationTask= A.Fake<IRegistrationTask>();
         A.CallTo(() => _workspace.Project).Returns(_project);
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new DeleteSimulationComparisonsUICommand(_applicationController, _workspace, _eventPublisher, _dialogCreator, _registrationTask);
         _individualSimulationComparison = new IndividualSimulationComparison().WithName("chart");
         sut.For(new [] {_individualSimulationComparison});
      }

      protected override void Because()
      {
         sut.Execute();
      }
   }

   public class When_the_user_decides_to_delete_a_summary_plot : concern_for_DeleteSimulationComparisonsUICommand
   {
      [Observation]
      public void the_user_should_be_asked_to_confirm_the_deletion()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(new[] { _individualSimulationComparison.Name }))).MustHaveHappened();
      }
   }

   public class When_the_user_decides_to_delete_a_summary_plot_and_cancel_the_deletion : concern_for_DeleteSimulationComparisonsUICommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(new[] { _individualSimulationComparison.Name }))).Returns(ViewResult.No);
      }

      [Observation]
      public void should_not_delete_the_summary_chart()
      {
         A.CallTo(() => _project.RemoveSimulationComparison(_individualSimulationComparison)).MustNotHaveHappened();
      }
   }

   public class When_the_user_decides_to_delete_a_summary_plot_and_confirn_the_deletion : concern_for_DeleteSimulationComparisonsUICommand
   {
      private SimulationComparisonDeletedEvent _chartDeletedEvent;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(new[] { _individualSimulationComparison.Name }))).Returns(ViewResult.Yes);
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationComparisonDeletedEvent>.Ignored)).Invokes(
            x => _chartDeletedEvent = x.GetArgument<SimulationComparisonDeletedEvent>(0));
      }

      [Observation]
      public void should_delete_the_summary_chart()
      {
         A.CallTo(() => _project.RemoveSimulationComparison(_individualSimulationComparison)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_the_summary_chart_deletion()
      {
         _chartDeletedEvent.Chart.ShouldBeEqualTo(_individualSimulationComparison);
      }

      [Observation]
      public void should_unregister_the_comparison()
      {
         A.CallTo(() => _registrationTask.Unregister(_individualSimulationComparison)).MustHaveHappened();
      }
   }
}