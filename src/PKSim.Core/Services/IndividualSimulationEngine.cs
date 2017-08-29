using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class IndividualSimulationEngine : ISimulationEngine<IndividualSimulation>
   {
      private readonly ISimModelManager _simModelManager;
      private readonly IProgressManager _progressManager;
      private IProgressUpdater _progressUpdater;
      private readonly ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      private readonly IEventPublisher _eventPublisher;
      private readonly ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private bool _shouldRaiseEvents;

      public IndividualSimulationEngine(ISimModelManager simModelManager, IProgressManager progressManager,
         ISimulationResultsSynchronizer simulationResultsSynchronizer,
         IEventPublisher eventPublisher, ISimulationToModelCoreSimulationMapper modelCoreSimulationMapper)
      {
         _simModelManager = simModelManager;
         _progressManager = progressManager;
         _simulationResultsSynchronizer = simulationResultsSynchronizer;
         _eventPublisher = eventPublisher;
         _modelCoreSimulationMapper = modelCoreSimulationMapper;
         _simModelManager.Terminated += terminated;
      }

      private void terminated(object sender, EventArgs eventArgs)
      {
         terminated();
      }

      private void terminated()
      {
         _progressUpdater?.Dispose();
         _progressUpdater = null;
         _simModelManager.Terminated -= terminated;
         _simModelManager.SimulationProgress -= simulationProgress;
      }

      public async Task RunAsync(IndividualSimulation individualSimulation, SimulationRunOptions simulationRunOptions)
      {
         _shouldRaiseEvents = simulationRunOptions.RaiseEvents;
         initializeProgress();

         _simModelManager.SimulationProgress += simulationProgress;
         //make sure that thread methods always catch and handle any exception,
         //otherwise we risk unplanned application termination
         var begin = SystemTime.UtcNow();
         try
         {
            raiseEvent(new SimulationRunStartedEvent());
            await runSimulation(individualSimulation, simulationRunOptions);
         }
         catch (Exception)
         {
            terminated();
            throw;
         }
         finally
         {
            var end = SystemTime.UtcNow();
            var timeSpent = end - begin;
            raiseEvent(new SimulationRunFinishedEvent(individualSimulation, timeSpent));
         }
      }

      private void raiseEvent<T>(T eventToRaise)
      {
         if (_shouldRaiseEvents)
            _eventPublisher.PublishEvent(eventToRaise);
      }

      private void initializeProgress()
      {
         if (!_shouldRaiseEvents)
            return;

         _progressUpdater = _progressManager.Create();
         _progressUpdater.Initialize(100, PKSimConstants.UI.Calculating);
      }

      public void Stop()
      {
         _simModelManager.StopSimulation();
      }

      private Task runSimulation(IndividualSimulation simulation, SimulationRunOptions simulationRunOptions)
      {
         return Task.Run(() =>
         {
            var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(simulation, shouldCloneModel: false);
            var simResults = _simModelManager.RunSimulation(modelCoreSimulation, simulationRunOptions);

            if (!simResults.Success)
               return;

            _simulationResultsSynchronizer.Synchronize(simulation, simResults.Results);
            updateResultsName(simulation);

            simulation.ClearPKCache();

            raiseEvent(new SimulationResultsUpdatedEvent(simulation));
         });
      }

      private void updateResultsName(IndividualSimulation simulation)
      {
         if (simulation.DataRepository == null)
            return;

         simulation.DataRepository.Name = simulation.Name;
      }

      private void simulationProgress(object sender, SimulationProgressEventArgs simulationProgressArgs)
      {
         _progressUpdater?.ReportProgress(simulationProgressArgs.Progress, PKSimConstants.UI.Calculating);
      }
   }
}