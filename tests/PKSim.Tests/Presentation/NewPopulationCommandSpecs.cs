using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_NewPopulationCommand : ContextSpecification<AddRandomPopulationCommand>
   {
      protected IPopulationTask _populationTask;

      protected override void Context()
      {
         _populationTask = A.Fake<IPopulationTask>();
         sut = new AddRandomPopulationCommand(_populationTask);
      }
   }

   public class When_executing_the_new_population_command : concern_for_NewPopulationCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_population_task_to_add_the_population_to_the_current_project()
      {
         A.CallTo(() => _populationTask.AddToProject()).MustHaveHappened();
      }
   }
}