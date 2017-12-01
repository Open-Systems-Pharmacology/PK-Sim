using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

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
      private readonly List<string> _parameterNameWithListOfValues;

      public ParameterToParameterDTOMapper(IRepresentationInfoRepository representationInfoRepository, IFormulaToFormulaTypeMapper formulaTypeMapper,
         IPathToPathElementsMapper parameterDisplayPathMapper, IFavoriteRepository favoriteRepository,
         IEntityPathResolver entityPathResolver)
      {
         _representationInfoRepository = representationInfoRepository;
         _formulaTypeMapper = formulaTypeMapper;
         _parameterDisplayPathMapper = parameterDisplayPathMapper;
         _favoriteRepository = favoriteRepository;
         _entityPathResolver = entityPathResolver;
         _parameterNameWithListOfValues = new List<string>(CoreConstants.Parameter.AllWithListOfValues);
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

         //now create special list of values for parmaeter for our discrete parameters 
         updateListOfValues(parameterDTO, parameter);
      }

      private IEnumerable<Unit> allUnitsFor(IParameter parameter)
      {
         var allVisibleUnits = parameter.Dimension.VisibleUnits();

         //WORKAROUND to remove nano units
         if (parameter.IsNamed(CoreConstants.Parameter.MEAN_WEIGHT))
            return pruneUnits(allVisibleUnits, "kg", "g");

         if (parameter.IsNamed(CoreConstants.Parameter.MEAN_HEIGHT))
            return pruneUnits(allVisibleUnits, "m", "cm");

         return allVisibleUnits;
      }

      private IEnumerable<Unit> pruneUnits(IEnumerable<Unit> allVisibleUnits, params string[] unitsToKeep)
      {
         return allVisibleUnits.Where(unit => unitsToKeep.Contains(unit.Name)).ToList();
      }

      private void updateListOfValues(ParameterDTO parameterDTO, IParameter parameter)
      {
         if (!_parameterNameWithListOfValues.Contains(parameter.Name)) return;
         if (parameter.IsNamed(CoreConstants.Parameter.PARTICLE_SIZE_DISTRIBUTION))
         {
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.PARTICLE_SIZE_DISTRIBUTION_NORMAL, PKSimConstants.UI.Normal);
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL, PKSimConstants.UI.LogNormal);
         }
         else if (parameter.IsNamed(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER))
         {
            parameterDTO.ListOfValues.Add(CoreConstants.Compound.BINDING_PARTNER_ALBUMIN, PKSimConstants.UI.Albumin);
            parameterDTO.ListOfValues.Add(CoreConstants.Compound.BINDING_PARTNER_AGP, PKSimConstants.UI.Glycoprotein);
            parameterDTO.ListOfValues.Add(CoreConstants.Compound.BINDING_PARTNER_UNKNOWN, PKSimConstants.UI.Unknown);
         }
         else if (parameter.IsNamed(CoreConstants.Parameter.NUMBER_OF_BINS))
         {
            addNumericListOfValues(parameterDTO, 1, CoreConstants.Parameter.MAX_NUMBER_OF_BINS);
         }
         else if (parameter.NameIsOneOf(CoreConstants.Parameter.Halogens))
         {
            addNumericListOfValues(parameterDTO, 0, CoreConstants.Parameter.MAX_NUMBER_OF_HALOGENS);
         }
         else if (parameter.Name.StartsWith(CoreConstants.Parameter.ParameterCompoundTypeBase))
         {
            parameterDTO.ListOfValues.Add(CoreConstants.Compound.COMPOUND_TYPE_ACID, CompoundType.Acid.ToString());
            parameterDTO.ListOfValues.Add(CoreConstants.Compound.COMPOUND_TYPE_NEUTRAL, CompoundType.Neutral.ToString());
            parameterDTO.ListOfValues.Add(CoreConstants.Compound.COMPOUND_TYPE_BASE, CompoundType.Base.ToString());
         }
         else if (parameter.IsNamed(CoreConstants.Parameter.PARTICLE_DISPERSE_SYSTEM))
         {
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.MONODISPERSE, PKSimConstants.UI.Monodisperse);
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.POLYDISPERSE, PKSimConstants.UI.Polydisperse);
         }
         else if (parameter.IsNamed(CoreConstants.Parameter.PRECIPITATED_DRUG_SOLUBLE))
         {
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.SOLUBLE, PKSimConstants.UI.Soluble);
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.INSOLUBLE, PKSimConstants.UI.Insoluble);
         }
         else if (parameter.IsNamed(CoreConstants.Parameter.GESTATIONAL_AGE))
         {
            addNumericListOfValues(parameterDTO, CoreConstants.PRETERM_RANGE.Min(), CoreConstants.PRETERM_RANGE.Max());
         }
         else if (CoreConstants.Parameter.AllBooleanParameters.Contains(parameter.Name))
         {
            parameterDTO.ListOfValues.Add(1, PKSimConstants.UI.Yes);
            parameterDTO.ListOfValues.Add(0, PKSimConstants.UI.No);
         }
         else if (parameter.NameIsOneOf(CoreConstants.Parameter.PARA_ABSORBTION_SINK, CoreConstants.Parameter.TRANS_ABSORBTION_SINK))
         {
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.SINK_CONDITION, PKSimConstants.UI.SinkCondition);
            parameterDTO.ListOfValues.Add(CoreConstants.Parameter.NO_SINK_CONDITION, PKSimConstants.UI.NoSinkCondition);
         }
         else
            throw new ArgumentException("Cannot create list of values", parameter.Name);
      }

      private void addNumericListOfValues(ParameterDTO parameterDTO, int min, int max)
      {
         for (int i = min; i <= max; i++)
         {
            parameterDTO.ListOfValues.Add(i, i.ToString());
         }
      }
   }
}