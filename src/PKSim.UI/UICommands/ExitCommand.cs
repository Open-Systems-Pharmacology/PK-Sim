using System.Windows.Forms;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.UI.UICommands
{
   public class ExitCommand : IExitCommand
   {
      private readonly IProjectTask _projectTask;
      private readonly IUserSettingsPersistor _userSettingsPersistor;
      private readonly IApplicationSettingsPersistor _applicationSettingsPersistor;
      public bool Canceled { get; private set; }

      public ExitCommand(IProjectTask projectTask,
         IUserSettingsPersistor userSettingsPersistor,
         IApplicationSettingsPersistor applicationSettingsPersistor
      )
      {
         _projectTask = projectTask;
         _userSettingsPersistor = userSettingsPersistor;
         _applicationSettingsPersistor = applicationSettingsPersistor;
      }

      public void Execute()
      {
         Canceled = (_projectTask.CloseCurrentProject() == false);
         if (Canceled) return;

         _userSettingsPersistor.SaveCurrent();
         _applicationSettingsPersistor.SaveCurrent();

         Application.Exit();
      }
   }
}