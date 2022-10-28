using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public class concern_for_GlobalPKAnalysisRunner : ContextSpecification<GlobalPKAnalysisRunner>
   {
      protected IndividualSimulation _simulation;
      private IRegistrationTask _registrationTask;
      protected ISimulationRunner _simulationRunner;
      private ISimulationFactory _simulationFactory;

      protected override void Context()
      {
         _simulationRunner = A.Fake<ISimulationRunner>();
         _registrationTask = A.Fake<IRegistrationTask>();
         _simulationFactory = A.Fake<ISimulationFactory>();

         sut = new GlobalPKAnalysisRunner(_simulationRunner, _simulationFactory, _registrationTask);
         _simulation = new IndividualSimulation();
      }
   }

   public class when_running_a_simulation_for_ddi_calculations : concern_for_GlobalPKAnalysisRunner
   {
      protected override void Because()
      {
         sut.RunForDDIRatio(_simulation);
      }

      [Observation]
      public void the_runner_should_not_raise_events_when_running_ddi_simulations()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(A<Simulation>._, A<SimulationRunOptions>.That.Matches(x => x.RaiseEvents == false))).MustHaveHappened();
      }
   }

   public class when_running_a_simulation_for_bioavailability_calculations : concern_for_GlobalPKAnalysisRunner
   {
      protected override void Because()
      {
         sut.RunForBioavailability(new SimpleProtocol(), _simulation, new Compound());
      }

      [Observation]
      public void the_runner_should_not_raise_events_when_running_bioavailability_simulations()
      {
         A.CallTo(() => _simulationRunner.RunSimulation(A<Simulation>._, A<SimulationRunOptions>.That.Matches(x => x.RaiseEvents == false))).MustHaveHappened();
      }
   }
}
