using System.Linq;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;

namespace PKSim.Core.Services
{
   public interface ISimulationRunner : IListener<SimulationResultsUpdatedEvent>
   {
      void RunSimulation(Simulation simulation, bool defineSettings = false);
      void StopSimulation();
   }

   public class SimulationRunner : ISimulationRunner,
                                   IStrictVisitor,
                                   IVisitor<IndividualSimulation>,
                                   IVisitor<PopulationSimulation>
   {
      private readonly ISimulationEngineFactory _simulationEngineFactory;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEntityValidationTask _entityValidationTask;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ICloner _cloner;
      private ISimulationEngine _simulationEngine;
      private bool _defineSettings;

      public SimulationRunner(ISimulationEngineFactory simulationEngineFactory, ISimulationAnalysisCreator simulationAnalysisCreator, ILazyLoadTask lazyLoadTask,
                              IEntityValidationTask entityValidationTask,  ISimulationSettingsRetriever simulationSettingsRetriever, ICloner cloner)
      {
         _simulationEngineFactory = simulationEngineFactory;
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _lazyLoadTask = lazyLoadTask;
         _entityValidationTask = entityValidationTask;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _cloner = cloner;
      }

      public void RunSimulation(Simulation simulation, bool defineSettings = false)
      {
         _defineSettings = defineSettings;
         _lazyLoadTask.Load(simulation);
         if (!_entityValidationTask.Validate(simulation)) return;
         this.Visit(simulation);
      }

      public void StopSimulation()
      {
         if (_simulationEngine == null) return;
         _simulationEngine.Stop();
         _simulationEngine = null;
      }

      public void Visit(IndividualSimulation simulation)
      {
         runSimulation(simulation);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         runSimulation(populationSimulation);
      }

      private void runSimulation<TSimulation>(TSimulation simulation) where TSimulation : Simulation
      {
         if (settingsRequired(simulation))
         {
            var outputSelections = _simulationSettingsRetriever.SettingsFor(simulation);
            if (outputSelections == null) return;
            simulation.OutputSelections.UpdatePropertiesFrom(outputSelections, _cloner);
         }

         var simulationEngine = _simulationEngineFactory.Create<TSimulation>();
         _simulationEngine = simulationEngine;
         simulationEngine.RunAsync(simulation);
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
         if (simulation == null || !simulation.HasResults) return;
         if (simulation.Analyses.Count() != 0) return;
         _simulationAnalysisCreator.CreateAnalysisFor(simulation);
      }
   }
}