using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class SettingsCommand : RunPresenterCommand<ISettingsPresenter>
   {
      public SettingsCommand(IApplicationController applicationController) : base(applicationController)
      {
      }
   }
}