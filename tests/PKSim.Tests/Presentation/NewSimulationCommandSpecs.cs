using OSPSuite.BDDHelper;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_NewSimulationCommand : ContextSpecification<NewSimulationCommand>
   {
      protected ISimulationTask _simulationTask;

      protected override void Context()
      {
         _simulationTask = A.Fake<ISimulationTask>();
         sut = new NewSimulationCommand(_simulationTask);
      }
   }

   
   public class When_executing_the_new_simulation_command : concern_for_NewSimulationCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_task_to_create_a_new_simulation()
      {
         A.CallTo(() => _simulationTask.AddToProject()).MustHaveHappened();
      }
   }
}