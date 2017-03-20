using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimpleProtocolToSimpleProtocolDTOMapper : IMapper<SimpleProtocol, SimpleProtocolDTO>
   {
   }

   public class SimpleProtocolToSimpleProtocolDTOMapper : ISimpleProtocolToSimpleProtocolDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;

      public SimpleProtocolToSimpleProtocolDTOMapper(IParameterToParameterDTOMapper parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public SimpleProtocolDTO MapFrom(SimpleProtocol simpleProtocol)
      {
         return new SimpleProtocolDTO(simpleProtocol)
         {
            Dose = _parameterDTOMapper.MapFrom(simpleProtocol.Dose),
            EndTime = _parameterDTOMapper.MapFrom(simpleProtocol.EndTimeParameter),
            ApplicationType = simpleProtocol.ApplicationType,
            DosingInterval = simpleProtocol.DosingInterval,
            TargetOrgan = simpleProtocol.TargetOrgan,
            TargetCompartment = simpleProtocol.TargetCompartment
         };
      }
   }
}