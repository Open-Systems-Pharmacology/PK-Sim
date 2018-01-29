using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ICompoundToCompoundTypeDTOMapper : IMapper<IEnumerable<IParameter>, CompoundTypeDTO>
   {
   }

   public class CompoundToCompoundTypeDTOMapper : ICompoundToCompoundTypeDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<TypePKaDTO> _parameterMapper;

      public CompoundToCompoundTypeDTOMapper(IParameterToParameterDTOInContainerMapper<TypePKaDTO> parameterMapper)
      {
         _parameterMapper = parameterMapper;
      }

      public CompoundTypeDTO MapFrom(IEnumerable<IParameter> compoundParameters)
      {
         var allCompoundParameters = compoundParameters.ToList();
         var compoundTypeAlternativeDTO = new CompoundTypeDTO();

         compoundTypeAlternativeDTO.AddTypePKa(typePKaFor(allCompoundParameters, CoreConstants.Parameters.PARAMETER_PKA1, CoreConstants.Parameters.COMPOUND_TYPE1));
         compoundTypeAlternativeDTO.AddTypePKa(typePKaFor(allCompoundParameters, CoreConstants.Parameters.PARAMETER_PKA2, CoreConstants.Parameters.COMPOUND_TYPE2));
         compoundTypeAlternativeDTO.AddTypePKa(typePKaFor(allCompoundParameters, CoreConstants.Parameters.PARAMETER_PKA3, CoreConstants.Parameters.COMPOUND_TYPE3));
         return compoundTypeAlternativeDTO;
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