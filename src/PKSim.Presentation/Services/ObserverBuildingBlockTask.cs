using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Services
{
   public class ObserverBuildingBlockTask : BuildingBlockTask<PKSimObserverBuildingBlock>, IObserverBuildingBlockTask
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ISimulationTransferLoader _simulationTransferLoader;

      public ObserverBuildingBlockTask(
         IExecutionContext executionContext,
         IBuildingBlockTask buildingBlockTask,
         IApplicationController applicationController,
         IObjectBaseFactory objectBaseFactory,
         ISimulationTransferLoader simulationTransferLoader
      ) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Observers)
      {
         _objectBaseFactory = objectBaseFactory;
         _simulationTransferLoader = simulationTransferLoader;
      }

      public override PKSimObserverBuildingBlock AddToProject()
      {
         return AddToProject<ICreateObserverBuildingBlockPresenter>();
      }


      public PKSimObserverBuildingBlock Create()
      {
         var observerBuildingBlock = _objectBaseFactory.Create<PKSimObserverBuildingBlock>();
         observerBuildingBlock.IsLoaded = true;
         return observerBuildingBlock;
      }
   }
}