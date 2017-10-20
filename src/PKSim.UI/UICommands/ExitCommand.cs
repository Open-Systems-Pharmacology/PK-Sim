using System.Windows.Forms;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.UI.UICommands
{
   public class ExitCommand : IExitCommand
   {
      private readonly IProjectTask _projectTask;
      private readonly IUserSettingsPersistor _userSettingsPersitor;
      private readonly IApplicationSettingsPersitor _applicationSettingsPersitor;
      public bool Canceled { get; private set; }

      public ExitCommand(IProjectTask projectTask,
         IUserSettingsPersistor userSettingsPersitor,
         IApplicationSettingsPersitor applicationSettingsPersitor
      )
      {
         _projectTask = projectTask;
         _userSettingsPersitor = userSettingsPersitor;
         _applicationSettingsPersitor = applicationSettingsPersitor;
      }

      public void Execute()
      {
         Canceled = (_projectTask.CloseCurrentProject() == false);
         if (Canceled) return;

         _userSettingsPersitor.SaveCurrent();
         _applicationSettingsPersitor.SaveCurrent();

         Application.Exit();
      }
   }
}