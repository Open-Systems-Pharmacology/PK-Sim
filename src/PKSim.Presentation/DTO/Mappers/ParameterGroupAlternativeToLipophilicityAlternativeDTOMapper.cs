using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper : IMapper<ParameterAlternative, LipophilictyAlternativeDTO>
   {
   }

   public class ParameterGroupAlternativeToLipophilicityAlternativeDTOMapper : IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<LipophilictyAlternativeDTO> _parameterDTOMapper;

      public ParameterGroupAlternativeToLipophilicityAlternativeDTOMapper(IParameterToParameterDTOInContainerMapper<LipophilictyAlternativeDTO> parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public LipophilictyAlternativeDTO MapFrom(ParameterAlternative parameterAlternative)
      {
         var lipophilictyAlternativeDTO = new LipophilictyAlternativeDTO(parameterAlternative);

         var lipophilicity = parameterAlternative.Parameter(CoreConstants.Parameter.Lipophilicity);
         lipophilictyAlternativeDTO.LipophilictyParameter = _parameterDTOMapper.MapFrom(lipophilicity, lipophilictyAlternativeDTO, dto => dto.Lipophilicty, dto => dto.LipophilictyParameter);

         return lipophilictyAlternativeDTO;
      }
   }
}