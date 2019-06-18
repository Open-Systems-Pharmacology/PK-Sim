using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Events;

namespace PKSim.Presentation.Services
{
   public interface IEventTask : IBuildingBlockTask<PKSimEvent>
   {
      EventMapping CreateEventMapping();
   }

   public class EventTask : BuildingBlockTask<PKSimEvent>, IEventTask
   {
      private readonly IEventMappingFactory _eventMappingFactory;

      public EventTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IEventMappingFactory eventMappingFactory, IApplicationController applicationController)
         : base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Event)
      {
         _eventMappingFactory = eventMappingFactory;
      }

      public override PKSimEvent AddToProject()
      {
         return AddToProject<ICreateEventPresenter>();
      }

      public EventMapping CreateEventMapping()
      {
         return _eventMappingFactory.Create();
      }
   }
}