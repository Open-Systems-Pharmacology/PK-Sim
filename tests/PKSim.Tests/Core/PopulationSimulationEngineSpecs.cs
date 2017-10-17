using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationSimulationEngine : ContextSpecification<ISimulationEngine<PopulationSimulation>>
   {
      private IEventPublisher _eventPubliser;
      protected IProgressUpdater _progressUpdater;
      private ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      protected IPopulationExportTask _popExportTask;
      protected ISimulationToModelCoreSimulationMapper _simMapper;
      protected IPopulationRunner _populationRunner;
      private IProgressManager _progressManager;
      private ISimulationPersistableUpdater _simulationPersistableUpdater;
      private ICoreUserSettings _userSettings;
      protected IPopulationSimulationAnalysisSynchronizer _populationSimulationAnalysisSynchronizer;

      protected override void Context()
      {
         _populationRunner = A.Fake<IPopulationRunner>();
         _eventPubliser = A.Fake<IEventPublisher>();
         _progressUpdater = A.Fake<IProgressUpdater>();
         _progressManager = A.Fake<IProgressManager>();
         _simulationResultsSynchronizer = A.Fake<ISimulationResultsSynchronizer>();
         _simMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _popExportTask = A.Fake<IPopulationExportTask>();
         _simulationPersistableUpdater = A.Fake<ISimulationPersistableUpdater>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _populationSimulationAnalysisSynchronizer = A.Fake<IPopulationSimulationAnalysisSynchronizer>();

         sut = new PopulationSimulationEngine(_populationRunner,
            _progressManager,
            _eventPubliser,
            _simulationResultsSynchronizer,
            _popExportTask,
            _simMapper,
            _simulationPersistableUpdater,
            _userSettings,
            _populationSimulationAnalysisSynchronizer);

         A.CallTo(() => _progressManager.Create()).Returns(_progressUpdater);
      }
   }

   public class When_the_population_simulation_engine_is_starting_a_simulation_run_for_a_given_population_simulation : concern_for_PopulationSimulationEngine
   {
      private PopulationSimulation _populationSimulation;
      private DataTable _populationData;
      private IModelCoreSimulation _modelSimulation;
      private PopulationRunResults _runResults;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _modelSimulation = A.Fake<IModelCoreSimulation>();
         _populationData = A.Fake<DataTable>();
         _runResults = new PopulationRunResults();
         A.CallTo(() => _popExportTask.CreatePopulationDataFor(_populationSimulation, A<bool>._)).Returns(_populationData);
         A.CallTo(() => _simMapper.MapFrom(_populationSimulation, false)).Returns(_modelSimulation);
         A.CallTo(() => _populationRunner.RunPopulationAsync(_modelSimulation, _populationData, A<DataTable>._, A<DataTable>._)).Returns(_runResults);
      }

      protected override void Because()
      {
         sut.Run(_populationSimulation);
      }

      [Observation]
      public void should_retrieve_the_data_for_the_pop_simulation_for_the_variable()
      {
         A.CallTo(() => _popExportTask.CreatePopulationDataFor(_populationSimulation, A<bool>._)).MustHaveHappened();
      }

      [Observation]
      public void should_syncrhonize_the_analysis_defined_in_the_population()
      {
         A.CallTo(() => _populationSimulationAnalysisSynchronizer.UpdateAnalysesDefinedIn(_populationSimulation)).MustHaveHappened();
      }
   }
}