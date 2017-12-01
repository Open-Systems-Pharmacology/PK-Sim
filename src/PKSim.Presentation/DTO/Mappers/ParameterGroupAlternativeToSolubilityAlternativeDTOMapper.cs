using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterGroupAlternativeToSolubilityAlternativeDTOMapper : IMapper<ParameterAlternative, SolubilityAlternativeDTO>
   {
   }

   public class ParameterGroupAlternativeToSolubilityAlternativeDTOMapper : IParameterGroupAlternativeToSolubilityAlternativeDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<SolubilityAlternativeDTO> _parameterDTOMapper;

      public ParameterGroupAlternativeToSolubilityAlternativeDTOMapper(IParameterToParameterDTOInContainerMapper<SolubilityAlternativeDTO> parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public SolubilityAlternativeDTO MapFrom(ParameterAlternative parameterAlternative)
      {
         var solubilityAlternativeDTO = new SolubilityAlternativeDTO(parameterAlternative);

         var solubility = parameterAlternative.Parameter(CoreConstants.Parameter.SOLUBILITY_AT_REFERENCE_PH);
         var refPh = parameterAlternative.Parameter(CoreConstants.Parameter.REFERENCE_PH);
         var gainPerCharge = parameterAlternative.Parameter(CoreConstants.Parameter.SolubilityGainPerCharge);
         solubilityAlternativeDTO.SolubilityParameter = _parameterDTOMapper.MapFrom(solubility, solubilityAlternativeDTO, dto => dto.Solubility, dto => dto.SolubilityParameter);
         solubilityAlternativeDTO.RefpHParameter = _parameterDTOMapper.MapFrom(refPh, solubilityAlternativeDTO, dto => dto.RefpH, dto => dto.RefpHParameter);
         solubilityAlternativeDTO.GainPerChargeParameter = _parameterDTOMapper.MapFrom(gainPerCharge, solubilityAlternativeDTO, dto => dto.GainPerCharge, dto => dto.GainPerChargeParameter);

         return solubilityAlternativeDTO;
      }
   }
}