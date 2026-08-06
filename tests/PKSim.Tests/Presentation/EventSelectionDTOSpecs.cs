using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_EventSelectionDTO : ContextSpecification<EventSelectionDTO>
   {
      protected PKSimEvent _event;

      protected override void Context()
      {
         _event = new PKSimEvent { Id = "event1" };
         sut = new EventSelectionDTO { BuildingBlock = _event };
      }
   }

   public class When_comparing_two_event_selection_dto : concern_for_EventSelectionDTO
   {
      [Observation]
      public void should_return_equal_if_the_underlying_building_blocks_are_equal()
      {
         sut.Equals(new EventSelectionDTO { BuildingBlock = _event }).ShouldBeTrue();
      }

      [Observation]
      public void should_return_unequal_otherwise()
      {
         sut.Equals(new EventSelectionDTO { BuildingBlock = new PKSimEvent() }).ShouldBeFalse();
      }

      [Observation]
      public void should_return_display_name_from_to_string()
      {
         sut.DisplayName = "MyEvent";
         sut.ToString().ShouldBeEqualTo("MyEvent");
      }
   }
}
