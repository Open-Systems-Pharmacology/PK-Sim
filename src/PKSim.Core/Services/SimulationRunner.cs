using System;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationRunner
   {
      Task RunSimulation(Simulation simulation, SimulationRunOptions simulationRunOptions = null, CancellationToken cancellationToken = default);
   }

   public class SimulationRunner : ISimulationRunner
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEntityValidationTask _entityValidationTask;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;
      private readonly Task _simulationDidNotRun = Task.FromResult(false);

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

      public Task RunSimulation(
         Simulation simulation,
         SimulationRunOptions simulationRunOptions = null,
         CancellationToken cancellationToken = default)
      {
         var options = simulationRunOptions ?? new SimulationRunOptions();
         _lazyLoadTask.Load(simulation);

         if (options.Validate && !_entityValidationTask.Validate(simulation))
            return _simulationDidNotRun;

         switch (simulation)
         {
            case IndividualSimulation individualSimulation:
               return runSimulation<IndividualSimulation, SimulationRunResults>(individualSimulation, options, cancellationToken);

            case PopulationSimulation populationSimulation:
               return runSimulation<PopulationSimulation, PopulationRunResults>(populationSimulation, options, cancellationToken);

            default:
               return _simulationDidNotRun;
         }
      }

      private async Task runSimulation<TSimulation, TResult>(
         TSimulation simulation,
         SimulationRunOptions options,
         CancellationToken cancellationToken)
         where TSimulation : Simulation
      {
         var simulationEngine = _simulationEngineFactory.Create<TSimulation, TResult>();

         if (options.RunForAllOutputs)
            _simulationPersistableUpdater.ResetPersistable(simulation);
         else
            _simulationPersistableUpdater.UpdatePersistableFromSettings(simulation);

         updateSolverSettings(simulation, options);
         await simulationEngine.RunAsync(simulation, options, cancellationToken);
         simulation.HasChanged = true;
      }

      private static void updateSolverSettings<TSimulation>(TSimulation simulation, SimulationRunOptions options)
         where TSimulation : Simulation
      {
         switch (options.JacobianUse)
         {
            case JacobianUse.AsIs:
               return;
            case JacobianUse.TurnOff:
               simulation.Solver.UseJacobian = false;
               return;
            case JacobianUse.TurnOn:
               simulation.Solver.UseJacobian = true;
               return;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }
}