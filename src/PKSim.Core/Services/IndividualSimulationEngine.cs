using System;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Serialization.SimModel.Services;

namespace PKSim.Core.Services
{
   public class IndividualSimulationEngine : ISimulationEngine<IndividualSimulation>
   {
      private readonly ISimModelManager _simModelManager;
      private readonly IProgressManager _progressManager;
      private IProgressUpdater _progressUpdater;
      private readonly ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      private readonly IEventPublisher _eventPublisher;
      private readonly IExceptionManager _exceptionManager;
      private readonly ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;

      public IndividualSimulationEngine(ISimModelManager simModelManager, IProgressManager progressManager,
         ISimulationResultsSynchronizer simulationResultsSynchronizer,
         IEventPublisher eventPublisher, IExceptionManager exceptionManager,
         ISimulationToModelCoreSimulationMapper modelCoreSimulationMapper, ISimulationPersistableUpdater simulationPersistableUpdater)
      {
         _simModelManager = simModelManager;
         _progressManager = progressManager;
         _simulationResultsSynchronizer = simulationResultsSynchronizer;
         _eventPublisher = eventPublisher;
         _exceptionManager = exceptionManager;
         _modelCoreSimulationMapper = modelCoreSimulationMapper;
         _simulationPersistableUpdater = simulationPersistableUpdater;
         _simModelManager.Terminated += terminated;
      }

      private void terminated(object sender, EventArgs eventArgs)
      {
         terminated();
      }

      private void terminated()
      {
         _progressUpdater?.Dispose();
         _simModelManager.Terminated -= terminated;
         _simModelManager.SimulationProgress -= simulationProgress;
      }

      public async Task RunAsync(IndividualSimulation individualSimulation)
      {
         _progressUpdater = _progressManager.Create();
         _progressUpdater.Initialize(100, PKSimConstants.UI.Calculating);
         _simModelManager.SimulationProgress += simulationProgress;
         //make sure that thread methods always catch and handle any exception,
         //otherwise we risk unplanned application termination
         var begin = SystemTime.UtcNow();
         try
         {
            _eventPublisher.PublishEvent(new SimulationRunStartedEvent());
            await runSimulation(individualSimulation, exportAll: false, raiseEvents: true, checkForNegativeValues: true);
         }
         catch (Exception ex)
         {
            _exceptionManager.LogException(ex);
            terminated();
         }
         finally
         {
            var end = SystemTime.UtcNow();
            var timeSpent = end - begin;
            _eventPublisher.PublishEvent(new SimulationRunFinishedEvent(individualSimulation, timeSpent));
         }
      }

      public void Stop()
      {
         _simModelManager.StopSimulation();
      }

      public void Run(IndividualSimulation simulation)
      {
         runSimulation(simulation, exportAll: false, raiseEvents: false, checkForNegativeValues: true).Wait();
      }

      private Task runSimulation(IndividualSimulation simulation, bool exportAll, bool raiseEvents, bool checkForNegativeValues)
      {
         //Should be done outside of the Task.Run to ensure that any event that might be raised by this action won't cause threading issue
         updatePersistableFor(simulation, exportAll);

         return Task.Run(() =>
         {
            var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(simulation, shouldCloneModel:false);
            var simRunOptions = new SimulationRunOptions
            {
               SimModelExportMode = SimModelExportMode.Optimized,
               CheckForNegativeValues = checkForNegativeValues
            };
               
            var simResults = _simModelManager.RunSimulation(modelCoreSimulation, simRunOptions);

            if (!simResults.Success)
               return;

            _simulationResultsSynchronizer.Synchronize(simulation, simResults.Results);
            simulation.ClearPKCache();

            if (raiseEvents)
               _eventPublisher.PublishEvent(new SimulationResultsUpdatedEvent(simulation));
         });
      }

      public Task RunForBatch(IndividualSimulation individualSimulation, bool checkNegativeValues)
      {
         //we want to export all 
         return runSimulation(individualSimulation, exportAll: true, raiseEvents: false, checkForNegativeValues: checkNegativeValues);
      }

      private void updatePersistableFor(IndividualSimulation simulation, bool exportAll)
      {
         if (exportAll)
            _simulationPersistableUpdater.ResetPersistable(simulation);
         else
            _simulationPersistableUpdater.UpdatePersistableFromSettings(simulation);
      }

      private void simulationProgress(object sender, SimulationProgressEventArgs simulationProgressArgs)
      {
         _progressUpdater.ReportProgress(simulationProgressArgs.Progress, PKSimConstants.UI.Calculating);
      }
   }
}