using System;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.Services
{
   public interface IObserverBuildingBlockTask : IBuildingBlockTask<PKSimObserverBuildingBlock>
   {
   }

   public class ObserverBuildingBlockTask : BuildingBlockTask<PKSimObserverBuildingBlock>, IObserverBuildingBlockTask

   {
      public ObserverBuildingBlockTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Observers)
      {
      }

      public override PKSimObserverBuildingBlock AddToProject()
      {
         throw new NotImplementedException();
      }
   }
}