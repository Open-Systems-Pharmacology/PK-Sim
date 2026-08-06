using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class EventPlaceholderMappingDTO : DxValidatableDTO
   {
      public string EventKey { get; set; }
      public EventSelectionDTO Selection { get; set; }
      public PKSimEvent Event => Selection?.BuildingBlock;

      public EventPlaceholderMappingDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule eventNotNull { get; } = CreateRule.For<EventPlaceholderMappingDTO>()
            .Property(x => x.Selection)
            .WithRule((dto, s) => s?.BuildingBlock != null)
            .WithError((dto, s) => PKSimConstants.Error.BuildingBlockNotDefined(PKSimConstants.ObjectTypes.Event));

         public static IEnumerable<IBusinessRule> All()
         {
            yield return eventNotNull;
         }
      }
   }
}
