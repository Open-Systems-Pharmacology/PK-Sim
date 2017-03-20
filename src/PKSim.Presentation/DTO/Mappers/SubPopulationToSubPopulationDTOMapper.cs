using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Mappers
{
    public interface ISubPopulationToSubPopulationDTOMapper : IMapper<SubPopulation, IEnumerable<CategoryParameterValueVersionDTO>>
    {
    }

    public class SubPopulationToSubPopulationDTOMapper : ISubPopulationToSubPopulationDTOMapper
    {
        private readonly IParameterValueVersionToCategoryParameterValueVersionDTOMapper _parameterValueVersionDTOMapper;

        public SubPopulationToSubPopulationDTOMapper(IParameterValueVersionToCategoryParameterValueVersionDTOMapper parameterValueVersionDTOMapper)
        {
            _parameterValueVersionDTOMapper = parameterValueVersionDTOMapper;
        }

        public IEnumerable<CategoryParameterValueVersionDTO> MapFrom(SubPopulation subPopulation)
        {
           return subPopulation.ParameterValueVersions.MapAllUsing(_parameterValueVersionDTOMapper);
        }
    }
}