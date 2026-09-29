using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class EventPlaceholderMappingDTO : DxValidatableDTO
   {
      public string EventKey { get; set; }
      public EventSelectionDTO Selection { get; set; }
      public PKSimEvent Event => Selection?.BuildingBlock;
   }
}
