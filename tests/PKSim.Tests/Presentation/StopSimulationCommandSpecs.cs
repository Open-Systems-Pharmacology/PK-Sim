using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
   public abstract class concern_for_StopSimulationCommand : ContextSpecification<StopSimulationCommand> 
   {
      protected IInteractiveSimulationRunner _simulationRunner;

      protected override void Context()
      {
         _simulationRunner = A.Fake<IInteractiveSimulationRunner>();
         sut = new StopSimulationCommand(_simulationRunner);
      }
   }

   
   public class When_the_stop_simulation_command_is_being_executed : concern_for_StopSimulationCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_runner_to_stop_the_current_run()
      {
         A.CallTo(() => _simulationRunner.StopSimulation()).MustHaveHappened();
      }
   }
}	
