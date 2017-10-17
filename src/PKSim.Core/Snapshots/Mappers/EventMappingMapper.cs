using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class EventMappingMapper : SnapshotMapperBase<EventMapping, EventSelection, PKSimProject, PKSimProject>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly IEventMappingFactory _eventMappingFactory;

      public EventMappingMapper(ParameterMapper parameterMapper, IEventMappingFactory eventMappingFactory)
      {
         _parameterMapper = parameterMapper;
         _eventMappingFactory = eventMappingFactory;
      }

      public override async Task<EventSelection> MapToSnapshot(EventMapping eventMapping, PKSimProject project)
      {
         var eventBuildingBlock = project.BuildingBlockById(eventMapping.TemplateEventId);
         var snapshot = await SnapshotFrom(eventMapping, x => { x.Name = eventBuildingBlock.Name; });

         snapshot.StartTime = await _parameterMapper.MapToSnapshot(eventMapping.StartTime);
         //reset name as name is clear from context already
         snapshot.StartTime.Name = null;
         return snapshot;
      }

      public override async Task<EventMapping> MapToModel(EventSelection snapshot, PKSimProject project)
      {
         var pksimEvent = project.BuildingBlockByName<PKSimEvent>(snapshot.Name);
         var eventMapping = _eventMappingFactory.Create(pksimEvent);
         await _parameterMapper.MapToModel(snapshot.StartTime, eventMapping.StartTime);
         return eventMapping;
      }
   }
}