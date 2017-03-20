using OSPSuite.Utility;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.DTO.Mappers
{
    public interface IParameterScalingToParameterScalingDTOMapper : IMapper<ParameterScaling, ParameterScalingDTO>
    {
    }

    public class ParameterScalingToParameterScalingDTOMapper : IParameterScalingToParameterScalingDTOMapper
    {
        private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
       private readonly IParameterToParameterDTOMapper _parameterDTOMapper;

       public ParameterScalingToParameterScalingDTOMapper(IFullPathDisplayResolver fullPathDisplayResolver,IParameterToParameterDTOMapper parameterDTOMapper)
        {
           _fullPathDisplayResolver = fullPathDisplayResolver;
           _parameterDTOMapper = parameterDTOMapper;
        }

       public ParameterScalingDTO MapFrom(ParameterScaling parameterScaling)
        {
            var parameterScalingDTO = new ParameterScalingDTO(parameterScaling)
                                          {
                                              ParameterFullPathDisplay = _fullPathDisplayResolver.FullPathFor(parameterScaling.SourceParameter),
                                              SourceParameter = _parameterDTOMapper.MapFrom(parameterScaling.SourceParameter),
                                              TargetParameter = _parameterDTOMapper.MapFrom(parameterScaling.TargetParameter),
                                              ScalingMethod = parameterScaling.ScalingMethod
                                          };

            return parameterScalingDTO;
        }
    }
}