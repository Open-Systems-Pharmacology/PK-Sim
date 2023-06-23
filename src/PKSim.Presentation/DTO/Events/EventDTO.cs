using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation.DTO.Events
{
   public class EventDTO
   {
      public string Description { get; set; }
      public EventGroupBuilder Template { get; set; }
   }
}