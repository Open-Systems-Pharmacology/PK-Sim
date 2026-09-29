using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
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

   public class When_reading_the_event_of_an_event_placeholder_mapping_dto_without_a_selected_event : concern_for_EventPlaceholderMappingDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.Selection = new EventSelectionDTO { BuildingBlock = null };
      }

      [Observation]
      public void should_not_return_an_event()
      {
         sut.Event.ShouldBeNull();
      }
   }

   public class When_reading_the_event_of_an_event_placeholder_mapping_dto_with_a_selected_event : concern_for_EventPlaceholderMappingDTO
   {
      private PKSimEvent _event;

      protected override void Context()
      {
         base.Context();
         _event = new PKSimEvent();
         sut.Selection = new EventSelectionDTO { BuildingBlock = _event };
      }

      [Observation]
      public void should_expose_the_event_from_the_selection()
      {
         sut.Event.ShouldBeEqualTo(_event);
      }
   }
}
