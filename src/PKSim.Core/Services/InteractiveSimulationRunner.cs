using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IInteractiveSimulationRunner
   {
      Task RunSimulation(Simulation simulation, bool selectOutput);
      void StopSimulation(Simulation simulation);
      void StopAllSimulations();
      bool IsSimulationRunning(Simulation simulation);
      bool IsSimulationIdle(Simulation simulation);
   }

   public class InteractiveSimulationRunner : IInteractiveSimulationRunner
   {
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ISimulationRunner _simulationRunner;
      private readonly ICloner _cloner;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IExecutionContext _executionContext;
      private readonly ConcurrentDictionary<Simulation, CancellationTokenSource> _cancellationTokenSources = new ConcurrentDictionary<Simulation, CancellationTokenSource>();
      private readonly IEventPublisher _eventPublisher;
      private readonly SimulationRunOptions _simulationRunOptions;
      protected bool _shouldRaiseEvents;

      public InteractiveSimulationRunner(
         ISimulationSettingsRetriever simulationSettingsRetriever,
         ISimulationRunner simulationRunner,
         ICloner cloner,
         ISimulationAnalysisCreator simulationAnalysisCreator,
         ILazyLoadTask lazyLoadTask,
         IExecutionContext executionContext,
         IEventPublisher eventPublisher)
      {
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _simulationRunner = simulationRunner;
         _cloner = cloner;
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _lazyLoadTask = lazyLoadTask;
         _executionContext = executionContext;
         _eventPublisher = eventPublisher;

         _simulationRunOptions = new SimulationRunOptions
         {
            CheckForNegativeValues = true,
            RaiseEvents = true, //This is always true, but still worth checking before rising an event since it can change.
            RunForAllOutputs = false,
            SimModelExportMode = SimModelExportMode.Optimized
         };
      }

      public bool IsSimulationRunning(Simulation simulation) =>
         _cancellationTokenSources.TryGetValue(simulation, out var cts) && !cts.IsCancellationRequested;

      public Task RunSimulation(Simulation simulation, bool selectOutput) =>
         runSimulationAsync(simulation, selectOutput);

      private async Task runSimulationAsync(Simulation simulation, bool selectOutput)
      {
         _shouldRaiseEvents = _simulationRunOptions.RaiseEvents;
         var cts = new CancellationTokenSource();
         var begin = new DateTime();
         if (!_cancellationTokenSources.TryAdd(simulation, cts)) //this will prevent from running one that is already running
            return;
         try
         {
            _lazyLoadTask.Load(simulation);

            if (outputSelectionRequired(simulation, selectOutput))
            {
               var outputSelections = _simulationSettingsRetriever.SettingsFor(simulation);
               if (outputSelections == null)
                  return;

               simulation.OutputSelections.UpdatePropertiesFrom(outputSelections, _cloner);
               mappingsNotSelected(simulation).Each(simulation.OutputMappings.Remove);

               _executionContext.PublishEvent(new SimulationOutputSelectionsChangedEvent(simulation));
            }
            begin = SystemTime.UtcNow();
            _eventPublisher.PublishEvent(new SimulationRunStartedEvent(simulation));
            await _simulationRunner.RunSimulation(simulation, _simulationRunOptions, cts.Token);

            addAnalysableToSimulationIfRequired(simulation);
         }
         finally
         {
            if (_cancellationTokenSources.ContainsKey(simulation))
            {
               var end = SystemTime.UtcNow();
               var timeSpent = end - begin;
               if (_cancellationTokenSources.TryRemove(simulation, out var ctsToDispose))
               {
                  ctsToDispose.Dispose();
               }
               _eventPublisher.PublishEvent(new SimulationRunFinishedEvent(simulation, timeSpent));
            }
         }
      }

      private static IReadOnlyList<OutputMapping> mappingsNotSelected(Simulation simulation) =>
         simulation.OutputMappings.Where(outputMapping => !outputMappingIsSelected(simulation, outputMapping)).ToList();

      private static bool outputMappingIsSelected(Simulation simulation, OutputMapping outputMapping) =>
         simulation.OutputSelections.AllOutputs.Any(quantitySelection => Equals(quantitySelection.Path, outputMapping.OutputPath));

      private bool outputSelectionRequired(Simulation simulation, bool selectOutput)
      {
         if (selectOutput)
            return true;

         if (simulation.OutputSelections == null)
            return true;

         return !simulation.OutputSelections.HasSelection;
      }

      private void addAnalysableToSimulationIfRequired(Simulation simulation)
      {
         if (simulation == null || !simulation.HasResults) return;
         if (simulation.Analyses.Count() != 0) return;
         _simulationAnalysisCreator.CreateAnalysisFor(simulation);
      }
      private void raiseEvent<T>(T eventToPublish)
      {
         if (_shouldRaiseEvents)
            _eventPublisher.PublishEvent(eventToPublish);
      }

      public void StopSimulation(Simulation simulation)
      {
         tryCancelAndDispose(simulation);
      }

      public void StopAllSimulations()
      {
         foreach (var simulation in _cancellationTokenSources.Keys.ToList())
         {
            tryCancelAndDispose(simulation);
         }
      }

      private void tryCancelAndDispose(Simulation simulation)
      {
         if (_cancellationTokenSources.TryRemove(simulation, out var cts))
         {
            cts.Cancel();
            cts.Dispose();
            _executionContext.PublishEvent(new SimulationRunCanceledEvent(simulation));
         }
      }

      public bool IsSimulationIdle(Simulation simulation)
      {
         return !_cancellationTokenSources.TryGetValue(simulation, out var cts)
                || cts.IsCancellationRequested;
      }
   }
}