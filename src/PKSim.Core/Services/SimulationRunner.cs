using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationRunner : IListener<SimulationResultsUpdatedEvent>
   {
      Task RunSimulation(Simulation simulation, bool defineSettings = false, bool createDefaultAnalysis = true);
      void StopSimulation();
   }

   public class SimulationRunner : ISimulationRunner
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEntityValidationTask _entityValidationTask;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ICloner _cloner;
      private ISimulationEngine _simulationEngine;
      private bool _defineSettings;
      private readonly Task _simulationDidNotRun = Task.FromResult(false);
      private bool _createDefaultAnalysis;

      public SimulationRunner(ISimulationEngineFactory simulationEngineFactory, ISimulationAnalysisCreator simulationAnalysisCreator, ILazyLoadTask lazyLoadTask,
         IEntityValidationTask entityValidationTask, ISimulationSettingsRetriever simulationSettingsRetriever, ICloner cloner)
      {
         _simulationEngineFactory = simulationEngineFactory;
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _lazyLoadTask = lazyLoadTask;
         _entityValidationTask = entityValidationTask;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _cloner = cloner;
      }

      public Task RunSimulation(Simulation simulation, bool defineSettings = false, bool createDefaultAnalysis = true)
      {
         _defineSettings = defineSettings;
         _createDefaultAnalysis = createDefaultAnalysis;
         _lazyLoadTask.Load(simulation);

         if (!_entityValidationTask.Validate(simulation))
            return _simulationDidNotRun;

         switch (simulation)
         {
            case IndividualSimulation individualSimulation:
               return runSimulation(individualSimulation);

            case PopulationSimulation populationSimulation:
               return runSimulation(populationSimulation);
         }
         return _simulationDidNotRun;
      }

      public void StopSimulation()
      {
         if (_simulationEngine == null) return;
         _simulationEngine.Stop();
         _simulationEngine = null;
      }

      private async Task runSimulation<TSimulation>(TSimulation simulation) where TSimulation : Simulation
      {
         if (settingsRequired(simulation))
         {
            var outputSelections = _simulationSettingsRetriever.SettingsFor(simulation);
            if (outputSelections == null) return;
            simulation.OutputSelections.UpdatePropertiesFrom(outputSelections, _cloner);
         }

         var simulationEngine = _simulationEngineFactory.Create<TSimulation>();
         _simulationEngine = simulationEngine;
         await simulationEngine.RunAsync(simulation);
         simulation.HasChanged = true;
      }

      private bool settingsRequired(Simulation simulation)
      {
         if (_defineSettings)
            return true;

         if (simulation.OutputSelections == null)
            return true;

         return !simulation.OutputSelections.HasSelection;
      }

      public void Handle(SimulationResultsUpdatedEvent eventToHandle)
      {
         var simulation = eventToHandle.Simulation as Simulation;
         if (simulation == null || !simulation.HasResults || !_createDefaultAnalysis) return;
         if (simulation.Analyses.Count() != 0) return;
         _simulationAnalysisCreator.CreateAnalysisFor(simulation);
      }
   }
}