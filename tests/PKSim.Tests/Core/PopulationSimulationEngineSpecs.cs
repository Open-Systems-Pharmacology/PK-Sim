using System.Data;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;

using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationSimulationEngine : ContextSpecificationAsync<IPopulationSimulationEngine>
   {
      protected IEventPublisher _eventPublisher;
      protected IProgressUpdater _progressUpdater;
      private ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      protected IPopulationExportTask _popExportTask;
      protected ISimulationToModelCoreSimulationMapper _simMapper;
      protected IPopulationRunner _populationRunner;
      private IProgressManager _progressManager;
      private ICoreUserSettings _userSettings;
      protected IPopulationSimulationAnalysisSynchronizer _populationSimulationAnalysisSynchronizer;
      protected PopulationSimulation _populationSimulation;
      private DataTable _populationData;
      private IModelCoreSimulation _modelSimulation;
      private PopulationRunResults _runResults;
      protected SimulationRunOptions _simulationRunOptions;

      protected override Task Context()
      {
         _populationRunner = A.Fake<IPopulationRunner>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _progressUpdater = A.Fake<IProgressUpdater>();
         _progressManager = A.Fake<IProgressManager>();
         _simulationResultsSynchronizer = A.Fake<ISimulationResultsSynchronizer>();
         _simMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _popExportTask = A.Fake<IPopulationExportTask>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _populationSimulationAnalysisSynchronizer = A.Fake<IPopulationSimulationAnalysisSynchronizer>();

         sut = new PopulationSimulationEngine(_populationRunner,
            _progressManager,
            _eventPublisher,
            _simulationResultsSynchronizer,
            _popExportTask,
            _simMapper,
            _userSettings,
            _populationSimulationAnalysisSynchronizer);

         A.CallTo(() => _progressManager.Create()).Returns(_progressUpdater);

         _populationSimulation = A.Fake<PopulationSimulation>();
         _modelSimulation = A.Fake<IModelCoreSimulation>();
         _populationData = A.Fake<DataTable>();
         _runResults = new PopulationRunResults();
         
         A.CallTo(() => _popExportTask.CreatePopulationDataFor(_populationSimulation, A<bool>._)).Returns(_populationData);
         A.CallTo(() => _simMapper.MapFrom(_populationSimulation, false)).Returns(_modelSimulation);
         A.CallTo(() => _populationRunner.RunPopulationAsync(_modelSimulation, A<RunOptions>._, _populationData, A<DataTable>._, A<DataTable>._)).Returns(_runResults);

         return _completed;
      }
   }

   public class When_the_population_simulation_engine_is_starting_a_simulation_run_for_a_given_population_simulation : concern_for_PopulationSimulationEngine
   {

      protected override async Task Context()
      {
         await base.Context();
         _simulationRunOptions = new SimulationRunOptions { RaiseEvents = true };
      }

      protected override Task Because()
      {
         return sut.RunAsync(_populationSimulation, _simulationRunOptions);
      }

      [Observation]
      public void should_retrieve_the_data_for_the_pop_simulation_for_the_variable()
      {
         A.CallTo(() => _popExportTask.CreatePopulationDataFor(_populationSimulation, A<bool>._)).MustHaveHappened();
      }

      [Observation]
      public void should_synchronize_the_analysis_defined_in_the_population()
      {
         A.CallTo(() => _populationSimulationAnalysisSynchronizer.UpdateAnalysesDefinedIn(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_raise_events_for_simulation_start_and_end()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunStartedEvent>._)).MustHaveHappened();
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunFinishedEvent>._)).MustHaveHappened();
      }
   }

   public class When_the_simulation_run_options_indicate_the_events_should_not_be_raised : concern_for_PopulationSimulationEngine
   {
      protected override async Task Context()
      {
         await base.Context();
         _simulationRunOptions = new SimulationRunOptions { RaiseEvents = false };
      }

      protected override Task Because()
      {
         return sut.RunAsync(_populationSimulation, _simulationRunOptions);
      }

      [Observation]
      public void should_raise_events_for_simulation_start_and_end()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunStartedEvent>._)).MustNotHaveHappened();
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunFinishedEvent>._)).MustNotHaveHappened();
      }
   }
}