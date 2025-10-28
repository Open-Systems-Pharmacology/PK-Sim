using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public abstract class SimulationEngine
   {
      protected bool _shouldRaiseEvents;
      private readonly IEventPublisher _eventPublisher;

      protected SimulationEngine(IEventPublisher eventPublisher)
      {
         _eventPublisher = eventPublisher;
      }

      protected void RaiseEvent<T>(T eventToPublish)
      {
         if(_shouldRaiseEvents)
            _eventPublisher.PublishEvent(eventToPublish);
      }
   }

   public class PopulationSimulationEngine : SimulationEngine, IPopulationSimulationEngine
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

      public async Task RunAsync(PopulationSimulation populationSimulation, SimulationRunOptions simulationRunOptions, CancellationToken cancellationToken = default)
      {
         _progressUpdater = _progressManager.Create();
         _progressUpdater.Initialize(populationSimulation.NumberOfItems, PKSimConstants.UI.Calculating);

         _shouldRaiseEvents = simulationRunOptions.RaiseEvents;

         try
         {
            var populationData = _populationExporter.CreatePopulationDataFor(populationSimulation);
            var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(populationSimulation, shouldCloneModel: false);
            var runOptions = new RunOptions {NumberOfCoresToUse = _userSettings.MaximumNumberOfCoresToUse};
            var populationRunResults = await _populationRunner.RunPopulationAsync(modelCoreSimulation, runOptions, populationData, populationSimulation.AgingData.ToDataTable(), cancellationToken: cancellationToken);
            _simulationResultsSynchronizer.Synchronize(populationSimulation, populationRunResults.Results);
            _populationSimulationAnalysisSynchronizer.UpdateAnalysesDefinedIn(populationSimulation);
            RaiseEvent(new SimulationResultsUpdatedEvent(populationSimulation));
         }
         catch (OperationCanceledException)
         {
            simulationTerminated();
            return;
         }
         catch (Exception)
         {
            simulationTerminated();
            throw;
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

      private void simulationProgress(object sender, MultipleSimulationsProgressEventArgs e)
      {
         _progressUpdater.ReportProgress(e.NumberOfCalculatedSimulation, PKSimConstants.UI.CalculationPopulationSimulation(e.NumberOfCalculatedSimulation, e.NumberOfSimulations));
      }
   }
}