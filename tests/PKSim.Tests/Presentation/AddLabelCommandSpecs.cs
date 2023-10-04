using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Services.Commands;
using PKSim.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_AddLabelCommand : ContextSpecification<AddLabelCommand>
   {
      protected ICoreWorkspace _workspace;
      protected ILabelTask _labelTask;

      protected override void Context()
      {
         _workspace = A.Fake<ICoreWorkspace>();
         _labelTask = A.Fake<ILabelTask>();
         sut = new AddLabelCommand(_labelTask, _workspace);
         A.CallTo(() => _workspace.HistoryManager).Returns(A.Fake<IHistoryManager<PKSimProject>>());
      }
   }

   public class When_adding_the_label : concern_for_AddLabelCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_add_a_label_to_the_current_history()
      {
         A.CallTo(() => _labelTask.AddLabelTo(_workspace.HistoryManager)).MustHaveHappened();
      }
   }
}