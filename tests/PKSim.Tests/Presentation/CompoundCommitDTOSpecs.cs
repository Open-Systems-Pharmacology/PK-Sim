using System.Collections.Generic;
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

   public class When_validating_a_compound_commit_dto_in_update_existing_mode : concern_for_CompoundCommitDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateNew = false;
         sut.SelectedExistingSet = sut.AvailableExistingSets[0];
         sut.NewSetName = "";
      }

      [Observation]
      public void should_be_valid_even_with_empty_name()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_validating_a_compound_commit_dto_in_update_mode_with_duplicate_name : concern_for_CompoundCommitDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateNew = false;
         sut.SelectedExistingSet = sut.AvailableExistingSets[0];
         sut.NewSetName = "ExistingSet1";
      }

      [Observation]
      public void should_be_valid_because_name_is_ignored_in_update_mode()
      {
         sut.IsValid().ShouldBeTrue();
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
