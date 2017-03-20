using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IOutputIntervalToOutputIntervalDTOMapper : IMapper<OutputInterval, OutputIntervalDTO>
   {
   }

   public class OutputIntervalToOutputIntervalDTOMapper : IOutputIntervalToOutputIntervalDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<OutputIntervalDTO> _parameterDTOMapper;

      public OutputIntervalToOutputIntervalDTOMapper(IParameterToParameterDTOInContainerMapper<OutputIntervalDTO> parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public OutputIntervalDTO MapFrom(OutputInterval outputInterval)
      {
         var outputIntervalDTO = new OutputIntervalDTO {OutputInterval = outputInterval};
         outputIntervalDTO.StartTimeParameter = _parameterDTOMapper.MapFrom(outputInterval.StartTime, outputIntervalDTO, x => x.StartTime, x => x.StartTimeParameter);
         outputIntervalDTO.EndTimeParameter = _parameterDTOMapper.MapFrom(outputInterval.EndTime, outputIntervalDTO, x => x.EndTime, x => x.EndTimeParameter);
         outputIntervalDTO.ResolutionParameter = _parameterDTOMapper.MapFrom(outputInterval.Resolution, outputIntervalDTO, x => x.Resolution, x => x.ResolutionParameter);
         return outputIntervalDTO;
      }
   }
}