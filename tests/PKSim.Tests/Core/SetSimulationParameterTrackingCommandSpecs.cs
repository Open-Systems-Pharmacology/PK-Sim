using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Events;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SetSimulationParameterTrackingCommand : ContextSpecification<SetSimulationParameterTrackingCommand>
   {
      protected IExecutionContext _executionContext;
      protected IndividualSimulation _simulation;
      protected const string _lipophilicityPath = "Organism|Aspirin|Lipophilicity";
      protected const string _permeabilityPath = "Organism|Aspirin|Permeability";

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _simulation = new IndividualSimulation { Id = "SimId", Name = "Sim" };
         A.CallTo(() => _executionContext.Get<Simulation>(_simulation.Id)).Returns(_simulation);

         _simulation.ParameterChangeTracker.Track(_lipophilicityPath);
         _simulation.ParameterChangeTracker.Track(_permeabilityPath);

         sut = new SetSimulationParameterTrackingCommand(_simulation, new[] { _lipophilicityPath, _permeabilityPath }, tracked: false);
      }
   }

   public class When_untracking_committed_simulation_parameters : concern_for_SetSimulationParameterTrackingCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_untrack_the_committed_paths()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeFalse();
      }

      [Observation]
      public void should_publish_a_simulation_status_changed_event()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationStatusChangedEvent>.That.Matches(x => x.Simulation == _simulation))).MustHaveHappened();
      }

      [Observation]
      public void should_not_be_shown_in_the_history()
      {
         sut.Visible.ShouldBeFalse();
      }
   }

   public class When_undoing_the_untracking_of_committed_simulation_parameters : concern_for_SetSimulationParameterTrackingCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_track_the_committed_paths_again()
      {
         _simulation.ParameterChangeTracker.IsTracked(_lipophilicityPath).ShouldBeTrue();
         _simulation.ParameterChangeTracker.IsTracked(_permeabilityPath).ShouldBeTrue();
      }

      [Observation]
      public void should_publish_a_simulation_status_changed_event_for_the_undo_as_well()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationStatusChangedEvent>.That.Matches(x => x.Simulation == _simulation))).MustHaveHappenedTwiceExactly();
      }
   }

   public class When_undoing_the_untracking_of_a_path_that_was_not_tracked : concern_for_SetSimulationParameterTrackingCommand
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Untrack(_permeabilityPath);
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_only_restore_the_path_whose_tracking_state_was_changed_by_the_command()
      {
         _simulation.ParameterChangeTracker.IsTracked(_lipophilicityPath).ShouldBeTrue();
         _simulation.ParameterChangeTracker.IsTracked(_permeabilityPath).ShouldBeFalse();
      }
   }
}
