using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_InteractionSelection : ContextSpecification<InteractionSelection>
   {
      protected override void Context()
      {
         sut = new InteractionSelection{CompoundName = "Comp", MoleculeName = "Mol", ProcessName = "Proc"};
      }
   }

   public class When_cloning_an_interaction_process : concern_for_InteractionSelection
   {
      private ICloneManager _cloneManager;
      private InteractionSelection _clone;

      protected override void Context()
      {
         base.Context();
         _cloneManager= A.Fake<ICloneManager>();
      }

      protected override void Because()
      {
         _clone = sut.Clone(_cloneManager);
      }

      [Observation]
      public void should_return_a_new_interaction_process_having_the_same_properties_as_the_original()
      {
         _clone.MoleculeName.ShouldBeEqualTo(sut.MoleculeName);
         _clone.ProcessName.ShouldBeEqualTo(sut.ProcessName);
         _clone.CompoundName.ShouldBeEqualTo(sut.CompoundName);   
      }
   }

   public class When_checking_if_an_interaction_selection_matches_an_interaction_process : concern_for_InteractionSelection
   {
      private InteractionProcess _interactionProcess;
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithName("Comp");
         _interactionProcess = new InhibitionProcess { ParentContainer = _compound }.WithName("Proc");
      }
      [Observation]
      public void should_return_true_if_the_compound_names_and_process_names_are_the_same()
      {
         sut.Matches(_interactionProcess).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_only_the_compound_names_are_the_same()
      {
         _interactionProcess.Name = "XXX";
         sut.Matches(_interactionProcess).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_only_the_process_names_are_the_same()
      {
         _compound.Name = "XXX";
         sut.Matches(_interactionProcess).ShouldBeFalse();

      }

      [Observation]
      public void should_return_false_otherwise()
      {
         _compound.Name = "XXX";
         _interactionProcess.Name = "YYY";
         sut.Matches(_interactionProcess).ShouldBeFalse();
      }
   }
}	