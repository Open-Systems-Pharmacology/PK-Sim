using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportProjectToSnapshotCommand : IUICommand
   {
      private readonly IProjectTask _projectTask;

      public ExportProjectToSnapshotCommand(IProjectTask projectTask)
      {
         _projectTask = projectTask;
      }

      public async void Execute()
      {
         await _projectTask.SecureAwait(x => x.ExportCurrentProjectToSnapshot());
      }
   }
}