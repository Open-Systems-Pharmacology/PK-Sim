using PKSim.Presentation.Presenters.Main;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class SimulationExplorerVisibilityCommand : MainViewItemPresenterVisibilityCommand<ISimulationExplorerPresenter>
   {
      public SimulationExplorerVisibilityCommand(IApplicationController applicationController) : base(applicationController)
      {
      }
   }
}