using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterToParameterDTOMapper : OSPSuite.Presentation.Mappers.IParameterToParameterDTOMapper
   {
      IParameterDTO MapAsReadWriteFrom(IParameter parameter);
   }

   public class ParameterToParameterDTOMapper : IParameterToParameterDTOMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IFormulaToFormulaTypeMapper _formulaTypeMapper;
      private readonly IPathToPathElementsMapper _parameterDisplayPathMapper;
      private readonly IFavoriteRepository _favoriteRepository;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterListOfValuesRetriever _parameterListOfValuesRetriever;

      public ParameterToParameterDTOMapper(
         IRepresentationInfoRepository representationInfoRepository,
         IFormulaToFormulaTypeMapper formulaTypeMapper,
         IPathToPathElementsMapper parameterDisplayPathMapper,
         IFavoriteRepository favoriteRepository,
         IEntityPathResolver entityPathResolver,
         IParameterListOfValuesRetriever parameterListOfValuesRetriever
      )
      {
         _representationInfoRepository = representationInfoRepository;
         _formulaTypeMapper = formulaTypeMapper;
         _parameterDisplayPathMapper = parameterDisplayPathMapper;
         _favoriteRepository = favoriteRepository;
         _entityPathResolver = entityPathResolver;
         _parameterListOfValuesRetriever = parameterListOfValuesRetriever;
      }

      public IParameterDTO MapFrom(IParameter parameter)
      {
         if (parameter == null)
            return new NullParameterDTO();

         var parameterDTO = new ParameterDTO(parameter);
         updateParameterDTOFromParameter(parameterDTO, parameter);
         return parameterDTO;
      }

      public IParameterDTO MapAsReadWriteFrom(IParameter parameter)
      {
         if (parameter == null)
            return new NullParameterDTO();

         var parameterDTO = new WritableParameterDTO(parameter);
         updateParameterDTOFromParameter(parameterDTO, parameter);
         return parameterDTO;
      }

      private void updateParameterDTOFromParameter(ParameterDTO parameterDTO, IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         var representationInfo = _representationInfoRepository.InfoFor(parameter);
         parameterDTO.DisplayName = representationInfo.DisplayName;
         parameterDTO.Description = representationInfo.Description;
         parameterDTO.AllUnits = allUnitsFor(parameter);
         parameterDTO.FormulaType = _formulaTypeMapper.MapFrom(parameter.Formula);
         parameterDTO.IsFavorite = _favoriteRepository.Contains(parameterPath);
         parameterDTO.Sequence = parameter.Sequence;
         parameterDTO.PathElements = _parameterDisplayPathMapper.MapFrom(parameter);

         //now create special list of values for parameter for our discrete parameters 
         updateListOfValues(parameterDTO, parameter);
      }

      private IEnumerable<Unit> allUnitsFor(IParameter parameter)
      {
         var allVisibleUnits = parameter.Dimension.VisibleUnits();

         //WORKAROUND to remove nano units
         if (parameter.IsNamed(CoreConstants.Parameters.MEAN_WEIGHT))
            return pruneUnits(allVisibleUnits, "kg", "g");

         if (parameter.IsNamed(CoreConstants.Parameters.MEAN_HEIGHT))
            return pruneUnits(allVisibleUnits, "m", "cm");

         return allVisibleUnits;
      }

      private IEnumerable<Unit> pruneUnits(IEnumerable<Unit> allVisibleUnits, params string[] unitsToKeep)
      {
         return allVisibleUnits.Where(unit => unitsToKeep.Contains(unit.Name)).ToList();
      }

      private void updateListOfValues(ParameterDTO parameterDTO, IParameter parameter) => _parameterListOfValuesRetriever.UpdateLisOfValues(parameterDTO.ListOfValues, parameter);
   }
}