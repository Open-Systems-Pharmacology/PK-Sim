using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants.Parameters;

namespace PKSim.Core.Services
{
   public interface IEventBuildingBlockCreator
   {
      /// <summary>
      ///    return the event building block built based on the given protocol and the associated formulation.
      ///    Special simulation event such as eat,sport etc.. should be managed in this class as well
      /// </summary>
      EventGroupBuildingBlock CreateFor(Simulation simulation);
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
      private EventGroupBuildingBlock _eventGroupBuildingBlock;
      private readonly IParameterDefaultStateUpdater _parameterDefaultStateUpdater;

      public EventBuildingBlockCreator(IObjectBaseFactory objectBaseFactory,
         IProtocolToSchemaItemsMapper schemaItemsMapper,
         IApplicationFactory applicationFactory,
         IFormulationFromMappingRetriever formulationFromMappingRetriever,
         ICloneManagerForBuildingBlock cloneManagerForBuildingBlock,
         IParameterIdUpdater parameterIdUpdater,
         IParameterSetUpdater parameterSetUpdater,
         IEventGroupRepository eventGroupRepository,
         IParameterDefaultStateUpdater parameterDefaultStateUpdater)
      {
         _objectBaseFactory = objectBaseFactory;
         _schemaItemsMapper = schemaItemsMapper;
         _applicationFactory = applicationFactory;
         _formulationFromMappingRetriever = formulationFromMappingRetriever;
         _cloneManagerForBuildingBlock = cloneManagerForBuildingBlock;
         _parameterIdUpdater = parameterIdUpdater;
         _parameterSetUpdater = parameterSetUpdater;
         _eventGroupRepository = eventGroupRepository;
         _parameterDefaultStateUpdater = parameterDefaultStateUpdater;
      }

      public EventGroupBuildingBlock CreateFor(Simulation simulation)
      {
         try
         {
            _simulation = simulation;
            _eventGroupBuildingBlock = _objectBaseFactory.Create<EventGroupBuildingBlock>().WithName(DefaultNames.EventBuildingBlock);
            _cloneManagerForBuildingBlock.FormulaCache = _eventGroupBuildingBlock.FormulaCache;

            createApplications(_simulation.CompoundPropertiesList);

            createEvents();

            _parameterDefaultStateUpdater.UpdateDefaultFor(_eventGroupBuildingBlock);

            return _eventGroupBuildingBlock;
         }
         finally
         {
            _simulation = null;
            _eventGroupBuildingBlock = null;
         }
      }

      private void createEvents()
      {
         var eventStartTimes = collectEventStartTimes();

         foreach (var entry in eventStartTimes)
         {
            createEventGroup(entry.Event, entry.UniqueStartTimes);
         }
      }

      private void createEventGroup(PKSimEvent pkSimEvent, IReadOnlyList<IParameter> startTimes)
      {
         var templateEventGroup = _eventGroupRepository.FindByName(pkSimEvent.TemplateName);

         var eventGroup = _cloneManagerForBuildingBlock.Clone(templateEventGroup);
         eventGroup.Name = pkSimEvent.Name;
         eventGroup.RemoveChild(eventGroup.MainSubContainer());

         _parameterSetUpdater.UpdateValuesByName(pkSimEvent, eventGroup);

         int eventIndex = 0;

         foreach (var startTime in startTimes.OrderBy(st => st.Value))
         {
            var mainSubContainer = _cloneManagerForBuildingBlock.Clone(templateEventGroup.MainSubContainer());

            eventIndex += 1;
            mainSubContainer.Name = $"{pkSimEvent.Name}_{eventIndex}";

            var eventStartTime = mainSubContainer.StartTime();
            _parameterSetUpdater.UpdateValue(startTime, eventStartTime);
            allowNegativeStartTime(eventStartTime);

            eventGroup.Add(mainSubContainer);
         }

         _parameterIdUpdater.UpdateBuildingBlockId(eventGroup, pkSimEvent);

         _eventGroupBuildingBlock.Add(eventGroup);
      }

      //an event may be scheduled before an administration (negative protocol offset), so its start time must allow negative values
      private static void allowNegativeStartTime(IParameter startTime)
      {
         if (startTime?.Info == null) return;

         startTime.Info.MinValue = null;
         startTime.Info.MinIsAllowed = true;
      }

