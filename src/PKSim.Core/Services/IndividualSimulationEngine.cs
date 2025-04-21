using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Journal;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class IndividualSimulationEngine : SimulationEngine, IIndividualSimulationEngine
   {
      private readonly ISimModelManager _simModelManager;
      private readonly IProgressManager _progressManager;
      private IProgressUpdater _progressUpdater;
      private readonly ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      private readonly ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      


      public IndividualSimulationEngine(ISimModelManager simModelManager, IProgressManager progressManager,
         ISimulationResultsSynchronizer simulationResultsSynchronizer,
         IEventPublisher eventPublisher, ISimulationToModelCoreSimulationMapper modelCoreSimulationMapper) : base(eventPublisher)
      {
         _simModelManager = simModelManager;
         _progressManager = progressManager;
         _simulationResultsSynchronizer = simulationResultsSynchronizer;
         _modelCoreSimulationMapper = modelCoreSimulationMapper;
         _simModelManager.Terminated += terminated;
      }

      private void terminated(object sender, EventArgs eventArgs)
      {
         RaiseEvent(new ProgressDoneWithMessageEvent(PKSimConstants.UI.SimulationRun));
         terminated();
      }

      private void terminated()
      {
         _progressUpdater?.Dispose();
         _progressUpdater = null;
         _simModelManager.Terminated -= terminated;
         _simModelManager.SimulationProgress -= simulationProgress;
      }

      public async Task RunAsync(IndividualSimulation individualSimulation, SimulationRunOptions simulationRunOptions, CancellationToken cancellationToken = default)
      {
         _shouldRaiseEvents = simulationRunOptions.RaiseEvents;
         initializeProgress();
         _simModelManager.SimulationProgress += simulationProgress;
         //make sure that thread methods always catch and handle any exception,
         //otherwise we risk unplanned application termination
         var begin = SystemTime.UtcNow();
         try
         {
            RaiseEvent(new SimulationRunStartedEvent(individualSimulation));
            await runSimulation(individualSimulation, simulationRunOptions, cancellationToken);
         }
         catch (Exception ex)
         {
            terminated();
            if(!(ex is TaskCanceledException)) //do not throw if this has been canceled
               throw;
         }
         finally
         {
            var end = SystemTime.UtcNow();
            var timeSpent = end - begin;
            RaiseEvent(new SimulationRunFinishedEvent(individualSimulation, timeSpent));
         }
      }

      private void initializeProgress()
      {
         if (!_shouldRaiseEvents)
            return;

         _progressUpdater = _progressManager.Create();
         _progressUpdater.Initialize(100, PKSimConstants.UI.Calculating);
      }
      
      private async Task  runSimulation(IndividualSimulation simulation, SimulationRunOptions simulationRunOptions, CancellationToken cancellationToken = default)
      {
         var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(simulation, shouldCloneModel: false);
         var simResults = await _simModelManager.RunSimulationAsync(modelCoreSimulation, cancellationToken, simulationRunOptions);

         if (!simResults.Success)
            return;

         _simulationResultsSynchronizer.Synchronize(simulation, simResults.Results);
         updateResultsName(simulation);

         simulation.ClearPKCache();

         RaiseEvent(new SimulationResultsUpdatedEvent(simulation));
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

      public void StopSimulation(Simulation simulation)
      {
      
      }
   }
}