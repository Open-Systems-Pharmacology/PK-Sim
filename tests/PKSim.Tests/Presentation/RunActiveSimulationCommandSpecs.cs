using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_RunActiveSimulationCommand : ContextSpecification<RunActiveSimulationCommand>
   {
      protected ISimulationRunner _simulationRunner;
      protected IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _simulationRunner = A.Fake<ISimulationRunner>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         sut = new RunActiveSimulationCommand(_simulationRunner, _activeSubjectRetriever);
      }
   }

   
   public class When_ask_to_run_the_simulation_for_the_active_simulation : concern_for_RunActiveSimulationCommand
   {
      private  PKSim.Core.Model.Simulation _activeSimulation;

      protected override void Context()
      {
         base.Context();
         _activeSimulation = A.Fake< PKSim.Core.Model.Simulation>();
         A.CallTo(() => _activeSubjectRetriever.Active< PKSim.Core.Model.Simulation>()).Returns(_activeSimulation);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_runner_and_tell_him_to_run_the_simulation()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(_activeSimulation,false)).MustHaveHappened();
      }

   }
}