      /// <summary>
      ///    Collects unique start times grouped by PKSimEvent from both standalone event mappings
      ///    and protocol event placeholder mappings. Duplicate start time values are removed.
      /// </summary>
      private Cache<string, EventStartTimeCollection> collectEventStartTimes()
      {
         var result = new Cache<string, EventStartTimeCollection>(x => x.Event.Id);
         var eventStartTimeFor = eventStartTimeForDef(result);

         // Standalone events
         foreach (var eventMapping in _simulation.EventProperties.EventMappings)
         {
            var pkSimEvent = resolveEvent(eventMapping.TemplateEventId);
            eventStartTimeFor(pkSimEvent).AddStartTime(eventMapping.StartTime);
         }

         // Protocol events
         foreach (var protocolProperties in _simulation.CompoundPropertiesList.Select(x => x.ProtocolProperties))
         {
            if (protocolProperties.Protocol == null)
               continue;

            foreach (var schemaItem in _schemaItemsMapper.MapFrom(protocolProperties.Protocol).Where(item => item.IsEvent))
            {
               var mapping = protocolProperties.EventMappingWith(schemaItem.EventKey);
               var pkSimEvent = resolveEvent(mapping?.TemplateEventId);
               eventStartTimeFor(pkSimEvent).AddStartTime(schemaItem.StartTime);
            }
         }

         return result;
      }

      private PKSimEvent resolveEvent(string templateEventId)
      {
         var usedBuildingBlock = string.IsNullOrEmpty(templateEventId) ? null : _simulation.UsedBuildingBlockByTemplateId(templateEventId);
         var pkSimEvent = usedBuildingBlock?.BuildingBlock.DowncastTo<PKSimEvent>();

         if (pkSimEvent == null)
            throw new PKSimException($"Could not resolve event building block for template id '{templateEventId}'.");

         return pkSimEvent;
      }

      private static Func<PKSimEvent, EventStartTimeCollection> eventStartTimeForDef(Cache<string, EventStartTimeCollection> cache) => pkSimEvent =>
      {
         if (!cache.Contains(pkSimEvent.Id))
            cache.Add(new EventStartTimeCollection(pkSimEvent));

         return cache[pkSimEvent.Id];
      };

      private void createApplications(IReadOnlyList<CompoundProperties> compoundPropertiesList)
      {
         var administrations = compoundPropertiesList
            .Where(x => x.ProtocolProperties.Protocol != null)
            .Select(x => new AdministrationContext(x))
            .ToList();

         assignUniqueEventGroupNames(administrations);

         administrations.Each(x => addProtocol(x.CompoundProperties, x.EventGroupName));
      }


      private static void assignUniqueEventGroupNames(IReadOnlyList<AdministrationContext> administrations)
      {
         var sharedProtocolNames = protocolNamesUsedByMoreThanOneCompound(administrations);

         // assign event group names. For non shared, use the protocol names, for shared, use a composite name with the compound name
         createEventGroupNamesForProtocols(administrations, sharedProtocolNames);
         
         // if a non-shared protocol event group has the same name as a shared composite, add the compound name to the non-shared
         adjustNonSharedProtocolNames(administrations, sharedProtocolNames);

         // there could still be name collisions. Use indexes to guarantee uniqueness
         var usedNames = new HashSet<string>();
         administrations.Each(x => x.EventGroupName = indexedNameFrom(x.EventGroupName, usedNames));
      }

      private static void adjustNonSharedProtocolNames(IReadOnlyList<AdministrationContext> administrations, ICollection<string> sharedProtocolNames)
      {
         administrations
            .Where(x => protocolIsNotShared(sharedProtocolNames, x) && eventGroupNameIsDuplicated(administrations, x.EventGroupName))
            .ToList()
            .Each(x => x.EventGroupName = x.CompositeName);
      }

      private static bool protocolIsNotShared(ICollection<string> sharedProtocolNames, AdministrationContext x) => 
         !sharedProtocolNames.Contains(x.ProtocolName);

      private static void createEventGroupNamesForProtocols(IEnumerable<AdministrationContext> administrations, ICollection<string> sharedProtocolNames) =>
         administrations.Each(x => x.EventGroupName = sharedProtocolNames.Contains(x.ProtocolName) ? x.CompositeName : x.ProtocolName);

      private static ICollection<string> protocolNamesUsedByMoreThanOneCompound(IEnumerable<AdministrationContext> administrations) =>
         new HashSet<string>(administrations.GroupBy(x => x.ProtocolName).Where(x => x.Count() > 1).Select(x => x.Key));

      private static bool eventGroupNameIsDuplicated(IEnumerable<AdministrationContext> administrations, string eventGroupName) =>
         administrations.Count(x => string.Equals(x.EventGroupName, eventGroupName)) > 1;

      private static string indexedNameFrom(string name, ISet<string> usedNames)
      {
         var uniqueName = name;
         var index = 2;
         while (!usedNames.Add(uniqueName))
            uniqueName = $"{name}_{index++}";

         return uniqueName;
      }

      //Associates an administered compound with the event group name that will represent its protocol in the model.
      private class AdministrationContext
      {
         public AdministrationContext(CompoundProperties compoundProperties)
         {
            CompoundProperties = compoundProperties;
         }

