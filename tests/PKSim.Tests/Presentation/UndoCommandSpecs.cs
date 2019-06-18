using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Presentation.Core;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_UndoCommand : ContextSpecification<UndoCommand>
   {
      private ICoreWorkspace _workspace;
      protected IHistoryManager _historyManager;

      protected override void Context()
      {
         _workspace = A.Fake<ICoreWorkspace>();
         _historyManager = A.Fake<IHistoryManager>();
         A.CallTo(() => _workspace.HistoryManager).Returns(_historyManager);
         sut = new UndoCommand(_workspace);
      }
   }

   public class When_executing_the_undo_command : concern_for_UndoCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_retrieve_the_history_manager_for_the_active_workspace_and_trigger_the_undo()
      {
         A.CallTo(() => _historyManager.Undo()).MustHaveHappened();
      }
   }
}