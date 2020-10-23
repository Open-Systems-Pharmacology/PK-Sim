using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionParametersToSimulationExpressionsDTOMapper : IMapper<IEnumerable<IParameter>, SimulationExpressionsDTO>
   {
   }

   public class ExpressionParametersToSimulationExpressionsDTOMapper : IExpressionParametersToSimulationExpressionsDTOMapper
   {
      private readonly IParameterToParameterDTOInContainerMapper<ExpressionParameterDTO> _containerParameterMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IParameterTask _parameterTask;
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IExecutionContext _executionContext;
      private readonly IOrganTypeRepository _organTypeRepository;
      private readonly IExpressionContainerMapper _expressionContainerMapper;

      public ExpressionParametersToSimulationExpressionsDTOMapper(
         IParameterToParameterDTOInContainerMapper<ExpressionParameterDTO> containerParameterMapper,
         IRepresentationInfoRepository representationInfoRepository,
         IGroupRepository groupRepository, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterMapper,
         IFullPathDisplayResolver fullPathDisplayResolver,
         IExecutionContext executionContext, IOrganTypeRepository organTypeRepository,
         IExpressionContainerMapper expressionContainerMapper)
      {
         _containerParameterMapper = containerParameterMapper;
         _representationInfoRepository = representationInfoRepository;
         _groupRepository = groupRepository;
         _parameterTask = parameterTask;
         _parameterMapper = parameterMapper;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _executionContext = executionContext;
         _organTypeRepository = organTypeRepository;
         _expressionContainerMapper = expressionContainerMapper;
      }

      public SimulationExpressionsDTO MapFrom(IEnumerable<IParameter> expressionParameters)
      {
         var allParameters = expressionParameters.ToList();

         return new SimulationExpressionsDTO(
            updateGlobalParameter(allParameters, CoreConstants.Parameters.REFERENCE_CONCENTRATION),
            updateGlobalParameter(allParameters, CoreConstants.Parameters.HALF_LIFE_LIVER),
            updateGlobalParameter(allParameters, CoreConstants.Parameters.HALF_LIFE_INTESTINE),
            relativeExpressionsFrom(allParameters).ToList()
         );
      }

      private IParameterDTO updateGlobalParameter(List<IParameter> allParameters, string globalParameterName)
      {
         var globalParameter = allParameters.FindByName(globalParameterName);
         allParameters.Remove(globalParameter);
         return _parameterMapper.MapFrom(globalParameter);
      }

      private IEnumerable<ExpressionParameterDTO> relativeExpressionsFrom(IReadOnlyList<IParameter> allParameters)
      {
         return allParameters.Select(expressionContainerFor);
      }

      private ExpressionParameterDTO expressionContainerFor(IParameter relativeExpression)
      {
         var expressionContainerDTO = new ExpressionParameterDTO();
         var moleculeName = relativeExpression.ParentContainer.Name;
         var simulation = _executionContext.Get<Simulation>(relativeExpression.Origin.SimulationId);
         var molecule = simulation.Individual?.MoleculeByName<IndividualMolecule>(moleculeName);
         var isTransporter = moleculeIsTransporter(molecule);

         return _expressionContainerMapper.MapFrom(molecule, relativeExpression);
      }

      private bool moleculeIsTransporter(IndividualMolecule molecule)
      {
         return molecule?.MoleculeType == QuantityType.Transporter;
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

         return _organTypeRepository.OrganTypeFor(container) == OrganType.GiTractOrgans;
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
         if (string.Equals(parameterName, CoreConstants.Parameters.REL_EXP_BLOOD_CELLS))
            return CoreConstants.Compartment.BloodCells;

         if (string.Equals(parameterName, CoreConstants.Parameters.REL_EXP_PLASMA))
            return CoreConstants.Compartment.Plasma;

         if (string.Equals(parameterName, CoreConstants.Parameters.REL_EXP_VASC_ENDO))
            return CoreConstants.Compartment.VascularEndothelium;

         return parameterName;
      }
   }
}