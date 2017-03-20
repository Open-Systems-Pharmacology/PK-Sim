using System;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;


namespace PKSim.Presentation.DTO.Mappers
{
   public interface IEventMappingDTOToEventMappingMapper
   {
      IEventMapping MapFrom(EventMappingDTO eventMappingDTO,  PKSim.Core.Model.Simulation simulation);

   }

   public class EventMappingDTOToEventMappingMapper : IEventMappingDTOToEventMappingMapper
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public EventMappingDTOToEventMappingMapper(IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public IEventMapping MapFrom(EventMappingDTO eventMappingDTO,  PKSim.Core.Model.Simulation simulation)
      {
         var templateEventId = eventMappingDTO.Event.Id;
         var templateEvent = _buildingBlockRepository.All().FindById(templateEventId);

         //in that case, event is a simulation event, we need to retrieve the template event
         if (templateEvent == null)
         {
            var usedEvent = simulation.UsedBuildingBlockById(templateEventId);
            templateEventId = usedEvent.TemplateId;
         }

         var eventMapping = eventMappingDTO.EventMapping;
         eventMapping.TemplateEventId = templateEventId;
         return eventMapping;
      }
   }
}