using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ICompoundToCompoundTypeDTOMapper : IMapper<IEnumerable<IParameter>, CompoundTypeDTO>
   {
   }

   public class CompoundToCompoundTypeDTOMapper :  ICompoundToCompoundTypeDTOMapper
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

         addTypePKaFor(compoundTypeAlternativeDTO, allCompoundParameters, CoreConstants.Parameter.PARAMETER_PKA1, CoreConstants.Parameter.COMPOUND_TYPE1);
         addTypePKaFor(compoundTypeAlternativeDTO, allCompoundParameters, CoreConstants.Parameter.PARAMETER_PKA2, CoreConstants.Parameter.COMPOUND_TYPE2);
         addTypePKaFor(compoundTypeAlternativeDTO, allCompoundParameters, CoreConstants.Parameter.PARAMETER_PKA3, CoreConstants.Parameter.COMPOUND_TYPE3);
         return compoundTypeAlternativeDTO;
      }

      private void addTypePKaFor(CompoundTypeDTO compoundTypeDTO, IList<IParameter> compoundParameters, string parameterPka1, string parameterCompoundType1)
      {
         var pKaParameter = compoundParameters.FindByName(parameterPka1);
         var compoundTypeParameter = compoundParameters.FindByName(parameterCompoundType1);
         var typePKaDTO = new TypePKaDTO();
         typePKaDTO.CompoundTypeParameter = _parameterMapper.MapFrom(compoundTypeParameter, typePKaDTO, x => x.CompoundTypeValue, x => x.CompoundTypeParameter);
         typePKaDTO.PKaParameter = _parameterMapper.MapFrom(pKaParameter, typePKaDTO, x => x.PKa, x => x.PKaParameter);
         compoundTypeDTO.AddTypePKa(typePKaDTO);
      }
   }
}