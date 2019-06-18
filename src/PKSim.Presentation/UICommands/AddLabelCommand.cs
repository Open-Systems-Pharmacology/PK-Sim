using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Core;
using OSPSuite.Presentation.Services.Commands;
using PKSim.Core;

namespace PKSim.Presentation.UICommands
{
   public class AddLabelCommand : IUICommand
   {
      private readonly ILabelTask _labelTask;
      private readonly ICoreWorkspace _workspace;

      public AddLabelCommand(ILabelTask labelTask, ICoreWorkspace workspace)
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