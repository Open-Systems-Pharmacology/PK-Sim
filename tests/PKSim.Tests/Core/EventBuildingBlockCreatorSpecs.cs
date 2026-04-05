using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_EventBuildingBlockCreator : ContextSpecification<IEventBuildingBlockCreator>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IProtocolToSchemaItemsMapper _schemaItemsMapper;
      protected IApplicationFactory _applicationFactory;
      protected IFormulationFromMappingRetriever _formulationFromMappingRetriever;
      protected ICloneManagerForBuildingBlock _cloneManagerForBuildingBlock;
      protected IParameterIdUpdater _parameterIdUpdater;
      protected IParameterSetUpdater _parameterSetUpdater;
      protected IEventGroupRepository _eventGroupRepository;
      protected IParameterDefaultStateUpdater _parameterDefaultStateUpdater;
      protected IndividualSimulation _simulation;
      protected EventGroupBuildingBlock _result;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _schemaItemsMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         _applicationFactory = A.Fake<IApplicationFactory>();
         _formulationFromMappingRetriever = A.Fake<IFormulationFromMappingRetriever>();
         _cloneManagerForBuildingBlock = A.Fake<ICloneManagerForBuildingBlock>();
         _parameterIdUpdater = A.Fake<IParameterIdUpdater>();
         _parameterSetUpdater = A.Fake<IParameterSetUpdater>();
         _eventGroupRepository = A.Fake<IEventGroupRepository>();
         _parameterDefaultStateUpdater = A.Fake<IParameterDefaultStateUpdater>();

         _simulation = new IndividualSimulation { Properties = new SimulationProperties() };

         A.CallTo(() => _objectBaseFactory.Create<EventGroupBuildingBlock>()).Returns(new EventGroupBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<EventGroupBuilder>()).Returns(new EventGroupBuilder());

         sut = new EventBuildingBlockCreator(
            _objectBaseFactory,
            _schemaItemsMapper,
            _applicationFactory,
            _formulationFromMappingRetriever,
            _cloneManagerForBuildingBlock,
            _parameterIdUpdater,
            _parameterSetUpdater,
            _eventGroupRepository,
            _parameterDefaultStateUpdater);
      }

      protected SchemaItem createEventSchemaItem(string eventKey, double startTimeValue)
      {
         var schemaItem = new SchemaItem { ApplicationType = ApplicationTypes.Event, EventKey = eventKey };
         var startTimeParameter = new PKSimParameter().WithName(Constants.Parameters.START_TIME);
         startTimeParameter.Value = startTimeValue;
         schemaItem.Add(startTimeParameter);
         return schemaItem;
      }

      protected EventGroupBuilder createTemplateEventGroup(string name)
      {
         var templateEventGroup = new EventGroupBuilder().WithName(name);
         var mainSubContainer = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         var startTimeParam = new PKSimParameter().WithName(Constants.Parameters.START_TIME);
         mainSubContainer.Add(startTimeParam);
         templateEventGroup.Add(mainSubContainer);
         return templateEventGroup;
      }
   }

   public class When_creating_event_building_block_with_protocol_event_schema_items : concern_for_EventBuildingBlockCreator
   {
      private PKSimEvent _pkSimEvent;
      private EventGroupBuilder _templateEventGroup;
      private EventGroupBuilder _clonedEventGroup;
      private EventGroupBuilder _clonedMainSub1;
      private EventGroupBuilder _clonedMainSub2;

      protected override void Context()
      {
         base.Context();

         _pkSimEvent = new PKSimEvent { TemplateName = "BladderEmptying" }.WithName("MyEvent").WithId("event-id");
         _templateEventGroup = createTemplateEventGroup("BladderEmptying");

         _clonedEventGroup = createTemplateEventGroup("MyEvent");
         _clonedMainSub1 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub1.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));
         _clonedMainSub2 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub2.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));

         var protocol = A.Fake<Protocol>();
         var protocolProperties = new ProtocolProperties { Protocol = protocol };
         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = "EVENT_1",
            TemplateEventId = _pkSimEvent.Id,
            Event = _pkSimEvent
         });

         var compoundProperties = new CompoundProperties { ProtocolProperties = protocolProperties };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_pkSimEvent.Id, PKSimBuildingBlockType.Event) { BuildingBlock = _pkSimEvent });
         _simulation.Properties.AddCompoundProperties(compoundProperties);

         var eventSchemaItem1 = createEventSchemaItem("EVENT_1", 10);
         var eventSchemaItem2 = createEventSchemaItem("EVENT_1", 20);

         A.CallTo(() => _schemaItemsMapper.MapFrom(protocol)).Returns(new List<SchemaItem> { eventSchemaItem1, eventSchemaItem2 });
         A.CallTo(() => _eventGroupRepository.All()).Returns(new[] { _templateEventGroup });
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup)).Returns(_clonedEventGroup);

         var callIndex = 0;
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup.MainSubContainer()))
            .ReturnsLazily(() => callIndex++ == 0 ? _clonedMainSub1 : _clonedMainSub2);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation);
      }

      [Observation]
      public void should_create_event_group_with_event_name()
      {
         var eventGroups = _result.OfType<EventGroupBuilder>().Where(x => x.Name == "MyEvent").ToList();
         eventGroups.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_create_two_sub_containers_for_two_schema_items_with_same_key()
      {
         var eventGroup = _result.OfType<EventGroupBuilder>().First(x => x.Name == "MyEvent");
         var subContainers = eventGroup.GetChildren<EventGroupBuilder>().ToList();
         subContainers.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_name_sub_containers_with_index()
      {
         var eventGroup = _result.OfType<EventGroupBuilder>().First(x => x.Name == "MyEvent");
         var subContainers = eventGroup.GetChildren<EventGroupBuilder>().ToList();
         subContainers[0].Name.ShouldBeEqualTo("MyEvent_1");
         subContainers[1].Name.ShouldBeEqualTo("MyEvent_2");
      }

      [Observation]
      public void should_update_building_block_ids()
      {
         A.CallTo(() => _parameterIdUpdater.UpdateBuildingBlockId(A<EventGroupBuilder>.That.Matches(x => x.Name == "MyEvent"), _pkSimEvent)).MustHaveHappened();
      }

      [Observation]
      public void should_set_event_group_parameters_from_pksim_event()
      {
         A.CallTo(() => _parameterSetUpdater.UpdateValuesByName(_pkSimEvent, A<EventGroupBuilder>.That.Matches(x => x.Name == "MyEvent"))).MustHaveHappened();
      }
   }

   public class When_creating_event_building_block_with_no_event_mapping_for_key : concern_for_EventBuildingBlockCreator
   {
      protected override void Context()
      {
         base.Context();

         var protocol = A.Fake<Protocol>();
         var protocolProperties = new ProtocolProperties { Protocol = protocol };
         var compoundProperties = new CompoundProperties { ProtocolProperties = protocolProperties };
         _simulation.Properties.AddCompoundProperties(compoundProperties);

         var eventSchemaItem = createEventSchemaItem("EVENT_1", 10);
         A.CallTo(() => _schemaItemsMapper.MapFrom(protocol)).Returns(new List<SchemaItem> { eventSchemaItem });
      }

      [Observation]
      public void should_throw_when_event_cannot_be_resolved()
      {
         The.Action(() => sut.CreateFor(_simulation)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_creating_event_building_block_with_different_keys_mapping_to_same_event : concern_for_EventBuildingBlockCreator
   {
      private PKSimEvent _pkSimEvent;
      private EventGroupBuilder _templateEventGroup;
      private EventGroupBuilder _clonedEventGroup;
      private EventGroupBuilder _clonedMainSub1;
      private EventGroupBuilder _clonedMainSub2;

      protected override void Context()
      {
         base.Context();

         _pkSimEvent = new PKSimEvent { TemplateName = "BladderEmptying" }.WithName("MyEvent").WithId("event-id");
         _templateEventGroup = createTemplateEventGroup("BladderEmptying");

         _clonedEventGroup = createTemplateEventGroup("MyEvent");
         _clonedMainSub1 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub1.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));
         _clonedMainSub2 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub2.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));

         var protocol = A.Fake<Protocol>();
         var protocolProperties = new ProtocolProperties { Protocol = protocol };

         // Two different keys, same event template
         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = "EVENT_1",
            TemplateEventId = _pkSimEvent.Id,
            Event = _pkSimEvent
         });
         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = "EVENT_2",
            TemplateEventId = _pkSimEvent.Id,
            Event = _pkSimEvent
         });

         var compoundProperties = new CompoundProperties { ProtocolProperties = protocolProperties };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_pkSimEvent.Id, PKSimBuildingBlockType.Event) { BuildingBlock = _pkSimEvent });
         _simulation.Properties.AddCompoundProperties(compoundProperties);

         // Different keys with different start times
         var eventSchemaItem1 = createEventSchemaItem("EVENT_1", 10);
         var eventSchemaItem2 = createEventSchemaItem("EVENT_2", 20);

         A.CallTo(() => _schemaItemsMapper.MapFrom(protocol)).Returns(new List<SchemaItem> { eventSchemaItem1, eventSchemaItem2 });
         A.CallTo(() => _eventGroupRepository.All()).Returns(new[] { _templateEventGroup });
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup)).Returns(_clonedEventGroup);

         var mainSubIndex = 0;
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup.MainSubContainer()))
            .ReturnsLazily(() => mainSubIndex++ == 0 ? _clonedMainSub1 : _clonedMainSub2);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation);
      }

      [Observation]
      public void should_create_single_event_group()
      {
         _result.OfType<EventGroupBuilder>().Count(x => x.Name == "MyEvent").ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_combine_start_times_into_two_sub_containers()
      {
         var eventGroup = _result.OfType<EventGroupBuilder>().First(x => x.Name == "MyEvent");
         eventGroup.GetChildren<EventGroupBuilder>().Count().ShouldBeEqualTo(2);
      }
   }

   public class When_creating_event_building_block_with_different_keys_mapping_to_same_event_with_duplicate_start_times : concern_for_EventBuildingBlockCreator
   {
      private PKSimEvent _pkSimEvent;
      private EventGroupBuilder _templateEventGroup;
      private EventGroupBuilder _clonedEventGroup;
      private EventGroupBuilder _clonedMainSub;

      protected override void Context()
      {
         base.Context();

         _pkSimEvent = new PKSimEvent { TemplateName = "BladderEmptying" }.WithName("MyEvent").WithId("event-id");
         _templateEventGroup = createTemplateEventGroup("BladderEmptying");

         _clonedEventGroup = createTemplateEventGroup("MyEvent");
         _clonedMainSub = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));

         var protocol = A.Fake<Protocol>();
         var protocolProperties = new ProtocolProperties { Protocol = protocol };

         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = "EVENT_1",
            TemplateEventId = _pkSimEvent.Id,
            Event = _pkSimEvent
         });
         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = "EVENT_2",
            TemplateEventId = _pkSimEvent.Id,
            Event = _pkSimEvent
         });

         var compoundProperties = new CompoundProperties { ProtocolProperties = protocolProperties };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_pkSimEvent.Id, PKSimBuildingBlockType.Event) { BuildingBlock = _pkSimEvent });
         _simulation.Properties.AddCompoundProperties(compoundProperties);

         // Same start time for both keys — should be deduplicated
         var eventSchemaItem1 = createEventSchemaItem("EVENT_1", 10);
         var eventSchemaItem2 = createEventSchemaItem("EVENT_2", 10);

         A.CallTo(() => _schemaItemsMapper.MapFrom(protocol)).Returns(new List<SchemaItem> { eventSchemaItem1, eventSchemaItem2 });
         A.CallTo(() => _eventGroupRepository.All()).Returns(new[] { _templateEventGroup });
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup)).Returns(_clonedEventGroup);
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup.MainSubContainer())).Returns(_clonedMainSub);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation);
      }

      [Observation]
      public void should_create_single_event_group()
      {
         _result.OfType<EventGroupBuilder>().Count(x => x.Name == "MyEvent").ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_deduplicate_and_create_only_one_sub_container()
      {
         var eventGroup = _result.OfType<EventGroupBuilder>().First(x => x.Name == "MyEvent");
         eventGroup.GetChildren<EventGroupBuilder>().Count().ShouldBeEqualTo(1);
      }
   }

   public class When_creating_event_building_block_with_same_event_used_standalone_and_in_protocol : concern_for_EventBuildingBlockCreator
   {
      private PKSimEvent _pkSimEvent;
      private EventGroupBuilder _templateEventGroup;
      private EventGroupBuilder _clonedEventGroup;
      private EventGroupBuilder _clonedMainSub1;
      private EventGroupBuilder _clonedMainSub2;
      private EventGroupBuilder _clonedMainSub3;

      protected override void Context()
      {
         base.Context();

         _pkSimEvent = new PKSimEvent { TemplateName = "BladderEmptying" }.WithName("MyEvent").WithId("event-id");
         _templateEventGroup = createTemplateEventGroup("BladderEmptying");

         _clonedEventGroup = createTemplateEventGroup("MyEvent");
         _clonedMainSub1 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub1.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));
         _clonedMainSub2 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub2.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));
         _clonedMainSub3 = new EventGroupBuilder().WithName(CoreConstants.ContainerName.EventGroupMainSubContainer);
         _clonedMainSub3.Add(new PKSimParameter().WithName(Constants.Parameters.START_TIME));

         // Standalone event mapping at t=5
         var standaloneStartTime = new PKSimParameter().WithName(Constants.Parameters.START_TIME);
         standaloneStartTime.Value = 5;
         _simulation.EventProperties.AddEventMapping(new EventMapping
         {
            TemplateEventId = _pkSimEvent.Id,
            StartTime = standaloneStartTime
         });

         // Protocol event mapping at t=10 and duplicate at t=5
         var protocol = A.Fake<Protocol>();
         var protocolProperties = new ProtocolProperties { Protocol = protocol };
         protocolProperties.AddEventPlaceholderMapping(new EventPlaceholderMapping
         {
            EventKey = "EVENT_1",
            TemplateEventId = _pkSimEvent.Id,
            Event = _pkSimEvent
         });

         var compoundProperties = new CompoundProperties { ProtocolProperties = protocolProperties };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_pkSimEvent.Id, PKSimBuildingBlockType.Event) { BuildingBlock = _pkSimEvent });
         _simulation.Properties.AddCompoundProperties(compoundProperties);

         var eventSchemaItem1 = createEventSchemaItem("EVENT_1", 10);
         var eventSchemaItem2 = createEventSchemaItem("EVENT_1", 5); // duplicate of standalone

         A.CallTo(() => _schemaItemsMapper.MapFrom(protocol)).Returns(new List<SchemaItem> { eventSchemaItem1, eventSchemaItem2 });
         A.CallTo(() => _eventGroupRepository.All()).Returns(new[] { _templateEventGroup });
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup)).Returns(_clonedEventGroup);

         var mainSubIndex = 0;
         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_templateEventGroup.MainSubContainer()))
            .ReturnsLazily(() =>
            {
               mainSubIndex++;
               return mainSubIndex == 1 ? _clonedMainSub1 : mainSubIndex == 2 ? _clonedMainSub2 : _clonedMainSub3;
            });
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation);
      }

      [Observation]
      public void should_create_single_event_group()
      {
         _result.OfType<EventGroupBuilder>().Count(x => x.Name == "MyEvent").ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_merge_start_times_and_deduplicate()
      {
         // t=5 from standalone + t=5 from protocol (deduplicated) + t=10 from protocol = 2 unique
         var eventGroup = _result.OfType<EventGroupBuilder>().First(x => x.Name == "MyEvent");
         eventGroup.GetChildren<EventGroupBuilder>().Count().ShouldBeEqualTo(2);
      }
   }
}
