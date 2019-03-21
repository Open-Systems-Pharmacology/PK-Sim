using System.Collections.Generic;
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
      private readonly ICoreLoader _coreLoader;

      public ObserverBuildingBlockTask(
         IExecutionContext executionContext,
         IBuildingBlockTask buildingBlockTask,
         IApplicationController applicationController,
         IObjectBaseFactory objectBaseFactory,
         ICoreLoader coreLoader
      ) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Observers)
      {
         _objectBaseFactory = objectBaseFactory;
         _coreLoader = coreLoader;
      }

      public override PKSimObserverBuildingBlock AddToProject()
      {
         return AddToProject<ICreateObserverBuildingBlockPresenter>();
      }

      public IObserverBuilder LoadObserverFrom(string fileName)
      {
         return _coreLoader.LoadObserver(fileName);
      }

      public PKSimObserverBuildingBlock CreateWith(IEnumerable<IObserverBuilder> observers)
      {
         var observerBuildingBlock = _objectBaseFactory.Create<PKSimObserverBuildingBlock>();
         observerBuildingBlock.AddChildren(observers);
         observerBuildingBlock.IsLoaded = true;
         return observerBuildingBlock;
      }
   }
}