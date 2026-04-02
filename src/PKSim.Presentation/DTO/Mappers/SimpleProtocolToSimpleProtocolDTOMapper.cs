using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Protocols;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimpleProtocolToSimpleProtocolDTOMapper : IMapper<SimpleProtocol, SimpleProtocolDTO>
   {
   }

   public class SimpleProtocolToSimpleProtocolDTOMapper : ISimpleProtocolToSimpleProtocolDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public SimpleProtocolToSimpleProtocolDTOMapper(IParameterToParameterDTOMapper parameterDTOMapper, IBuildingBlockRepository buildingBlockRepository)
      {
         _parameterDTOMapper = parameterDTOMapper;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public SimpleProtocolDTO MapFrom(SimpleProtocol simpleProtocol)
      {
         var dto = new SimpleProtocolDTO(simpleProtocol)
         {
            Dose = _parameterDTOMapper.MapFrom(simpleProtocol.Dose),
            EndTime = _parameterDTOMapper.MapFrom(simpleProtocol.EndTimeParameter),
            EventOffset = _parameterDTOMapper.MapFrom(simpleProtocol.EventOffsetParameter),
            ApplicationType = simpleProtocol.ApplicationType,
            DosingInterval = simpleProtocol.DosingInterval,
            TargetOrgan = simpleProtocol.TargetOrgan,
            TargetCompartment = simpleProtocol.TargetCompartment
         };

         if (simpleProtocol.HasEvent)
            dto.SelectedEvent = _buildingBlockRepository.ById<PKSimEvent>(simpleProtocol.TemplateEventId);

         return dto;
      }
   }
}
