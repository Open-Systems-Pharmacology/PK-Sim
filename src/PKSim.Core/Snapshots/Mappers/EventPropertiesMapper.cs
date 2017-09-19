using System.Linq;
using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class EventPropertiesMapper : SnapshotMapperBase<EventProperties, EventSelections, PKSimProject, PKSimProject>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly EventMappingFactory _eventMappingFactory;

      public EventPropertiesMapper(ParameterMapper parameterMapper, EventMappingFactory eventMappingFactory)
      {
         _parameterMapper = parameterMapper;
         _eventMappingFactory = eventMappingFactory;
      }

      public override async Task<EventSelections> MapToSnapshot(EventProperties eventProperties, PKSimProject project)
      {
         var eventMappings = eventProperties.EventMappings;
         if (!eventMappings.Any())
            return null;

         var tasks = eventProperties.EventMappings.Select(x => eventSelectionFrom(x, project));
         return new EventSelections(await Task.WhenAll(tasks));
      }

      private async Task<EventSelection> eventSelectionFrom(EventMapping eventMapping, PKSimProject project)
      {
         var eventBuildingBlock = project.BuildingBlockById(eventMapping.TemplateEventId);
         return new EventSelection
         {
            Name = eventBuildingBlock.Name,
            StartTime = await _parameterMapper.MapToSnapshot(eventMapping.StartTime)
         };
      }

      public override async Task<EventProperties> MapToModel(EventSelections snapshot, PKSimProject project)
      {
         var eventProperties = new EventProperties();
         if (snapshot == null)
            return eventProperties;

         var tasks = snapshot.Select(x => eventMappingFrom(project, x));

         eventProperties.AddEventMappings(await Task.WhenAll(tasks));
         return eventProperties;
      }

      private async Task<EventMapping> eventMappingFrom(PKSimProject project, EventSelection x)
      {
         var pksimEvent = project.BuildingBlockByName<PKSimEvent>(x.Name);
         var eventMapping = _eventMappingFactory.Create(pksimEvent);
         await _parameterMapper.MapToModel(x.StartTime, eventMapping.StartTime);
         return eventMapping;
      }
   }
}