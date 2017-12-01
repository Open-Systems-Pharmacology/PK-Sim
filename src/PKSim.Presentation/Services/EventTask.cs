using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Events;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public interface IEventTask : IBuildingBlockTask<PKSimEvent>
   {
      PKSimEvent CreateEvent();
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

      public PKSimEvent CreateEvent()
      {
         return AddToProject<ICreateEventPresenter>();
      }

      public EventMapping CreateEventMapping()
      {
         return _eventMappingFactory.Create();
      }
   }
}