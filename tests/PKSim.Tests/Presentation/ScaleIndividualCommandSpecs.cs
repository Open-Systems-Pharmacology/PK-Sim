using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ScaleIndividualCommand : ContextSpecification<ScaleIndividualCommand>
   {
      protected IIndividualTask _individualTask;
      protected Individual _individual;

      protected override void Context()
      {
         _individualTask = A.Fake<IIndividualTask>();
         _individual = A.Fake<Individual>();
         sut = new ScaleIndividualCommand(_individualTask);
         sut.For(_individual);
      }
   }

   public class When_told_to_execute : concern_for_ScaleIndividualCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_individual_task_that_will_scale_the_given_individual()
      {
         A.CallTo(() => _individualTask.ScaleIndividual(_individual)).MustHaveHappened();
      }
   }
}