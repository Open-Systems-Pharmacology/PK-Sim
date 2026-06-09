using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core;

namespace PKSim.Presentation.UICommands
{
   public class UndoCommand : IUICommand
   {
      private readonly ICoreWorkspace _workspace;

      public UndoCommand(ICoreWorkspace workspace)
      {
         _workspace = workspace;
      }

      public void Execute()
      {
         _workspace.HistoryManager.Undo();
      }
   }
}