         public CompoundProperties CompoundProperties { get; }
         public string ProtocolName => CompoundProperties.ProtocolProperties.Protocol.Name;
         public string CompositeName => Constants.CompositeNameFor(ProtocolName, CompoundProperties.Compound.Name);
         public string EventGroupName { get; set; }
      }

      private void addProtocol(CompoundProperties compoundProperties, string eventGroupName)
      {
         var protocol = compoundProperties.ProtocolProperties.Protocol;

         var eventGroup = _objectBaseFactory.Create<EventGroupBuilder>()
            .WithName(eventGroupName);

         eventGroup.SourceCriteria.Add(new MatchTagCondition(CoreConstants.Tags.EVENTS));

         //Filter out event schema items - they are not applications.
         _schemaItemsMapper.MapFrom(protocol).Where(item => !item.IsEvent).ToList().Each((schemaItem, index) =>
         {
            //+1 to start at 1 for the nomenclature
            var applicationName = $"{CoreConstants.APPLICATION_NAME_TEMPLATE}{index + 1}";
            addApplication(eventGroup, schemaItem, applicationName, compoundProperties, protocol);
         });

         _parameterIdUpdater.UpdateBuildingBlockId(eventGroup, protocol);

         _eventGroupBuildingBlock.Add(eventGroup);
      }

      private void addApplication(EventGroupBuilder protocolGroupBuilder, ISchemaItem schemaItem, string applicationName, CompoundProperties compoundProperties, Protocol protocol)
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
               applicationParentContainer = protocolGroupBuilder.GetSingleChildByName<EventGroupBuilder>(formulation.Name);
            else
               applicationParentContainer = createFormulationAsEventGroupBuilderFrom(formulation);

            protocolGroupBuilder.Add(applicationParentContainer);
            formulationType = formulation.FormulationType;
            formulationParameters = applicationParentContainer.GetChildren<IParameter>();
         }
         else
         {
            applicationParentContainer = protocolGroupBuilder;
            formulationType = CoreConstants.Formulation.EMPTY_FORMULATION;
            formulationParameters = new List<IParameter>();
         }

         applicationParentContainer.Add(
            _applicationFactory.CreateFor(schemaItem, formulationType, applicationName, compoundProperties.Compound.Name, formulationParameters, _eventGroupBuildingBlock.FormulaCache));
      }

      private EventGroupBuilder createFormulationAsEventGroupBuilderFrom(Formulation formulation)
      {
         var formulationBuilder = _objectBaseFactory.Create<EventGroupBuilder>();

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

         if (formulation.FormulationType.Equals(CoreConstants.Formulation.PARTICLES))
            setParticleRadiusDistributionParametersToLockedAndInvisible(formulationBuilder);

         return formulationBuilder;
      }

      private static void setParticleRadiusDistributionParametersToLockedAndInvisible(IContainer formulationBuilder)
      {
         // first, set all parameters responsible for particle size distribution to locked
         CoreConstants.Parameters.ParticleDistributionStructuralParameters.Each(paramName => formulationBuilder.Parameter(paramName).Editable = false);

         // second, set some parameters to not visible depending on settings
         var parameterNamesToBeInvisible = new List<string> { PARTICLE_DISPERSE_SYSTEM };

         var numberOfBins = (int)formulationBuilder.Parameter(NUMBER_OF_BINS).Value;

         if (numberOfBins == 1)
            parameterNamesToBeInvisible.AddRange(CoreConstants.Parameters.HiddenParameterForMonodisperse);
         else
         {
            var particlesDistributionType = (int)formulationBuilder.Parameter(PARTICLE_SIZE_DISTRIBUTION).Value;

            if (particlesDistributionType == CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL)
               parameterNamesToBeInvisible.AddRange(CoreConstants.Parameters.HiddenParameterForPolydisperseNormal);
            else
               parameterNamesToBeInvisible.AddRange(CoreConstants.Parameters.HiddenParameterForPolydisperseLogNormal);
         }

         parameterNamesToBeInvisible.Each(paramName => formulationBuilder.Parameter(paramName).Visible = false);
      }

      private class EventStartTimeCollection
      {
         private readonly HashSet<double> _seenValues = new HashSet<double>();
         private readonly List<IParameter> _startTimes = new List<IParameter>();

         public PKSimEvent Event { get; }

         public IReadOnlyList<IParameter> UniqueStartTimes => _startTimes;

         public EventStartTimeCollection(PKSimEvent pkSimEvent)
         {
            Event = pkSimEvent;
         }

         public void AddStartTime(IParameter startTime)
         {
            if (_seenValues.Add(startTime.Value))
               _startTimes.Add(startTime);
         }
      }
   }
}