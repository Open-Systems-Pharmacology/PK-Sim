using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.UICommands
{
   public class UndoCommand : IUICommand
   {
      private readonly IWorkspace _workspace;

      public UndoCommand(IWorkspace workspace)
      {
         _workspace = workspace;
      }

      public void Execute()
      {
         _workspace.HistoryManager.Undo();
      }
   }
}