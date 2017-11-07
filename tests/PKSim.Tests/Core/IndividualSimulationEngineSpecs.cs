using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SimModelNET;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualSimulationEngine : ContextSpecificationAsync<ISimulationEngine<IndividualSimulation>>
   {
      protected ISimModelManager _simModelManager;
      protected IProgressUpdater _progressUpdater;
      protected IEventPublisher _eventPublisher;
      protected ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      protected ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      protected IProgressManager _progressManager;
      protected SimulationRunOptions _simulationRunOption;

      protected override Task Context()
      {
         _simModelManager = A.Fake<ISimModelManager>();
         _progressUpdater = A.Fake<IProgressUpdater>();
         _progressManager = A.Fake<IProgressManager>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _simulationResultsSynchronizer = A.Fake<ISimulationResultsSynchronizer>();
         _modelCoreSimulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();

         sut = new IndividualSimulationEngine(_simModelManager, _progressManager, _simulationResultsSynchronizer,
            _eventPublisher, _modelCoreSimulationMapper);

         A.CallTo(() => _progressManager.Create()).Returns(_progressUpdater);
         _simulationRunOption = new SimulationRunOptions {RaiseEvents = true};
         return _completed;
      }
   }

   public class When_the_simulation_engine_is_running_a_simulation : concern_for_IndividualSimulationEngine
   {
      private IndividualSimulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _simulation.Name = "Hello";
         _simulation.DataRepository = new DataRepository();
         _simulation.CompoundPKFor("TOTO").AucIV = 55;
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(true, Enumerable.Empty<ISolverWarning>(), new DataRepository()));
      }

      protected override Task Because()
      {
         return sut.RunAsync(_simulation, _simulationRunOption);
      }

      [Observation]
      public void should_initialize_the_progress_updater_for_percentage()
      {
         A.CallTo(() => _progressUpdater.Initialize(100, PKSimConstants.UI.Calculating)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_the_simulation_started_and_finishing_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunStartedEvent>._)).MustHaveHappened();
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunFinishedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_the_simulation_results_updated_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void should_reset_the_value_of_the_auc_for_iv()
      {
         _simulation.CompoundPKFor("TOTO").AucIV.ShouldBeNull();
      }

      [Observation]
      public void should_update_the_name_of_the_data_repository_to_match_the_name_of_the_simulation()
      {
         _simulation.DataRepository.Name.ShouldBeEqualTo(_simulation.Name);
      }
   }

   public class When_the_simulation_engine_is_being_notified_that_the_simulation_run_is_terminated : concern_for_IndividualSimulationEngine
   {
      private IndividualSimulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(true, Enumerable.Empty<ISolverWarning>(), new DataRepository()));
         await sut.RunAsync(_simulation, _simulationRunOption);
      }

      protected override Task Because()
      {
         _simModelManager.Terminated += Raise.WithEmpty();
         return _completed;
      }

      [Observation]
      public void should_dispose_the_progress_updater()
      {
         A.CallTo(() => _progressUpdater.Dispose()).MustHaveHappened();
      }
   }

   public class When_running_a_simulation_synchronously : concern_for_IndividualSimulationEngine
   {
      private IndividualSimulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _simulationRunOption.RaiseEvents = false;
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(true, Enumerable.Empty<ISolverWarning>(), new DataRepository()));
      }

      protected override Task Because()
      {
         return sut.RunAsync(_simulation, _simulationRunOption);
      }

      [Observation]
      public void should_not_notify_the_value_changed_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustNotHaveHappened();
      }
   }

   public class When_the_simulation_engine_is_being_notified_that_the_simulation_run_was_not_successful : concern_for_IndividualSimulationEngine
   {
      private IndividualSimulation _simulation;
      private DataRepository _dataRepository;

      protected override async Task Context()
      {
         await base.Context();
         _dataRepository = A.Fake<DataRepository>();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(false, Enumerable.Empty<ISolverWarning>(), _dataRepository));
         await sut.RunAsync(_simulation, _simulationRunOption);
      }

      [Observation]
      public void should_not_update_the_results()
      {
         A.CallTo(() => _simulationResultsSynchronizer.Synchronize(_simulation, _dataRepository)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_notify_the_value_changed_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustNotHaveHappened();
      }
   }

   public class When_the_simulation_engine_is_asked_to_stop_a_simulation_run : concern_for_IndividualSimulationEngine
   {
      protected override Task Because()
      {
         sut.Stop();
         return _completed;
      }

      [Observation]
      public void should_stop_the_simulation()
      {
         A.CallTo(() => _simModelManager.StopSimulation()).MustHaveHappened();
      }
   }
}