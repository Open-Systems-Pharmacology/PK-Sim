using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_NewIndividualCommand : ContextSpecification<AddIndividualCommand>
   {
      protected IIndividualTask _individualTask;

      protected override void Context()
      {
         _individualTask = A.Fake<IIndividualTask>();
         sut = new AddIndividualCommand(_individualTask);
      }
   }

   public class When_the_create_individual_command_is_being_executed : concern_for_NewIndividualCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverate_the_individual_task_to_create_an_individual()
      {
         A.CallTo(() => _individualTask.AddToProject()).MustHaveHappened();
      }
   }
}