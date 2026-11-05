using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Presentation
{
   public abstract class concern_for_PartialProcessDTO : ContextSpecification<PartialProcessDTO>
   {
      protected Compound _compound;

      protected override void Context()
      {
         _compound = new Compound();
         sut = new PartialProcessDTO(new EnzymaticProcess().WithName("Metabolization"))
         {
            Compound = _compound,
            MoleculeName = "CYP3A4",
            DataSource = "Lab"
         };
      }
   }

   public class When_checking_if_a_partial_process_DTO_is_valid : concern_for_PartialProcessDTO
   {
      [Observation]
      public void should_return_true_if_the_molecule_name_and_data_source_are_defined_and_do_not_contain_illegal_characters()
      {
         sut.IsValid().ShouldBeTrue(sut.Validate().Message);
      }

      [Observation]
      public void should_return_false_if_a_process_with_the_same_name_already_exists_in_the_compound()
      {
         var existingProcess = new EnzymaticProcess {MoleculeName = "CYP3A4", DataSource = "Lab"};
         existingProcess.RefreshName();
         _compound.AddProcess(existingProcess);
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_the_data_source_of_a_partial_process_DTO : concern_for_PartialProcessDTO
   {
      [Observation]
      public void should_not_report_an_error_for_a_data_source_without_illegal_characters()
      {
         sut.Validate(x => x.DataSource, "Lab").IsEmpty.ShouldBeTrue();
      }

      [Observation]
      public void should_report_an_error_for_an_empty_data_source()
      {
         sut.Validate(x => x.DataSource, "").IsEmpty.ShouldBeFalse();
      }

      [Observation]
      public void should_report_an_error_for_a_data_source_containing_an_illegal_character()
      {
         foreach (var illegalCharacter in ILLEGAL_CHARACTERS)
         {
            sut.Validate(x => x.DataSource, $"this{illegalCharacter}is{illegalCharacter}one{illegalCharacter}source").Message.Contains(PKSimConstants.Error.DataSourceCannotContainIllegalCharacters).ShouldBeTrue($"'{illegalCharacter}' was accepted");
         }
      }
   }

   public class When_validating_the_molecule_name_of_a_partial_process_DTO : concern_for_PartialProcessDTO
   {
      [Observation]
      public void should_not_report_an_error_for_a_molecule_name_without_illegal_characters()
      {
         sut.Validate(x => x.MoleculeName, "CYP2D6").IsEmpty.ShouldBeTrue();
      }

      [Observation]
      public void should_report_an_error_for_an_empty_molecule_name()
      {
         sut.Validate(x => x.MoleculeName, "").IsEmpty.ShouldBeFalse();
      }

      [Observation]
      public void should_report_an_error_for_a_molecule_name_containing_an_illegal_character()
      {
         foreach (var illegalCharacter in ILLEGAL_CHARACTERS)
         {
            sut.Validate(x => x.MoleculeName, $"CYP{illegalCharacter}3A4").Message.Contains(PKSimConstants.Error.MoleculeNameCannotContainIllegalCharacters).ShouldBeTrue($"'{illegalCharacter}' was accepted");
         }
      }
   }
}
