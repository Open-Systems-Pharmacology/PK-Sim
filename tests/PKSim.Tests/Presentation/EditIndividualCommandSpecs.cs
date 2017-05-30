using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditIndividualCommand : ContextSpecification<EditIndividualCommand>
   {
      protected Individual _individual;
      protected IIndividualTask _individualTask;

      protected override void Context()
      {
         _individual = A.Fake<Individual>();
         _individualTask = A.Fake<IIndividualTask>();
         sut = new EditIndividualCommand(_individualTask);
         sut.For(_individual);
      }
   }

   public class When_the_edit_individual_command_is_editing_an_individual : concern_for_EditIndividualCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_individual_task_that_will_edit_the_given_individual()
      {
         A.CallTo(() => _individualTask.Edit(_individual)).MustHaveHappened();
      }
   }
}