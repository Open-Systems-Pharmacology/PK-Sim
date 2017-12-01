using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_RunActiveSimulationCommand : ContextSpecification<RunSimulationCommand>
   {
      protected IInteractiveSimulationRunner _simulationRunner;
      protected IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _simulationRunner = A.Fake<IInteractiveSimulationRunner>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         sut = new RunSimulationCommand(_simulationRunner, _activeSubjectRetriever);
      }
   }

   public class When_ask_to_run_the_simulation_for_the_active_simulation : concern_for_RunActiveSimulationCommand
   {
      private Simulation _activeSimulation;

      protected override void Context()
      {
         base.Context();
         _activeSimulation = A.Fake<Simulation>();
         A.CallTo(() => _activeSubjectRetriever.Active<Simulation>()).Returns(_activeSimulation);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_runner_and_tell_him_to_run_the_simulation()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(_activeSimulation, false)).MustHaveHappened();
      }
   }
}