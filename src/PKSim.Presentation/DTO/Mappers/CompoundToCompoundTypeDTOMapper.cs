using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ICompoundToCompoundTypeDTOMapper : IMapper<IEnumerable<IParameter>, IReadOnlyList<TypePKaDTO>>
   {
   }

   public class CompoundToCompoundTypeDTOMapper : ICompoundToCompoundTypeDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<TypePKaDTO> _parameterMapper;

      public CompoundToCompoundTypeDTOMapper(IParameterToParameterDTOInContainerMapper<TypePKaDTO> parameterMapper)
      {
         _parameterMapper = parameterMapper;
      }

      public IReadOnlyList<TypePKaDTO> MapFrom(IEnumerable<IParameter> compoundParameters)
      {
         var allCompoundParameters = compoundParameters.ToList();
         return new List<TypePKaDTO>
         {
            typePKaFor(allCompoundParameters, CoreConstants.Parameters.PARAMETER_PKA1, Constants.Parameters.COMPOUND_TYPE1),
            typePKaFor(allCompoundParameters, CoreConstants.Parameters.PARAMETER_PKA2, Constants.Parameters.COMPOUND_TYPE2),
            typePKaFor(allCompoundParameters, CoreConstants.Parameters.PARAMETER_PKA3, Constants.Parameters.COMPOUND_TYPE3)
         };
      }

      private TypePKaDTO typePKaFor(IList<IParameter> compoundParameters, string parameterPka1, string parameterCompoundType1)
      {
         var pKaParameter = compoundParameters.FindByName(parameterPka1);
         var compoundTypeParameter = compoundParameters.FindByName(parameterCompoundType1);
         var dto = new TypePKaDTO();
         dto.CompoundTypeParameter = _parameterMapper.MapFrom(compoundTypeParameter, dto, x => x.CompoundTypeValue, x => x.CompoundTypeParameter);
         dto.PKaParameter = _parameterMapper.MapFrom(pKaParameter, dto, x => x.PKa, x => x.PKaParameter);
         return dto;
      }
   }
}