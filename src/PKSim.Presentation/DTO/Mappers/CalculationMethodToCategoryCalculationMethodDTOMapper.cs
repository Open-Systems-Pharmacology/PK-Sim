using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Mappers
{
    public interface ICalculationMethodToCategoryCalculationMethodDTOMapper : IMapper<CalculationMethod, CategoryCalculationMethodDTO>
    {
    }

    public class CalculationMethodToCategoryCalculationMethodDTOMapper : ICalculationMethodToCategoryCalculationMethodDTOMapper
    {
        private readonly IRepresentationInfoRepository _representationInfoRepository;

        public CalculationMethodToCategoryCalculationMethodDTOMapper(IRepresentationInfoRepository representationInfoRepository)
        {
            _representationInfoRepository = representationInfoRepository;
        }

        public CategoryCalculationMethodDTO MapFrom(CalculationMethod calculationMethod)
        {
            var repInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CATEGORY, calculationMethod.Category);
            return new CategoryCalculationMethodDTO
                       {
                           DisplayName = repInfo.DisplayName,
                           Description = repInfo.Description,
                           Category = calculationMethod.Category,
                           CalculationMethod = calculationMethod
                       };
        }
    }
}