using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Services
{
   public interface IObserverBuildingBlockTask : IBuildingBlockTask<PKSimObserverBuildingBlock>
   {
   }

   public class ObserverBuildingBlockTask : BuildingBlockTask<PKSimObserverBuildingBlock>, IObserverBuildingBlockTask

   {
      private readonly IDialogCreator _dialogCreator;

      public ObserverBuildingBlockTask(
         IExecutionContext executionContext,
         IBuildingBlockTask buildingBlockTask,
         IApplicationController applicationController,
         IDialogCreator dialogCreator
      ) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Observers)
      {
         _dialogCreator = dialogCreator;
      }

      public override PKSimObserverBuildingBlock AddToProject()
      {
         return AddToProject<ICreateObserverBuildingBlockPresenter>();
      }
   }
}