using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ShowAboutUICommand : RunPresenterCommand<IAboutPresenter>
   {
      public ShowAboutUICommand(IApplicationController applicationController) : base(applicationController)
      {
      }
   }
}