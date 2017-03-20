using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Presentation.DTO.Mappers
{
    public interface IParameterValueVersionToCategoryParameterValueVersionDTOMapper : IMapper<ParameterValueVersion, CategoryParameterValueVersionDTO>
    {
    }

    public class ParameterValueVersionToCategoryParameterValueVersionDTOMapper : IParameterValueVersionToCategoryParameterValueVersionDTOMapper
    {
        private readonly IRepresentationInfoRepository _representationInfoRepository;

        public ParameterValueVersionToCategoryParameterValueVersionDTOMapper(IRepresentationInfoRepository representationInfoRepository)
        {
            _representationInfoRepository = representationInfoRepository;
        }

        public CategoryParameterValueVersionDTO MapFrom(ParameterValueVersion parameterValueVersion)
        {
            var repInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CATEGORY, parameterValueVersion.Category);
            return new CategoryParameterValueVersionDTO
                       {
                           DisplayName = repInfo.DisplayName,
                           Description = repInfo.Description,
                           Category = parameterValueVersion.Category,
                           ParameterValueVersion = parameterValueVersion
                       };
        }
    }
}