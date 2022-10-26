using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
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
   public class BaseSimulationEngine
   {
      protected bool _shouldRaiseEvents;
      private readonly IEventPublisher _eventPublisher;

      protected BaseSimulationEngine(IEventPublisher eventPublisher)
      {
         _eventPublisher = eventPublisher;
      }

      protected void RaiseEvent<T>(T eventToPublish)
      {
         if(_shouldRaiseEvents)
            _eventPublisher.PublishEvent(eventToPublish);
      }
   }

   public class PopulationSimulationEngine : BaseSimulationEngine, IPopulationSimulationEngine
   {
      private readonly IPopulationRunner _populationRunner;
      private readonly IProgressManager _progressManager;
      private IProgressUpdater _progressUpdater;
      private readonly ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      private readonly IPopulationExportTask _populationExporter;
      private readonly ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private readonly ICoreUserSettings _userSettings;
      private readonly IPopulationSimulationAnalysisSynchronizer _populationSimulationAnalysisSynchronizer;

      public PopulationSimulationEngine(
         IPopulationRunner populationRunner,
         IProgressManager progressManager,
         IEventPublisher eventPublisher,
         ISimulationResultsSynchronizer simulationResultsSynchronizer,
         IPopulationExportTask populationExporter,
         ISimulationToModelCoreSimulationMapper modelCoreSimulationMapper,
         ICoreUserSettings userSettings,
         IPopulationSimulationAnalysisSynchronizer populationSimulationAnalysisSynchronizer) : base(eventPublisher)
      {
         _populationRunner = populationRunner;
         _progressManager = progressManager;
         _simulationResultsSynchronizer = simulationResultsSynchronizer;
         _populationExporter = populationExporter;
         _modelCoreSimulationMapper = modelCoreSimulationMapper;
         _userSettings = userSettings;
         _populationSimulationAnalysisSynchronizer = populationSimulationAnalysisSynchronizer;
         _populationRunner.Terminated += terminated;
         _populationRunner.SimulationProgress += simulationProgress;
      }

      public async Task<PopulationRunResults> RunAsync(PopulationSimulation populationSimulation, SimulationRunOptions simulationRunOptions)
      {
         _progressUpdater = _progressManager.Create();
         _progressUpdater.Initialize(populationSimulation.NumberOfItems, PKSimConstants.UI.Calculating);

         _shouldRaiseEvents = simulationRunOptions.RaiseEvents;

         var begin = SystemTime.UtcNow();
         try
         {
            var populationData = _populationExporter.CreatePopulationDataFor(populationSimulation);
            var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(populationSimulation, shouldCloneModel: false);
            var runOptions = new RunOptions {NumberOfCoresToUse = _userSettings.MaximumNumberOfCoresToUse};
            var simulationRunStartedEvent = new SimulationRunStartedEvent();
            RaiseEvent(simulationRunStartedEvent);
            var populationRunResults = await _populationRunner.RunPopulationAsync(modelCoreSimulation, runOptions, populationData, populationSimulation.AgingData.ToDataTable());
            _simulationResultsSynchronizer.Synchronize(populationSimulation, populationRunResults.Results);
            _populationSimulationAnalysisSynchronizer.UpdateAnalysesDefinedIn(populationSimulation);
            RaiseEvent(new SimulationResultsUpdatedEvent(populationSimulation));

            return populationRunResults;
         }
         catch (OperationCanceledException)
         {
            simulationTerminated();
            return null;
         }
         catch (Exception)
         {
            simulationTerminated();
            throw;
         }
         finally
         {
            var end = SystemTime.UtcNow();
            var timeSpent = end - begin;
            RaiseEvent(new SimulationRunFinishedEvent(populationSimulation, timeSpent));
         }
      }

      private void simulationTerminated()
      {
         terminated(this, new EventArgs());
      }

      private void terminated(object sender, EventArgs e)
      {
         _progressUpdater?.Dispose();
         _populationRunner.Terminated -= terminated;
         _populationRunner.SimulationProgress -= simulationProgress;
      }

      public void Stop()
      {
         _populationRunner.StopSimulation();
      }

      private void simulationProgress(object sender, MultipleSimulationsProgressEventArgs e)
      {
         _progressUpdater.ReportProgress(e.NumberOfCalculatedSimulation, PKSimConstants.UI.CalculationPopulationSimulation(e.NumberOfCalculatedSimulation, e.NumberOfSimulations));
      }
   }
}