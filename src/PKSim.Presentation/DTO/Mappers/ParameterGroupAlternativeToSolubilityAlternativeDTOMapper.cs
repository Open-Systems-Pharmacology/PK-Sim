using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
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
         var solubilityAtRefPh = parameterAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
         var solubilityTable = parameterAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);
         var solubilityParameter = solubilityTable.Formula.IsTable() ? solubilityTable : solubilityAtRefPh;

         var solubilityAlternativeDTO = new SolubilityAlternativeDTO(parameterAlternative, solubilityParameter.ValueOrigin);
         solubilityAlternativeDTO.SolubilityParameter = _parameterDTOMapper.MapFrom(solubilityParameter, solubilityAlternativeDTO, dto => dto.Solubility, dto => dto.SolubilityParameter);

         var refPh = parameterAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH);
         solubilityAlternativeDTO.RefpHParameter = _parameterDTOMapper.MapFrom(refPh, solubilityAlternativeDTO, dto => dto.RefpH, dto => dto.RefpHParameter);

         var gainPerCharge = parameterAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE);
         solubilityAlternativeDTO.GainPerChargeParameter = _parameterDTOMapper.MapFrom(gainPerCharge, solubilityAlternativeDTO, dto => dto.GainPerCharge, dto => dto.GainPerChargeParameter);
        

         return solubilityAlternativeDTO;
      }
   }
}