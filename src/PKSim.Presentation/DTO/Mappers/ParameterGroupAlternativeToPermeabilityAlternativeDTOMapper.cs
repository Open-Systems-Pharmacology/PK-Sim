using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper
   {
      PermeabilityAlternativeDTO MapFrom(ParameterAlternative parameterAlternative, string parameterName);
   }

   public class ParameterGroupAlternativeToPermeabilityAlternativeDTOMapper : IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<PermeabilityAlternativeDTO> _parameterDTOMapper;

      public ParameterGroupAlternativeToPermeabilityAlternativeDTOMapper(IParameterToParameterDTOInContainerMapper<PermeabilityAlternativeDTO> parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public PermeabilityAlternativeDTO MapFrom(ParameterAlternative parameterAlternative, string parameterName)
      {
         var permeabilityAlternativeDTO = new PermeabilityAlternativeDTO(parameterAlternative);

         var permeability = parameterAlternative.Parameter(parameterName);
         permeabilityAlternativeDTO.PermeabilityParameter = _parameterDTOMapper.MapFrom(permeability, permeabilityAlternativeDTO, dto => dto.Permeability, dto => dto.PermeabilityParameter);

         return permeabilityAlternativeDTO;
      }
   }
}