using PKSim.Presentation.Presenters.Main;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class HistoryVisibilityCommand : MainViewItemPresenterVisibilityCommand<IHistoryPresenter>
   {
      public HistoryVisibilityCommand(IApplicationController applicationController) : base(applicationController)
      {
      }
   }
}