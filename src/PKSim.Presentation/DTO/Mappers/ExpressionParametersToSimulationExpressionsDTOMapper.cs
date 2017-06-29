using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Extensions;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionParametersToSimulationExpressionsDTOMapper : IMapper<IEnumerable<IParameter>, SimulationExpressionsDTO>
   {
   }

   public class ExpressionParametersToSimulationExpressionsDTOMapper : IExpressionParametersToSimulationExpressionsDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<ExpressionContainerDTO> _containerParameterMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IParameterTask _parameterTask;
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IExecutionContext _executionContext;

      public ExpressionParametersToSimulationExpressionsDTOMapper(IParameterToParameterDTOInContainerMapper<ExpressionContainerDTO> containerParameterMapper, IRepresentationInfoRepository representationInfoRepository,
         IGroupRepository groupRepository, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterMapper, IFullPathDisplayResolver fullPathDisplayResolver,
         IExecutionContext executionContext)
      {
         _containerParameterMapper = containerParameterMapper;
         _representationInfoRepository = representationInfoRepository;
         _groupRepository = groupRepository;
         _parameterTask = parameterTask;
         _parameterMapper = parameterMapper;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _executionContext = executionContext;
      }

      public SimulationExpressionsDTO MapFrom(IEnumerable<IParameter> expressionParameters)
      {
         var dto = new SimulationExpressionsDTO();
         var allParameters = expressionParameters.ToList();

         dto.ReferenceConcentration = updateGlobalParameter(allParameters, CoreConstants.Parameter.REFERENCE_CONCENTRATION);
         dto.HalfLifeLiver = updateGlobalParameter(allParameters, CoreConstants.Parameter.HALF_LIFE_LIVER);
         dto.HalfLifeIntestine = updateGlobalParameter(allParameters, CoreConstants.Parameter.HALF_LIFE_INTESTINE);

         dto.RelativeExpressions = relativeExpressionsFrom(allParameters).ToList();
         return dto;
      }

      private IParameterDTO updateGlobalParameter(List<IParameter> allParameters, string globalParameterName)
      {
         var globalParameter = allParameters.FindByName(globalParameterName);
         allParameters.Remove(globalParameter);
         return _parameterMapper.MapFrom(globalParameter);
      }

      private IEnumerable<ExpressionContainerDTO> relativeExpressionsFrom(IReadOnlyList<IParameter> allParamters)
      {
         var expressionsParameters = _parameterTask.GroupExpressionParameters(allParamters);
         return expressionsParameters.KeyValues.Select(relativeExpression => expressionContainerFor(relativeExpression.Key, relativeExpression.Value))
            .Where(container => container != null);
      }

      private ExpressionContainerDTO expressionContainerFor(IParameter relativeExpression, IParameter relativeExpressioNorm)
      {
         var expressionContainerDTO = new ExpressionContainerDTO();
         var moleculeName = relativeExpression.ParentContainer.Name;
         var simulation = _executionContext.Get<Simulation>(relativeExpression.Origin.SimulationId);
         var molecule = simulation.Individual.MoleculeByName<IndividualMolecule>(moleculeName);
         var isTransporter = moleculeIsTransporter(molecule);

         expressionContainerDTO.RelativeExpressionParameter = _containerParameterMapper.MapFrom(relativeExpression, expressionContainerDTO, x => x.RelativeExpression, x => x.RelativeExpressionParameter);
         expressionContainerDTO.RelativeExpressionNormParameter = _containerParameterMapper.MapFrom(relativeExpressioNorm, expressionContainerDTO, x => x.RelativeExpressionNorm, x => x.RelativeExpressionNormParameter);

         IGroup group;

         if (parameterIsGlobalExpression(relativeExpression))
         {
            if (isTransporter)
               return null;

            group = _groupRepository.GroupByName(CoreConstants.Groups.VASCULAR_SYSTEM);
            expressionContainerDTO.ContainerPathDTO = _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, containerNameForGlobalExpression(relativeExpression.Name)).ToPathElement();
         }
         else if (expressionShouldBeTreatedAsGlobal(relativeExpression, isTransporter))
         {
            return null;
         }
         else if (relativeExpression.HasAncestorNamed(CoreConstants.Organ.Lumen))
         {
            group = _groupRepository.GroupByName(CoreConstants.Groups.GI_LUMEN);
            expressionContainerDTO.ContainerPathDTO = _representationInfoRepository.InfoFor(relativeExpression.ParentContainer.ParentContainer).ToPathElement();
         }
         else if (relativeExpression.HasAncestorNamed(CoreConstants.Compartment.Mucosa))
         {
            group = _groupRepository.GroupByName(CoreConstants.Groups.GI_MUCOSA);
            //Mucosa rel exp are for instance in Mucosa/Duodnum/interstitial
            expressionContainerDTO.ContainerPathDTO = _representationInfoRepository.InfoFor(relativeExpression.ParentContainer.ParentContainer.ParentContainer).ToPathElement();
         }

         else
         {
            var expressionContainer = expressionContainerFor(relativeExpression);
            group = _groupRepository.GroupByName(isGiTractOrgan(expressionContainer) ? CoreConstants.Groups.GI_NON_MUCOSA_TISSUE : CoreConstants.Groups.ORGANS_AND_TISSUES);
            expressionContainerDTO.ContainerPathDTO = _representationInfoRepository.InfoFor(expressionContainer).ToPathElement();
         }

         expressionContainerDTO.GroupingPathDTO = _representationInfoRepository.InfoFor(RepresentationObjectType.GROUP, group.Name).ToPathElement();
         expressionContainerDTO.Sequence = group.Sequence;
         expressionContainerDTO.MoleculeName = moleculeName;
         expressionContainerDTO.ParameterPath = _fullPathDisplayResolver.FullPathFor(relativeExpression);
         return expressionContainerDTO;
      }

      private bool moleculeIsTransporter(IndividualMolecule molecule)
      {
         return molecule.MoleculeType == QuantityType.Transporter;
      }

      private bool expressionShouldBeTreatedAsGlobal(IParameter relativeExpression, bool isTransporter)
      {
         if (relativeExpression.HasAncestorNamed(CoreConstants.Compartment.BloodCells) ||
             relativeExpression.HasAncestorNamed(CoreConstants.Compartment.Endosome))
            return true;

         var isPlasma = relativeExpression.HasAncestorNamed(CoreConstants.Compartment.Plasma);
         if (!isPlasma)
            return false;

         //special case for Plasma brain that can be indeed a local relative expression
         var expressionIsForTransporterBrainPlasma =
            isTransporter &&
            relativeExpression.HasAncestorNamed(CoreConstants.Organ.Brain);

         return !expressionIsForTransporterBrainPlasma;
      }

      private bool isGiTractOrgan(IContainer container)
      {
         if (container == null)
            return false;

         var organ = container as Organ;
         if (organ != null)
            return (organ.OrganType & OrganType.GiTractOrgans) != 0;

         return CoreConstants.Organ.GITractOrgans.Contains(container.Name);
      }

      private IContainer expressionContainerFor(IEntity entity)
      {
         if (entity == null)
            return null;

         var container = entity as IContainer;
         if (container != null)
         {
            if (container.ContainerType == ContainerType.Organ)
               return container;

            if (container.IsLiverZone())
               return container;
         }

         return expressionContainerFor(entity.ParentContainer);
      }

      private string containerNameForGlobalExpression(string parameterName)
      {
         if (string.Equals(parameterName, CoreConstants.Parameter.RelExpBloodCell))
            return CoreConstants.Compartment.BloodCells;

         if (string.Equals(parameterName, CoreConstants.Parameter.RelExpPlasma))
            return CoreConstants.Compartment.Plasma;

         if (string.Equals(parameterName, CoreConstants.Parameter.RelExpVascEndo))
            return CoreConstants.Compartment.VascularEndothelium;

         return parameterName;
      }

      private bool parameterIsGlobalExpression(IParameter relativeExpression)
      {
         return relativeExpression.NameIsOneOf(CoreConstants.Parameter.RelExpBloodCell,
            CoreConstants.Parameter.RelExpPlasma,
            CoreConstants.Parameter.RelExpVascEndo);
      }
   }
}