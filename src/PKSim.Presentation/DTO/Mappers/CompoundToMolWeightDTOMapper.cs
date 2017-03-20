using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;


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
         return new MolWeightDTO
                   {
                      MolWeightParameter = _parameterDTOMapper.MapFrom(allCompoundParameters.FindByName(Constants.Parameters.MOL_WEIGHT)),
                      MolWeightEffParameter = _parameterDTOMapper.MapFrom(allCompoundParameters.FindByName(CoreConstants.Parameter.MolWeightEff)),
                      HasHalogensParameter = _parameterDTOMapper.MapFrom(allCompoundParameters.FindByName(CoreConstants.Parameter.HAS_HALOGENS)),
                   };

      }
   }
}