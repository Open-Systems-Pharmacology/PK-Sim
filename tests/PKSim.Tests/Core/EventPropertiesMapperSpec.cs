using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_EventPropertiesMapper : ContextSpecificationAsync<EventMappingMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IEventMappingFactory _eventMappingFactory;
      protected PKSimEvent _event;
      protected EventSelection _snapshot;
      protected Parameter _snapshotEventStartTime1;
      protected PKSimProject _project;
      protected EventMapping _eventMapping;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _eventMappingFactory = A.Fake<IEventMappingFactory>();
         sut = new EventMappingMapper(_parameterMapper, _eventMappingFactory);

         _event = new PKSimEvent()
            .WithName("E1")
            .WithId("EventId");

         _eventMapping = new EventMapping
         {
            TemplateEventId = _event.Id,
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(1)
         };

         _snapshotEventStartTime1 = new Parameter {Name = "P1"};

         _project = new PKSimProject();
         _project.AddBuildingBlock(_event);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_eventMapping.StartTime)).Returns(_snapshotEventStartTime1);

         return _completed;
      }
   }

   public class When_mapping_event_properties_to_snapshot : concern_for_EventPropertiesMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_eventMapping, _project);
      }

      [Observation]
      public void Observation()
      {
         _snapshot.Name.ShouldBeEqualTo(_event.Name);
         _snapshot.StartTime.ShouldBeEqualTo(_snapshotEventStartTime1);
      }

      [Observation]
      public void should_have_reset_the_start_time_parameter_name()
      {
         _snapshotEventStartTime1.Name.ShouldBeNull();
      }
   }

   public class When_mapping_a_snapshot_event_to_event_properties : concern_for_EventPropertiesMapper
   {
      private EventMapping _newEventMapping;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_eventMapping, _project);
         A.CallTo(() => _eventMappingFactory.Create(_event)).ReturnsLazily(x => new EventMapping
         {
            StartTime = DomainHelperForSpecs.ConstantParameterWithValue(0)
         });
      }

      protected override async Task Because()
      {
         _newEventMapping = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_map_start_time_parameters_according_to_snapshot()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot.StartTime, _newEventMapping.StartTime)).MustHaveHappened();
      }
   }
}