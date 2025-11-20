using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ICompoundToMolWeightDTOMapper : IMapper<IEnumerable<IParameter>, MolWeightDTO>
   {
   }

   public class CompoundToMolWeightDTOMapper : ICompoundToMolWeightDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;

      public CompoundToMolWeightDTOMapper(IParameterToParameterDTOMapper parameterDTOMapper)
      {
         _parameterDTOMapper = parameterDTOMapper;
      }

      public MolWeightDTO MapFrom(IEnumerable<IParameter> compoundParameters)
      {
         var allCompoundParameters = compoundParameters.ToList();
         var effectiveMolWeight = allCompoundParameters.FindByName(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT);
         var molWeight = allCompoundParameters.FindByName(Constants.Parameters.MOL_WEIGHT);
         return new MolWeightDTO
         {
            MolWeightParameter = _parameterDTOMapper.MapMolWeightDTOFrom(molWeight, effectiveMolWeight),
            MolWeightEffParameter = _parameterDTOMapper.MapFrom(effectiveMolWeight),
            HasHalogensParameter = _parameterDTOMapper.MapFrom(allCompoundParameters.FindByName(Constants.Parameters.HAS_HALOGENS)),
         };
      }
   }
}