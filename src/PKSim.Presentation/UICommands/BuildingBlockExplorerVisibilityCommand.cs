using PKSim.Presentation.Presenters.Main;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class BuildingBlockExplorerVisibilityCommand : MainViewItemPresenterVisibilityCommand<IBuildingBlockExplorerPresenter>
   {
      public BuildingBlockExplorerVisibilityCommand(IApplicationController applicationController) : base(applicationController)
      {
      }
   }
}