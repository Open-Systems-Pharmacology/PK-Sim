using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_EventPropertiesMapper : ContextSpecificationAsync<EventPropertiesMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IEventMappingFactory _eventMappingFactory;
      protected PKSimEvent _event;
      protected EventProperties _eventProperties;
      protected EventSelections _snapshot;
      protected Parameter _snapshotEventStartTime1;
      protected Parameter _snapshotEventStartTime2;
      protected PKSimProject _project;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _eventMappingFactory = A.Fake<IEventMappingFactory>();
         sut = new EventPropertiesMapper(_parameterMapper, _eventMappingFactory);


         _event = new PKSimEvent()
            .WithName("E1")
            .WithId("EventId");

         _eventProperties = new EventProperties();
         _eventProperties.AddEventMapping(new EventMapping
         {
            TemplateEventId = _event.Id,
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(1)
         });


         _eventProperties.AddEventMapping(new EventMapping
         {
            TemplateEventId = _event.Id,
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(2)
         });

         _snapshotEventStartTime1 = new Parameter {Name = "P1"};
         _snapshotEventStartTime2 = new Parameter { Name = "P2" };

         _project = new PKSimProject();
         _project.AddBuildingBlock(_event);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_eventProperties.EventMappings[0].StartTime)).Returns(_snapshotEventStartTime1);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_eventProperties.EventMappings[1].StartTime)).Returns(_snapshotEventStartTime2);


         return Task.FromResult(true);
      }
   }

   public class When_mapping_event_properties_to_snapshot : concern_for_EventPropertiesMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_eventProperties, _project);
      }

      [Observation]
      public void Observation()
      {
         _snapshot.ElementAt(0).Name.ShouldBeEqualTo(_event.Name);
         _snapshot.ElementAt(0).StartTime.ShouldBeEqualTo(_snapshotEventStartTime1);
         _snapshot.ElementAt(1).Name.ShouldBeEqualTo(_event.Name);
         _snapshot.ElementAt(1).StartTime.ShouldBeEqualTo(_snapshotEventStartTime2);
      }

      [Observation]
      public void should_have_reset_the_start_time_parameter_name()
      {
         _snapshotEventStartTime1.Name.ShouldBeNull();
         _snapshotEventStartTime2.Name.ShouldBeNull();
      }
   }

   public class When_mapping_a_snapshot_event_to_event_properties : concern_for_EventPropertiesMapper
   {
      private EventProperties _newEventProperties;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_eventProperties, _project);
         A.CallTo(() => _eventMappingFactory.Create(_event)).ReturnsLazily(x => new EventMapping
         {
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(0)
         });
      }

      protected override async Task Because()
      {
         _newEventProperties = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_return_event_properties_with_mapping_set_as_expected()
      {
         _newEventProperties.EventMappings.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_map_start_time_parameters_according_to_snapshot()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot.ElementAt(0).StartTime, _newEventProperties.EventMappings[0].StartTime)).MustHaveHappened();
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot.ElementAt(1).StartTime, _newEventProperties.EventMappings[1].StartTime)).MustHaveHappened();
      }
   }
}