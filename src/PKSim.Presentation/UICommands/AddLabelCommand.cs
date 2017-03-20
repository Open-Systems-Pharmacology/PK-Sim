using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Core;
using OSPSuite.Presentation.Services.Commands;

namespace PKSim.Presentation.UICommands
{
   public class AddLabelCommand : IUICommand
   {
      private readonly ILabelTask _labelTask;
      private readonly IWorkspace _workspace;

      public AddLabelCommand(ILabelTask labelTask, IWorkspace workspace)
      {
         _labelTask = labelTask;
         _workspace = workspace;
      }

      public void Execute()
      {
         _labelTask.AddLabelTo(_workspace.HistoryManager);
      }
   }
}