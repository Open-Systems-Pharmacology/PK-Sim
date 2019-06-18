using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Serialization.SimModel.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IInteractiveSimulationRunner
   {
      Task RunSimulation(Simulation simulation, bool selectOutput);
      void StopSimulation();
   }

   public class InteractiveSimulationRunner : IInteractiveSimulationRunner
   {
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ISimulationRunner _simulationRunner;
      private readonly ICloner _cloner;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly ILazyLoadTask _lazyLoadTask;

      private readonly SimulationRunOptions _simulationRunOptions;

      public InteractiveSimulationRunner(
         ISimulationSettingsRetriever simulationSettingsRetriever, 
         ISimulationRunner simulationRunner, 
         ICloner cloner, 
         ISimulationAnalysisCreator simulationAnalysisCreator, 
         ILazyLoadTask lazyLoadTask)
      {
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _simulationRunner = simulationRunner;
         _cloner = cloner;
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _lazyLoadTask = lazyLoadTask;

         _simulationRunOptions = new SimulationRunOptions
         {
            CheckForNegativeValues = true,
            RaiseEvents = true,
            RunForAllOutputs = false,
            SimModelExportMode = SimModelExportMode.Optimized
         };
      }

      public async Task RunSimulation(Simulation simulation, bool selectOutput)
      {
         _lazyLoadTask.Load(simulation);

         if (outputSelectionRequired(simulation, selectOutput))
         {
            var outputSelections = _simulationSettingsRetriever.SettingsFor(simulation);
            if (outputSelections == null)
               return;

            simulation.OutputSelections.UpdatePropertiesFrom(outputSelections, _cloner);
         }

         await _simulationRunner.RunSimulation(simulation, _simulationRunOptions);

         addAnalysableToSimulationIfRequired(simulation);
      }

      private bool outputSelectionRequired(Simulation simulation, bool selectOutput)
      {
         if (selectOutput)
            return true;

         if (simulation.OutputSelections == null)
            return true;

         return !simulation.OutputSelections.HasSelection;
      }

      public void StopSimulation()
      {
         _simulationRunner.StopSimulation();
      }

      private void addAnalysableToSimulationIfRequired(Simulation simulation)
      {
         if (simulation == null || !simulation.HasResults) return;
         if (simulation.Analyses.Count() != 0) return;
         _simulationAnalysisCreator.CreateAnalysisFor(simulation);
      }
   }
}