using System;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class IndividualSimulationEngine : SimulationEngine, IIndividualSimulationEngine
   {
      private readonly IProgressManager _progressManager;
      private IProgressUpdater _progressUpdater;
      private readonly ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      private readonly ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private readonly ISimModelManagerFactory _simModelManagerFactory;

      public IndividualSimulationEngine(
         IProgressManager progressManager,
         ISimulationResultsSynchronizer simulationResultsSynchronizer,
         IEventPublisher eventPublisher,
         ISimulationToModelCoreSimulationMapper modelCoreSimulationMapper,
         ISimModelManagerFactory simModelManagerFactory) : base(eventPublisher)
      {
         _progressManager = progressManager;
         _simulationResultsSynchronizer = simulationResultsSynchronizer;
         _modelCoreSimulationMapper = modelCoreSimulationMapper;
         _simModelManagerFactory = simModelManagerFactory;
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
      }

      public async Task RunAsync(IndividualSimulation individualSimulation, SimulationRunOptions simulationRunOptions, CancellationToken cancellationToken = default)
      {
         _shouldRaiseEvents = simulationRunOptions.RaiseEvents;
         initializeProgress();
         //make sure that thread methods always catch and handle any exception,
         //otherwise we risk unplanned application termination
         try
         {
            await runSimulation(individualSimulation, simulationRunOptions, cancellationToken);
         }
         catch (Exception ex)
         {
            if (!(ex is TaskCanceledException)) //do not throw if this has been canceled
               throw;
         }
         finally
         {
            terminated();
         }
      }

      private void addEvents(ISimModelManager simModelManager)
      {
         if (simModelManager == null) return;
         simModelManager.SimulationProgress += simulationProgress;
         simModelManager.Terminated += terminated;
      }

      private void removeEvents(ISimModelManager simModelManager)
      {
         if (simModelManager == null) return;
         simModelManager.SimulationProgress -= simulationProgress;
         simModelManager.Terminated -= terminated;
      }

      private void initializeProgress()
      {
         if (!_shouldRaiseEvents)
            return;

         _progressUpdater = _progressManager.Create();
         _progressUpdater.Initialize(100, PKSimConstants.UI.Calculating);
      }

      private async Task runSimulation(IndividualSimulation simulation, SimulationRunOptions simulationRunOptions, CancellationToken cancellationToken = default)
      {
         var simModelManager = _simModelManagerFactory.Create();
         addEvents(simModelManager);

         var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(simulation, shouldCloneModel: false);
         var simResults = await simModelManager.RunSimulationAsync(modelCoreSimulation, cancellationToken, simulationRunOptions);

         removeEvents(simModelManager);

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
   }
}