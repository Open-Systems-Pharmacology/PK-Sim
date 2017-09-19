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
      private ParameterMapper _parameterMapper;
      private IEventMappingFactory _eventMappingFactory;
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

         _snapshotEventStartTime1 = new Parameter();
         _snapshotEventStartTime2 = new Parameter();

         _project = new PKSimProject();
         _project.AddBuildingBlock(_event);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_eventProperties.EventMappings[0].StartTime)).ReturnsAsync(_snapshotEventStartTime1);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_eventProperties.EventMappings[1].StartTime)).ReturnsAsync(_snapshotEventStartTime2);


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
   }
}