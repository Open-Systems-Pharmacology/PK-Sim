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
   public abstract class concern_for_SystemicProcessDTO : ContextSpecification<SystemicProcessDTO>
   {
      protected Compound _compound;

      protected override void Context()
      {
         _compound = new Compound();
         sut = new SystemicProcessDTO(new SystemicProcess().WithName("Hepatic Clearance"))
         {
            Compound = _compound,
            SystemicProcessType = "Hepatic Clearance",
            DataSource = "Lab"
         };
      }
   }

   public class When_checking_if_a_systemic_process_DTO_is_valid : concern_for_SystemicProcessDTO
   {
      [Observation]
      public void should_return_true_if_the_data_source_is_defined_and_does_not_contain_illegal_characters()
      {
         sut.IsValid().ShouldBeTrue(sut.Validate().Message);
      }
   }

   public class When_validating_the_data_source_of_a_systemic_process_DTO : concern_for_SystemicProcessDTO
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
}
