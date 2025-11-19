using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterToMolWeightParameterDTOMapper
   {
      MolWeightParameterDTO MapFrom(IParameter molWeightParameter, IParameter effectiveMolWeightParameter);
   }
   
   public class ParameterToMolWeightParameterDTOMapper : ParameterToParameterDTOMapper, IParameterToMolWeightParameterDTOMapper
   {
      public ParameterToMolWeightParameterDTOMapper(
         IRepresentationInfoRepository representationInfoRepository,
         IFormulaToFormulaTypeMapper formulaTypeMapper,
         IPathToPathElementsMapper parameterDisplayPathMapper,
         IFavoriteRepository favoriteRepository,
         IEntityPathResolver entityPathResolver,
         IParameterListOfValuesRetriever parameterListOfValuesRetriever
      ) : base(
         representationInfoRepository,
         formulaTypeMapper,
         parameterDisplayPathMapper,
         favoriteRepository,
         entityPathResolver,
         parameterListOfValuesRetriever
      )
      {
      }

      public MolWeightParameterDTO MapFrom(IParameter molWeightParameter, IParameter effectiveMolWeightParameter)
      {
         var molWeightParameterDTO = new MolWeightParameterDTO(molWeightParameter, effectiveMolWeightParameter);
         UpdateParameterDTOFromParameter(molWeightParameterDTO, molWeightParameter);
         return molWeightParameterDTO;
      }
   }
}