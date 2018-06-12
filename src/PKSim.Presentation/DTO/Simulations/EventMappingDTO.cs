using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class EventMappingDTO : DxValidatableDTO
   {
      public EventMapping EventMapping { get; }
      public IParameterDTO StartTimeParameter { get; set; }
      private PKSimEvent _event;

      public PKSimEvent Event
      {
         get => _event;
         set
         {
            _event = value;
            EventMapping.TemplateEventId = _event != null ? _event.Id : string.Empty;
            OnPropertyChanged(() => Event);
         }
      }

      public EventMappingDTO(EventMapping eventMapping)
      {
         EventMapping = eventMapping;
         Rules.AddRange(AllRules.All());
      }

      public double StartTime
      {
         get => StartTimeParameter.Value;
         set => StartTimeParameter.Value = value;
      }

      private static class AllRules
      {
         private static IBusinessRule buildingBlockNotNull
         {
            get
            {
               return CreateRule.For<EventMappingDTO>()
                  .Property(item => item.Event)
                  .WithRule((dto, ev) => ev != null)
                  .WithError((dto, block) => PKSimConstants.Error.BuildingBlockNotDefined(PKSimConstants.ObjectTypes.Event));
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return buildingBlockNotNull;
         }
      }
   }
}