using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_EventPlaceholderMappingDTO : ContextSpecification<EventPlaceholderMappingDTO>
   {
      protected override void Context()
      {
         sut = new EventPlaceholderMappingDTO();
      }
   }

   public class When_validating_an_event_placeholder_mapping_dto_without_a_selection : concern_for_EventPlaceholderMappingDTO
   {
      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_an_event_placeholder_mapping_dto_with_a_null_building_block : concern_for_EventPlaceholderMappingDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.Selection = new EventSelectionDTO { BuildingBlock = null };
      }

      [Observation]
      public void should_not_be_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_an_event_placeholder_mapping_dto_with_a_valid_event : concern_for_EventPlaceholderMappingDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.Selection = new EventSelectionDTO { BuildingBlock = new PKSimEvent() };
      }

      [Observation]
      public void should_be_valid()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_expose_the_event_from_the_selection()
      {
         sut.Event.ShouldNotBeNull();
      }
   }
}
