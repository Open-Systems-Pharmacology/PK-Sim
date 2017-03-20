using System.Linq;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IEventMappingFactory
   {
      IEventMapping Create();
      IEventMapping Create(PKSimEvent pkSimEvent);
   }

   public class EventMappingFactory : IEventMappingFactory
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IParameterFactory _parameterFactory;

      public EventMappingFactory(IBuildingBlockRepository buildingBlockRepository, IParameterFactory parameterFactory)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _parameterFactory = parameterFactory;
      }

      public IEventMapping Create()
      {
         return Create(_buildingBlockRepository.All<PKSimEvent>().FirstOrDefault());
      }

      public IEventMapping Create(PKSimEvent pkSimEvent)
      {
         var eventMapping = new EventMapping();
         eventMapping.TemplateEventId = pkSimEvent == null ? string.Empty : pkSimEvent.Id;
         eventMapping.StartTime = _parameterFactory.CreateFor(Constants.Parameters.START_TIME, 0, Constants.Dimension.TIME, PKSimBuildingBlockType.Event);
         return eventMapping;
      }
   }
}