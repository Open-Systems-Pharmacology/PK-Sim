using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IEventMappingToEventMappingDTOMapper
   {
      EventMappingDTO MapFrom(EventMapping eventMapping, Simulation simulation);
   }

   public class EventMappingToEventMappingDTOMapper : IEventMappingToEventMappingDTOMapper
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IParameterToParameterDTOInContainerMapper<EventMappingDTO> _parameterDTOMapper;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;

      public EventMappingToEventMappingDTOMapper(IBuildingBlockRepository buildingBlockRepository,
         IParameterToParameterDTOInContainerMapper<EventMappingDTO> parameterDTOMapper,
         IBuildingBlockInSimulationManager buildingBlockInSimulationManager)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _parameterDTOMapper = parameterDTOMapper;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
      }

      public EventMappingDTO MapFrom(EventMapping eventMapping, Simulation simulation)
      {
         var eventMappingDTO = new EventMappingDTO(eventMapping);
         eventMappingDTO.StartTimeParameter = _parameterDTOMapper.MapFrom(eventMapping.StartTime, eventMappingDTO, x => x.StartTime, x => x.StartTimeParameter);

         var templateEvent = _buildingBlockRepository.All<PKSimEvent>().FindById(eventMapping.TemplateEventId);
         var usedEvent = simulation.UsedBuildingBlockByTemplateId(eventMapping.TemplateEventId);
         //simulation was already using an event based on template. Retrieved the building block used
         if (usedEvent != null)
            templateEvent = _buildingBlockInSimulationManager.TemplateBuildingBlockUsedBy<PKSimEvent>(usedEvent);

         eventMappingDTO.Event = templateEvent;
         return eventMappingDTO;
      }
   }
}