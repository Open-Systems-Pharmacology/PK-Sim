using System.Data;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
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
      protected PopulationRunResults _runResults;
      protected SimulationRunOptions _simulationRunOptions;
      protected IDialogCreator _dialogCreator;

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
         _dialogCreator = A.Fake<IDialogCreator>();

         sut = new PopulationSimulationEngine(_populationRunner,
            _progressManager,
            _eventPublisher,
            _simulationResultsSynchronizer,
            _popExportTask,
            _simMapper,
            _userSettings,
            _populationSimulationAnalysisSynchronizer,
            _dialogCreator
         );

         A.CallTo(() => _progressManager.Create()).Returns(_progressUpdater);

         _populationSimulation = A.Fake<PopulationSimulation>();
         _modelSimulation = A.Fake<IModelCoreSimulation>();
         _populationData = A.Fake<DataTable>();
         _runResults = new PopulationRunResults();

         A.CallTo(() => _popExportTask.CreatePopulationDataFor(_populationSimulation, A<bool>._)).Returns(_populationData);
         A.CallTo(() => _simMapper.MapFrom(_populationSimulation, false)).Returns(_modelSimulation);
         A.CallTo(() => _populationRunner.RunPopulationAsync(_modelSimulation, A<RunOptions>._, _populationData, A<DataTable>._, A<DataTable>._, CancellationToken.None)).Returns(_runResults);

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

   public class When_the_simulation_results_has_at_least_one_failure : concern_for_PopulationSimulationEngine
   {
      private string _message;

      protected override async Task Context()
      {
         await base.Context();
         var individualResult = new IndividualResults { Id = 0, IndividualId = 0 };
         _runResults.Add(individualResult);
         _runResults.AddFailure(1, "Failed Simulation");

         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._))
            .Invokes(x => _message = x.GetArgument<string>(0));
         _simulationRunOptions = new SimulationRunOptions { RaiseEvents = false };

         A.CallTo(_populationRunner)
            .WithReturnType<Task<PopulationRunResults>>()
            .WithAnyArguments()
            .Returns(_runResults);
      }

      protected override Task Because()
      {
         return sut.RunAsync(_populationSimulation, _simulationRunOptions);
      }

      [Observation]
      public void should_show_message_with_simulations_failed()
      {
         _message.Contains("1 out of 2 individuals failed for simulation").ShouldBeTrue();
      }
   }

   public class When_the_population_run_has_multiple_failures_ids_are_listed_in_message : concern_for_PopulationSimulationEngine
   {
      private string _message;

      protected override async Task Context()
      {
         await base.Context();

         var individualResult1 = new IndividualResults { Id = 0, IndividualId = 0 };
         var individualResult2 = new IndividualResults { Id = 1, IndividualId = 1 };
         var individualResult3 = new IndividualResults { Id = 2, IndividualId = 2 };
         _runResults.Add(individualResult1);
         _runResults.Add(individualResult2);
         _runResults.Add(individualResult3);

         _runResults.AddFailure(1, "Failed Individual 1");
         _runResults.AddFailure(2, "Failed Individual 2");

         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._))
            .Invokes(call => _message = call.GetArgument<string>(0));

         _simulationRunOptions = new SimulationRunOptions { RaiseEvents = false };

         A.CallTo(_populationRunner)
            .WithReturnType<Task<PopulationRunResults>>()
            .WithAnyArguments()
            .Returns(_runResults);
      }

      protected override Task Because()
      {
         return sut.RunAsync(_populationSimulation, _simulationRunOptions);
      }

      [Observation]
      public void should_list_failing_individual_ids_in_message()
      {
         _message.ShouldNotBeNull();
         _message.Contains("1").ShouldBeTrue();
         _message.Contains("2").ShouldBeTrue();
      }
   }
}