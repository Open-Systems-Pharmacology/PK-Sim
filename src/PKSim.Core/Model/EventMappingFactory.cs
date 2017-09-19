using System.Linq;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IEventMappingFactory
   {
      EventMapping Create();
      EventMapping Create(PKSimEvent pkSimEvent);
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

      public EventMapping Create()
      {
         return Create(_buildingBlockRepository.All<PKSimEvent>().FirstOrDefault());
      }

      public EventMapping Create(PKSimEvent pkSimEvent)
      {
         return new EventMapping
         {
            TemplateEventId = pkSimEvent == null ? string.Empty : pkSimEvent.Id,
            StartTime = _parameterFactory.CreateFor(Constants.Parameters.START_TIME, 0, Constants.Dimension.TIME, PKSimBuildingBlockType.Event)
         };
      }
   }
}