using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;

namespace PKSim.Presentation
{
   public abstract class concern_for_FormulationMappingDTO : ContextSpecification<FormulationMappingDTO>
   {
      protected override void Context()
      {
         sut = new FormulationMappingDTO {ApplicationType = ApplicationTypes.Oral};
      }
   }

   public class When_validating_a_formulation_mapping_dto_with_an_undefined_selection : concern_for_FormulationMappingDTO
   {
      [Observation]
      public void should_return_invalid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_a_formulation_mapping_dto_with_a_selection_whose_underlying_building_block_is_undefined: concern_for_FormulationMappingDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.Selection = new FormulationSelectionDTO();
      }

      [Observation]
      public void should_return_invalid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }
}	