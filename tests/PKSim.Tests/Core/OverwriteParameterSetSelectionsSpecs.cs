using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_OverwriteParameterSetSelections : ContextSpecification<OverwriteParameterSetSelections>
   {
      protected OverwriteParameterSet _renalImpairmentSet;
      protected OverwriteParameterSet _hepaticImpairmentSet;

      protected override void Context()
      {
         sut = new OverwriteParameterSetSelections();
         _renalImpairmentSet = new OverwriteParameterSet { Name = "RenalImpairment" };
         _hepaticImpairmentSet = new OverwriteParameterSet { Name = "HepaticImpairment" };
      }
   }

   public class When_setting_a_selection_for_a_compound : concern_for_OverwriteParameterSetSelections
   {
      protected override void Because()
      {
         sut.SetSelectionForCompound("Aspirin", _renalImpairmentSet);
      }

      [Observation]
      public void should_store_the_selection()
      {
         sut.SelectedSetFor("Aspirin").ShouldBeEqualTo(_renalImpairmentSet);
      }

      [Observation]
      public void should_have_one_selection()
      {
         sut.Selections.Count.ShouldBeEqualTo(1);
      }
   }

   public class When_updating_a_selection_for_an_existing_compound : concern_for_OverwriteParameterSetSelections
   {
      protected override void Context()
      {
         base.Context();
         sut.SetSelectionForCompound("Aspirin", _renalImpairmentSet);
      }

      protected override void Because()
      {
         sut.SetSelectionForCompound("Aspirin", _hepaticImpairmentSet);
      }

      [Observation]
      public void should_update_the_selection()
      {
         sut.SelectedSetFor("Aspirin").ShouldBeEqualTo(_hepaticImpairmentSet);
      }

      [Observation]
      public void should_not_add_a_duplicate_entry()
      {
         sut.Selections.Count.ShouldBeEqualTo(1);
      }
   }

   public class When_querying_a_selection_for_an_unknown_compound : concern_for_OverwriteParameterSetSelections
   {
      [Observation]
      public void should_return_null()
      {
         sut.SelectedSetFor("Unknown").ShouldBeNull();
      }
   }

   public class When_removing_a_selection : concern_for_OverwriteParameterSetSelections
   {
      protected override void Context()
      {
         base.Context();
         sut.SetSelectionForCompound("Aspirin", _renalImpairmentSet);
      }

      protected override void Because()
      {
         sut.RemoveSelectionForCompound("Aspirin");
      }

      [Observation]
      public void should_no_longer_have_the_selection()
      {
         sut.SelectedSetFor("Aspirin").ShouldBeNull();
      }

      [Observation]
      public void should_have_no_selections()
      {
         sut.Selections.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_cloning_selections : concern_for_OverwriteParameterSetSelections
   {
      private OverwriteParameterSetSelections _clone;
      private OverwriteParameterSet _caffeineSet;

      protected override void Context()
      {
         base.Context();
         _caffeineSet = new OverwriteParameterSet { Name = "HepaticImpairment" };
         sut.SetSelectionForCompound("Aspirin", _renalImpairmentSet);
         sut.SetSelectionForCompound("Caffeine", _caffeineSet);
      }

      protected override void Because()
      {
         _clone = sut.Clone();
      }

      [Observation]
      public void should_clone_all_selections()
      {
         _clone.Selections.Count.ShouldBeEqualTo(2);
         _clone.SelectedSetFor("Aspirin").ShouldBeEqualTo(_renalImpairmentSet);
         _clone.SelectedSetFor("Caffeine").ShouldBeEqualTo(_caffeineSet);
      }

      [Observation]
      public void should_create_independent_copy()
      {
         _clone.SetSelectionForCompound("Aspirin", _hepaticImpairmentSet);
         sut.SelectedSetFor("Aspirin").ShouldBeEqualTo(_renalImpairmentSet);
      }
   }
}
