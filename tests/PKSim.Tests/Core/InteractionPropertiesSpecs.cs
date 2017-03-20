using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_InteractionProperties : ContextSpecification<InteractionProperties>
   {
      protected override void Context()
      {
         sut = new InteractionProperties();
      }
   }

   public class When_checking_if_interactions_are_defined : concern_for_InteractionProperties
   {
      private InteractionSelection _interaction;

      protected override void Context()
      {
         base.Context();
         _interaction = new InteractionSelection {MoleculeName = "CYP3A4"};
      }

      [Observation]
      public void should_return_true_if_at_least_one_interaction_exists()
      {
         sut.AddInteraction(_interaction);
         sut.Any().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         sut.Any().ShouldBeFalse();
      }
   }

   public class When_returning_the_list_of_interaction_molecules : concern_for_InteractionProperties
   {
      protected override void Context()
      {
         base.Context();
         sut.AddInteraction(new InteractionSelection {MoleculeName = "CYP3A4", ProcessName = "P1"});
         sut.AddInteraction(new InteractionSelection { MoleculeName = "CYP2D6", ProcessName = "P1" });
         sut.AddInteraction(new InteractionSelection { MoleculeName = "AUD", ProcessName = "P1" });
         sut.AddInteraction(new InteractionSelection { MoleculeName = "CYP3A4", ProcessName = "P1" });
         sut.AddInteraction(new InteractionSelection { MoleculeName = "XXX"});
      }

      [Observation]
      public void should_return_the_distinct_list_of_molecules_involved_in_well_defined_interaction_processes()
      {
         sut.InteractingMoleculeNames.ShouldOnlyContain("CYP3A4", "CYP2D6", "AUD");
      }
   }

   public class When_checking_if_an_interaction_process_is_used_in_the_interaction_properties : concern_for_InteractionProperties
   {
      private InteractionProcess _interactionProcess;

      protected override void Context()
      {
         base.Context();
         _interactionProcess = new InhibitionProcess();
      }

      [Observation]
      public void should_return_false_if_no_interaciton_is_defined()
      {
         sut.Uses(_interactionProcess).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_at_least_one_interaction_matches_the_process_properties()
      {
         var interaction= A.Fake<InteractionSelection>();
         A.CallTo(() => interaction.Matches(_interactionProcess)).Returns(true);
         sut.AddInteraction(interaction);
         sut.Uses(_interactionProcess).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_no_interaction_matches_the_process_properties()
      {
         var interaction = A.Fake<InteractionSelection>();
         A.CallTo(() => interaction.Matches(_interactionProcess)).Returns(false);
         sut.AddInteraction(interaction);
         sut.Uses(_interactionProcess).ShouldBeFalse();
      }
   }
}	