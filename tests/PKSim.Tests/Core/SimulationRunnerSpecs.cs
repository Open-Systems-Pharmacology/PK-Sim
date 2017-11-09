using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationRunner : ContextSpecificationAsync<ISimulationRunner>
   {
      protected ISimulationEngine<IndividualSimulation> _simulationEngine;
      protected ISimulationEngine<PopulationSimulation> _popSimulationEngine;
      protected ISimulationEngineFactory _simulationEngineFactory;
      protected ILazyLoadTask _lazyLoadTask;
      protected IEntityValidationTask _entityTask;
      private ISimulationPersistableUpdater _simulationPersistableUpdater;
      protected SimulationRunOptions _simulationRunOptions;

      protected override Task Context()
      {
         _simulationEngine = A.Fake<ISimulationEngine<IndividualSimulation>>();
         _popSimulationEngine = A.Fake<ISimulationEngine<PopulationSimulation>>();
         _simulationEngineFactory = A.Fake<ISimulationEngineFactory>();
         _simulationPersistableUpdater = A.Fake<ISimulationPersistableUpdater>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _entityTask = A.Fake<IEntityValidationTask>();
         A.CallTo(() => _simulationEngineFactory.Create<PopulationSimulation>()).Returns(_popSimulationEngine);
         A.CallTo(() => _simulationEngineFactory.Create<IndividualSimulation>()).Returns(_simulationEngine);

         sut = new SimulationRunner(_simulationEngineFactory, _lazyLoadTask, _entityTask, _simulationPersistableUpdater);

         _simulationRunOptions = new SimulationRunOptions();
         return _completed;
      }
   }

   public class When_the_simulation_runner_was_told_to_run_a_simulation : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulationRunOptions = new SimulationRunOptions();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _entityTask.Validate(_simulation)).Returns(true);
      }

      protected override Task Because()
      {
         return sut.RunSimulation(_simulation, _simulationRunOptions);
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _lazyLoadTask.Load<Simulation>(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_activate_the_simulation_engine_to_run_with_the_active_simulation()
      {
         A.CallTo(() => _simulationEngine.RunAsync(_simulation, _simulationRunOptions)).MustHaveHappened();
      }
   }

   public class When_the_simulation_runner_was_told_to_stop_the_acticve_run : concern_for_SimulationRunner
   {
      private Simulation _activeSimulation;

      protected override async Task Context()
      {
         await base.Context();
         _activeSimulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _entityTask.Validate(_activeSimulation)).Returns(true);
         await sut.RunSimulation(_activeSimulation, _simulationRunOptions);
      }

      protected override Task Because()
      {
         sut.StopSimulation();
         return _completed;
      }

      [Observation]
      public void should_stop_the_simulation_engine()
      {
         A.CallTo(() => _simulationEngine.Stop()).MustHaveHappened();
      }
   }

   public class When_the_simulation_runner_is_told_to_stop_a_simulation_that_was_never_started : concern_for_SimulationRunner
   {
      protected override Task Because()
      {
         sut.StopSimulation();
         return _completed;
      }

      [Observation]
      public void should_not_crash()
      {
      }
   }

   public class When_the_simulation_runner_is_told_to_run_an_invalid_simulation_that_was_not_accepted_by_the_user : concern_for_SimulationRunner
   {
      private IndividualSimulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _entityTask.Validate(_simulation)).Returns(false);
      }

      protected override Task Because()
      {
         return sut.RunSimulation(_simulation, _simulationRunOptions);
      }

      [Observation]
      public void should_not_run_the_simulation()
      {
         A.CallTo(() => _simulationEngine.RunAsync(_simulation, _simulationRunOptions)).MustNotHaveHappened();
      }
   }
}