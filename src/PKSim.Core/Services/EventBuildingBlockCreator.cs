using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IEventBuildingBlockCreator
   {
      /// <summary>
      ///    return the event building block built based on the given protocol and the associated formulation.
      ///    Special simulation event such as eat,sport etc.. should be managed in this class as well
      /// </summary>
      IEventGroupBuildingBlock CreateFor(Simulation simulation);
   }

   public class EventBuildingBlockCreator : IEventBuildingBlockCreator
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IProtocolToSchemaItemsMapper _schemaItemsMapper;
      private readonly IApplicationFactory _applicationFactory;
      private readonly IFormulationFromMappingRetriever _formulationFromMappingRetriever;
      private readonly ICloneManagerForBuildingBlock _cloneManagerForBuildingBlock;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IEventGroupRepository _eventGroupRepository;
      private Simulation _simulation;
      private IEventGroupBuildingBlock _eventGroupBuildingBlock;

      public EventBuildingBlockCreator(IObjectBaseFactory objectBaseFactory,
         IProtocolToSchemaItemsMapper schemaItemsMapper,
         IApplicationFactory applicationFactory,
         IFormulationFromMappingRetriever formulationFromMappingRetriever,
         ICloneManagerForBuildingBlock cloneManagerForBuildingBlock,
         IParameterIdUpdater parameterIdUpdater,
         IParameterSetUpdater parameterSetUpdater,
         IEventGroupRepository eventGroupRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _schemaItemsMapper = schemaItemsMapper;
         _applicationFactory = applicationFactory;
         _formulationFromMappingRetriever = formulationFromMappingRetriever;
         _cloneManagerForBuildingBlock = cloneManagerForBuildingBlock;
         _parameterIdUpdater = parameterIdUpdater;
         _parameterSetUpdater = parameterSetUpdater;
         _eventGroupRepository = eventGroupRepository;
      }

      public IEventGroupBuildingBlock CreateFor(Simulation simulation)
      {
         try
         {
            _simulation = simulation;
            _eventGroupBuildingBlock = _objectBaseFactory.Create<IEventGroupBuildingBlock>().WithName(simulation.Name);
            _cloneManagerForBuildingBlock.FormulaCache = _eventGroupBuildingBlock.FormulaCache;

            createApplications(_simulation.CompoundPropertiesList);

            createNonApplicationEvents();

            return _eventGroupBuildingBlock;
         }
         finally
         {
            _simulation = null;
            _eventGroupBuildingBlock = null;
         }
      }

      private void createNonApplicationEvents()
      {
         // group events by the event-building block they are using
         var eventBuildingBlockInfos = (from eventMapping in _simulation.EventProperties.EventMappings
               let usedBuildingBlock = _simulation.UsedBuildingBlockByTemplateId(eventMapping.TemplateEventId)
               let eventBuildingBlock = usedBuildingBlock.BuildingBlock.DowncastTo<PKSimEvent>()
               select new {eventBuildingBlock.Id, eventBuildingBlock.TemplateName, eventBuildingBlock.Name})
            .Distinct();

         // create event groups for each used event-building block
         foreach (var eventBuildingBlockInfo in eventBuildingBlockInfos)
         {
            // get event group template
            var templateEventGroup = _eventGroupRepository.FindByName(eventBuildingBlockInfo.TemplateName);

            // create new event group
            var eventGroup = _cloneManagerForBuildingBlock.Clone(templateEventGroup);
            eventGroup.Name = eventBuildingBlockInfo.Name;
            eventGroup.RemoveChild(eventGroup.MainSubContainer());

            // get building block and eventgroup-template to be used
            var eventBuildingBlock = _simulation.UsedBuildingBlockById(eventBuildingBlockInfo.Id);
            var eventTemplate = eventBuildingBlock.BuildingBlock.DowncastTo<PKSimEvent>();

            // set event group parameter
            _parameterSetUpdater.UpdateValuesByName(eventTemplate, eventGroup);

            // create subcontainers (event groups) for all events of the same type
            int eventIndex = 0; //used for naming of event subcontainers only

            foreach (var eventMapping in _simulation.EventProperties.EventMappings.OrderBy(em => em.StartTime.Value))
            {
               if (!eventMapping.TemplateEventId.Equals(eventBuildingBlock.TemplateId))
                  continue; //event from different template

               // clone main event subcontainer and set its start time
               var mainSubContainer = _cloneManagerForBuildingBlock.Clone(templateEventGroup.MainSubContainer());

               eventIndex += 1;
               mainSubContainer.Name = $"{eventBuildingBlockInfo.Name}_{eventIndex}";

               _parameterSetUpdater.UpdateValue(eventMapping.StartTime, mainSubContainer.StartTime());

               eventGroup.Add(mainSubContainer);
            }

            // update building block ids
            _parameterIdUpdater.UpdateBuildingBlockId(eventGroup, eventTemplate);

            _eventGroupBuildingBlock.Add(eventGroup);
         }
      }

      private void createApplications(IReadOnlyList<CompoundProperties> compoundPropertiesList)
      {
         // create ApplicationSet-EventGroup
         var applicationSet = _objectBaseFactory.Create<IEventGroupBuilder>()
            .WithName(Constants.APPLICATIONS)
            .WithIcon(Constants.APPLICATIONS);

         applicationSet.SourceCriteria.Add(new MatchTagCondition(Constants.ROOT_CONTAINER_TAG));

         compoundPropertiesList.Each(cp => addProtocol(applicationSet, cp));

         _eventGroupBuildingBlock.Add(applicationSet);
      }

      private void addProtocol(IEventGroupBuilder applicationSet, CompoundProperties compoundProperties)
      {
         var protocol = compoundProperties.ProtocolProperties.Protocol;
         if (protocol == null)
            return;

         var protocolEventGroup = _objectBaseFactory.Create<IEventGroupBuilder>()
            .WithName(protocol.Name)
            .WithParentContainer(applicationSet);

         _schemaItemsMapper.MapFrom(protocol).Each((schemaItem, index) =>
         {
            //+1 to start at 1 for the nomenclature
            string applicationName = $"{CoreConstants.APPLICATION_NAME_TEMPLATE}{index + 1}";
            addApplication(protocolEventGroup, schemaItem, applicationName, compoundProperties, protocol);
         });

         _parameterIdUpdater.UpdateBuildingBlockId(protocolEventGroup, protocol);
      }

      private void addApplication(IEventGroupBuilder protocolGroupBuilder, ISchemaItem schemaItem, string applicationName, CompoundProperties compoundProperties, Protocol protocol)
      {
         IContainer applicationParentContainer;
         string formulationType;

         IEnumerable<IParameter> formulationParameters;

         if (schemaItem.NeedsFormulation)
         {
            var formulation = _formulationFromMappingRetriever.FormulationUsedBy(_simulation, compoundProperties.ProtocolProperties.MappingWith(schemaItem.FormulationKey));
            if (formulation == null)
               throw new NoFormulationFoundForRouteException(protocol, schemaItem.ApplicationType);

            //check if used formulation container is already created and create if needed
            if (protocolGroupBuilder.ContainsName(formulation.Name))
               applicationParentContainer = protocolGroupBuilder.GetSingleChildByName<IEventGroupBuilder>(formulation.Name);
            else
               applicationParentContainer = createFormulationAsEventGroupBuilderFrom(formulation);

            protocolGroupBuilder.Add(applicationParentContainer);
            formulationType = formulation.FormulationType;

            formulationParameters = applicationParentContainer.GetChildren<IParameter>();
         }
         else
         {
            applicationParentContainer = protocolGroupBuilder;
            formulationType = CoreConstants.Formulation.EmptyFormulation;
            formulationParameters = new List<IParameter>();
         }

         applicationParentContainer.Add(
            _applicationFactory.CreateFor(schemaItem, formulationType, applicationName, compoundProperties.Compound.Name, formulationParameters, _eventGroupBuildingBlock.FormulaCache));
      }

      private IEventGroupBuilder createFormulationAsEventGroupBuilderFrom(Formulation formulation)
      {
         var formulationBuilder = _objectBaseFactory.Create<IEventGroupBuilder>();

         formulationBuilder.UpdatePropertiesFrom(formulation, _cloneManagerForBuildingBlock);
         foreach (var parameter in formulation.AllParameters())
         {
            var builderParameter = _cloneManagerForBuildingBlock.Clone(parameter);
            //reset the origin in any case since it might have been set by clone
            builderParameter.Origin.ParameterId = string.Empty;
            formulationBuilder.Add(builderParameter);
         }

         _parameterIdUpdater.UpdateBuildingBlockId(formulationBuilder, formulation);
         _parameterIdUpdater.UpdateParameterIds(formulation, formulationBuilder);

         if (formulation.FormulationType.Equals(CoreConstants.Formulation.Particles))
            setParticleRadiusDistributionParametersToLockedAndInvisible(formulationBuilder);

         return formulationBuilder;
      }

      private static void setParticleRadiusDistributionParametersToLockedAndInvisible(IContainer formulationBuilder)
      {
         // first, set all parameteres responsible for particle size distribution to locked
         CoreConstants.Parameter.ParticleDistributionStructuralParameters.Each(paramName => formulationBuilder.Parameter(paramName).Editable = false);

         // second, set some parameters to not visible depending on settings
         var parameterNamesToBeInvisible = new List<string> {CoreConstants.Parameter.PARTICLE_DISPERSE_SYSTEM};

         var numberOfBins = (int) formulationBuilder.Parameter(CoreConstants.Parameter.NUMBER_OF_BINS).Value;

         if (numberOfBins == 1)
            parameterNamesToBeInvisible.AddRange(CoreConstants.Parameter.HiddenParameterForMonodisperse);
         else
         {
            var particlesDistributionType = (int) formulationBuilder.Parameter(CoreConstants.Parameter.PARTICLE_SIZE_DISTRIBUTION).Value;

            if (particlesDistributionType == CoreConstants.Parameter.PARTICLE_SIZE_DISTRIBUTION_NORMAL)
               parameterNamesToBeInvisible.AddRange(CoreConstants.Parameter.HiddenParameterForPolydisperseNormal);
            else
               parameterNamesToBeInvisible.AddRange(CoreConstants.Parameter.HiddenParameterForPolydisperseLogNormal);
         }

         parameterNamesToBeInvisible.Each(paramName => formulationBuilder.Parameter(paramName).Visible = false);
      }
   }
}