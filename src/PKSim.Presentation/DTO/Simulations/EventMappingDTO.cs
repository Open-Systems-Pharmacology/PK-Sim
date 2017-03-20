using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Simulations
{
   public class EventMappingDTO : DxValidatableDTO
   {
      public IEventMapping EventMapping { get; private set; }
      public IParameterDTO StartTimeParameter { get; set; }
      private PKSimEvent _event;

      public PKSimEvent Event
      {
         get { return _event; }
         set
         {
            _event = value;
            EventMapping.TemplateEventId = _event != null ? _event.Id : string.Empty;
            OnPropertyChanged(() => Event);
         }
      }

      public EventMappingDTO(IEventMapping eventMapping)
      {
         EventMapping = eventMapping;
         Rules.AddRange(AllRules.All());
      }

      public double StartTime
      {
         get { return StartTimeParameter.Value; }
         set { StartTimeParameter.Value = value; }
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