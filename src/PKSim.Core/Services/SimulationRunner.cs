using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationRunner
   {
      Task RunSimulation(Simulation simulation, SimulationRunOptions simulationRunOptions = null);
      void StopSimulation();
   }

   public class SimulationRunner : ISimulationRunner
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEntityValidationTask _entityValidationTask;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;
      private ISimulationEngine _simulationEngine;
      private readonly Task _simulationDidNotRun = Task.FromResult(false);
      private SimulationRunOptions _simulationRunOptions;

      public SimulationRunner(
         ISimulationEngineFactory simulationEngineFactory,
         ILazyLoadTask lazyLoadTask,
         IEntityValidationTask entityValidationTask,
         ISimulationPersistableUpdater simulationPersistableUpdater)
      {
         _simulationEngineFactory = simulationEngineFactory;
         _lazyLoadTask = lazyLoadTask;
         _entityValidationTask = entityValidationTask;
         _simulationPersistableUpdater = simulationPersistableUpdater;
      }

      public Task RunSimulation(Simulation simulation, SimulationRunOptions simulationRunOptions = null)
      {
         _simulationRunOptions = simulationRunOptions ?? new SimulationRunOptions();
         _lazyLoadTask.Load(simulation);

         if (_simulationRunOptions.Validate && !_entityValidationTask.Validate(simulation))
            return _simulationDidNotRun;

         switch (simulation)
         {
            case IndividualSimulation individualSimulation:
               return runSimulation(individualSimulation, _simulationPersistableUpdater.UpdatePersistableFromSettings);

            case PopulationSimulation populationSimulation:
               return runSimulation(populationSimulation, _simulationPersistableUpdater.UpdatePersistableFromSettings);
         }
         return _simulationDidNotRun;
      }

      public void StopSimulation()
      {
         if (_simulationEngine == null) return;
         _simulationEngine.Stop();
         _simulationEngine = null;
      }

      private async Task runSimulation<TSimulation>(TSimulation simulation, Action<TSimulation> updatePersistableFromSettings) where TSimulation : Simulation
      {
         var simulationEngine = _simulationEngineFactory.Create<TSimulation>();
         _simulationEngine = simulationEngine;

         if (_simulationRunOptions.RunForAllOutputs)
            _simulationPersistableUpdater.ResetPersistable(simulation);
         else
            updatePersistableFromSettings(simulation);

         await simulationEngine.RunAsync(simulation, _simulationRunOptions);
         simulation.HasChanged = true;
      }
   }
}