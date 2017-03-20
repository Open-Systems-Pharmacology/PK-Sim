using System.Windows.Forms;
using PKSim.Presentation;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.UI.UICommands
{
   public class ExitCommand : IExitCommand
   {
      private readonly IProjectTask _projectTask;
      private readonly IUserSettingsPersistor _userSettingsPersitor;
      private readonly IUserSettings _userSettings;
      public bool Canceled { get; private set; }
      public bool ShouldCloseApplication { get; set; }

      public ExitCommand(IProjectTask projectTask, IUserSettingsPersistor userSettingsPersitor, IUserSettings userSettings)
      {
         _projectTask = projectTask;
         _userSettingsPersitor = userSettingsPersitor;
         _userSettings = userSettings;
         ShouldCloseApplication = true;
      }

      public void Execute()
      {
         Canceled = (_projectTask.CloseCurrentProject() == false);
         if (Canceled) return;

         _userSettingsPersitor.Save(_userSettings);

         if (!ShouldCloseApplication) return;

         Application.Exit();
      }
   }
}