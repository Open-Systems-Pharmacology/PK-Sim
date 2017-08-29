using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterGroupAlternativeToFractionUnboundAlternativeDTOMapper : IMapper<ParameterAlternativeWithSpecies, FractionUnboundAlternativeDTO>
   {
   }

   public class ParameterGroupAlternativeToFractionUnboundAlternativeDTOMapper : IParameterGroupAlternativeToFractionUnboundAlternativeDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<FractionUnboundAlternativeDTO> _parameterDTOMapper;

      public ParameterGroupAlternativeToFractionUnboundAlternativeDTOMapper(IParameterToParameterDTOInContainerMapper<FractionUnboundAlternativeDTO> parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public FractionUnboundAlternativeDTO MapFrom(ParameterAlternativeWithSpecies parameterAlternative)
      {
         var fractionUnboundAlternativeDTO = new FractionUnboundAlternativeDTO(parameterAlternative);

         var lipophilicity = parameterAlternative.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE);
         fractionUnboundAlternativeDTO.FractionUnboundParameter = _parameterDTOMapper.MapFrom(lipophilicity, fractionUnboundAlternativeDTO, dto => dto.FractionUnbound, dto => dto.FractionUnboundParameter);

         return fractionUnboundAlternativeDTO;
      }
   }
}