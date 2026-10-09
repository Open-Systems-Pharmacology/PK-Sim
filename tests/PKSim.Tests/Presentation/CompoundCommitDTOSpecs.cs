using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundCommitDTO : ContextSpecification<CompoundCommitDTO>
   {
      protected override void Context()
      {
         sut = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = new Compound { Name = "Aspirin" },
            AvailableExistingSets = new List<OverwriteParameterSet>
            {
               new() { Name = "ExistingSet1" },
               new() { Name = "ExistingSet2" }
            },
            CreateNew = true,
            NewSetName = "NewSet",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true }
            }
         };
      }
   }

   public class When_validating_a_compound_commit_dto_with_valid_new_set_name : concern_for_CompoundCommitDTO
   {
      [Observation]
      public void should_be_valid()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_validating_a_compound_commit_dto_with_empty_new_set_name : concern_for_CompoundCommitDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateNew = true;
         sut.NewSetName = "";
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_a_compound_commit_dto_with_null_new_set_name : concern_for_CompoundCommitDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateNew = true;
         sut.NewSetName = null;
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_a_compound_commit_dto_with_duplicate_name : concern_for_CompoundCommitDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateNew = true;
         sut.NewSetName = "ExistingSet1";
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_a_compound_commit_dto_updating_while_no_set_is_selected_in_the_simulation : concern_for_CompoundCommitDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateNew = false;
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_not_offer_to_update_a_set()
      {
         sut.CanUpdateSetSelectedInSimulation.ShouldBeFalse();
      }
   }

   public abstract class concern_for_CompoundCommitDTO_updating_the_set_selected_in_the_simulation : ContextSpecification<CompoundCommitDTO>
   {
      protected OverwriteParameterSet _setSelectedInSimulation;

      protected override void Context()
      {
         _setSelectedInSimulation = new OverwriteParameterSet { Name = "SelectedSet" };
         sut = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = new Compound { Name = "Aspirin" },
            AvailableExistingSets = new List<OverwriteParameterSet> { _setSelectedInSimulation },
            SetSelectedInSimulation = _setSelectedInSimulation,
            CreateNew = false,
            NewSetName = "",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5 }
            }
         };
      }
   }

   public class When_validating_a_compound_commit_dto_updating_the_set_selected_in_the_simulation : concern_for_CompoundCommitDTO_updating_the_set_selected_in_the_simulation
   {
      [Observation]
      public void should_be_valid_even_with_empty_name()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_offer_to_update_the_selected_set()
      {
         sut.CanUpdateSetSelectedInSimulation.ShouldBeTrue();
      }
   }

   public class When_validating_a_compound_commit_dto_updating_the_selected_set_with_a_name_used_by_another_set : concern_for_CompoundCommitDTO_updating_the_set_selected_in_the_simulation
   {
      protected override void Context()
      {
         base.Context();
         sut.NewSetName = "SelectedSet";
      }

      [Observation]
      public void should_be_valid_because_name_is_ignored_in_update_mode()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_listing_the_parameters_of_a_compound_commit_dto_holding_every_kind_of_row : ContextSpecification<CompoundCommitDTO>
   {
      protected override void Context()
      {
         sut = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = new Compound { Name = "Aspirin" },
            AvailableExistingSets = new List<OverwriteParameterSet>(),
            NewSetName = "NewSet",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Changed" },
               new() { Path = "Reset", IsRemoval = true },
               new() { Path = "Untouched", IsUnchanged = true }
            }
         };
      }

      [Observation]
      public void should_show_the_change_and_the_untouched_entry_when_creating_a_new_set()
      {
         sut.CreateNew = true;
         sut.VisibleParameters.Select(x => x.Path).ShouldOnlyContain("Changed", "Untouched");
      }

      [Observation]
      public void should_show_the_change_and_the_reset_when_updating_the_selected_set()
      {
         sut.CreateNew = false;
         sut.VisibleParameters.Select(x => x.Path).ShouldOnlyContain("Changed", "Reset");
      }

      [Observation]
      public void should_offer_to_create_a_new_set()
      {
         sut.CanCreateNewSet.ShouldBeTrue();
      }
   }

   public class When_listing_the_parameters_of_a_compound_commit_dto_holding_only_reset_parameters : ContextSpecification<CompoundCommitDTO>
   {
      protected override void Context()
      {
         sut = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = new Compound { Name = "Aspirin" },
            AvailableExistingSets = new List<OverwriteParameterSet>(),
            NewSetName = "NewSet",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Reset", IsRemoval = true }
            }
         };
      }

      [Observation]
      public void should_not_offer_to_create_a_new_set()
      {
         sut.CanCreateNewSet.ShouldBeFalse();
      }

      [Observation]
      public void should_list_nothing_when_creating_a_new_set()
      {
         sut.CreateNew = true;
         sut.VisibleParameters.ShouldBeEmpty();
      }
   }

   public class When_validating_a_compound_commit_dto_with_no_existing_sets : ContextSpecification<CompoundCommitDTO>
   {
      protected override void Context()
      {
         sut = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = new Compound { Name = "Aspirin" },
            AvailableExistingSets = new List<OverwriteParameterSet>(),
            CreateNew = true,
            NewSetName = "AnyName",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true }
            }
         };
      }

      [Observation]
      public void should_be_valid()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }
}
