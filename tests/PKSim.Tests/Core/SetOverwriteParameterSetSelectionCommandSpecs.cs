using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SetOverwriteParameterSetSelectionCommand : ContextSpecification<SetOverwriteParameterSetSelectionCommand>
   {
      protected IExecutionContext _executionContext;
      protected IndividualSimulation _simulation;
      protected Compound _simulationCompound;
      protected OverwriteParameterSet _humanSet;
      protected OverwriteParameterSet _ratSet;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };
         _humanSet = new OverwriteParameterSet { Name = "Human", Id = "HumanId" };
         _ratSet = new OverwriteParameterSet { Name = "Rat", Id = "RatId" };
         _simulationCompound.AddOverwriteParameterSet(_humanSet);
         _simulationCompound.AddOverwriteParameterSet(_ratSet);

         _simulation = new IndividualSimulation { Id = "SimId", Name = "Sim" };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateCompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _simulationCompound });
         A.CallTo(() => _executionContext.Get<Simulation>(_simulation.Id)).Returns(_simulation);

         sut = new SetOverwriteParameterSetSelectionCommand(_simulation, "Aspirin", "Rat");
      }
   }

   public class When_selecting_an_overwrite_parameter_set_for_a_compound_in_a_simulation : concern_for_SetOverwriteParameterSetSelectionCommand
   {
      protected override void Context()
      {
         base.Context();
         _simulation.AddOverwriteParameterSetSelection("Aspirin", _humanSet);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_select_the_set_of_the_compound_used_in_the_simulation()
      {
         _simulation.OverwriteParameterSetSelections.SelectedSetFor("Aspirin").ShouldBeEqualTo(_ratSet);
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

   public class When_selecting_no_overwrite_parameter_set_for_a_compound_in_a_simulation : concern_for_SetOverwriteParameterSetSelectionCommand
   {
      protected override void Context()
      {
         base.Context();
         _simulation.AddOverwriteParameterSetSelection("Aspirin", _humanSet);
         sut = new SetOverwriteParameterSetSelectionCommand(_simulation, "Aspirin", null);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_store_an_explicit_selection_of_no_set()
      {
         _simulation.OverwriteParameterSetSelections.HasSelectionFor("Aspirin").ShouldBeTrue();
         _simulation.OverwriteParameterSetSelections.SelectedSetFor("Aspirin").ShouldBeNull();
      }
   }

   public class When_undoing_the_selection_of_an_overwrite_parameter_set_that_replaced_another_set : concern_for_SetOverwriteParameterSetSelectionCommand
   {
      protected override void Context()
      {
         base.Context();
         _simulation.AddOverwriteParameterSetSelection("Aspirin", _humanSet);
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_select_the_previous_set_again()
      {
         _simulation.OverwriteParameterSetSelections.SelectedSetFor("Aspirin").ShouldBeEqualTo(_humanSet);
      }
   }

   public class When_undoing_the_selection_of_an_overwrite_parameter_set_for_a_compound_explicitly_using_no_set : concern_for_SetOverwriteParameterSetSelectionCommand
   {
      protected override void Context()
      {
         base.Context();
         _simulation.AddOverwriteParameterSetSelection("Aspirin", null);
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_the_explicit_selection_of_no_set()
      {
         _simulation.OverwriteParameterSetSelections.HasSelectionFor("Aspirin").ShouldBeTrue();
         _simulation.OverwriteParameterSetSelections.SelectedSetFor("Aspirin").ShouldBeNull();
      }
   }

   public class When_undoing_the_selection_of_an_overwrite_parameter_set_for_a_compound_without_any_selection : concern_for_SetOverwriteParameterSetSelectionCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_remove_the_selection_again()
      {
         _simulation.OverwriteParameterSetSelections.HasSelectionFor("Aspirin").ShouldBeFalse();
      }
   }

   public class When_redoing_the_selection_of_an_overwrite_parameter_set_for_a_compound_without_any_selection : concern_for_SetOverwriteParameterSetSelectionCommand
   {
      protected override void Because()
      {
         var undo = sut.ExecuteAndInvokeInverse(_executionContext);
         undo.DowncastTo<IReversibleCommand<IExecutionContext>>().InvokeInverse(_executionContext);
      }

      [Observation]
      public void should_select_the_set_again()
      {
         _simulation.OverwriteParameterSetSelections.SelectedSetFor("Aspirin").ShouldBeEqualTo(_ratSet);
      }
   }
}
