using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using SimModelNET;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualSimulationEngine : ContextSpecification<ISimulationEngine<IndividualSimulation>>
   {
      protected ISimModelManager _simModelManager;
      protected IProgressUpdater _progressUpdater;
      protected IEventPublisher _eventPublisher;
      protected IExceptionManager _exceptionManager;
      protected ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      protected ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      protected IProgressManager _progressManager;
      protected ISimulationPersistableUpdater _simulationPersistableUpdater;

      protected override void Context()
      {
         _simModelManager = A.Fake<ISimModelManager>();
         _progressUpdater = A.Fake<IProgressUpdater>();
         _progressManager = A.Fake<IProgressManager>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _exceptionManager = A.Fake<IExceptionManager>();
         _simulationResultsSynchronizer = A.Fake<ISimulationResultsSynchronizer>();
         _modelCoreSimulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _simulationPersistableUpdater = A.Fake<ISimulationPersistableUpdater>();
         A.CallTo(() => _progressManager.Create()).Returns(_progressUpdater);
         sut = new IndividualSimulationEngine(_simModelManager, _progressManager, _simulationResultsSynchronizer,
            _eventPublisher, _exceptionManager, _modelCoreSimulationMapper, _simulationPersistableUpdater);
      }
   }

   public class When_the_simulation_engine_is_running_a_simulation : concern_for_IndividualSimulationEngine
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _simulation.CompoundPKFor("TOTO").AucIV = 55;
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(true, Enumerable.Empty<ISolverWarning>(), new DataRepository()));
      }

      [Observation]
      public async Task should_initialize_the_progress_updater_for_percentage()
      {
         await sut.RunAsync(_simulation);
         A.CallTo(() => _progressUpdater.Initialize(100, PKSimConstants.UI.Calculating)).MustHaveHappened();
      }

      [Observation]
      public async Task should_notify_the_simulation_started_and_finishing_event()
      {
         await sut.RunAsync(_simulation);
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunStartedEvent>._)).MustHaveHappened();
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationRunFinishedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public async Task should_notify_the_simulation_results_updated_event()
      {
         await sut.RunAsync(_simulation);
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public async Task should_reset_the_value_of_the_auc_for_iv()
      {
         await sut.RunAsync(_simulation);
         _simulation.CompoundPKFor("TOTO").AucIV.ShouldBeNull();
      }
   }

   public class When_the_simulation_engine_is_being_notified_that_the_simulation_run_is_terminated : concern_for_IndividualSimulationEngine
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(true, Enumerable.Empty<ISolverWarning>(), new DataRepository()));
         sut.RunAsync(_simulation);
      }

      protected override void Because()
      {
         //necesarry to add a thread sleep to prevent observation being asserted before the actual call
         Thread.Sleep(1000);
         _simModelManager.Terminated += Raise.WithEmpty();
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

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(true, Enumerable.Empty<ISolverWarning>(), new DataRepository()));
      }

      protected override void Because()
      {
         sut.Run(_simulation);
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

      protected override void Context()
      {
         base.Context();
         _dataRepository= A.Fake<DataRepository>();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(_simModelManager).WithReturnType<SimulationRunResults>().Returns(new SimulationRunResults(false, Enumerable.Empty<ISolverWarning>(), _dataRepository));
         sut.Run(_simulation);
      }

      [Observation]
      public void should_not_update_the_results()
      {
          A.CallTo(() =>_simulationResultsSynchronizer.Synchronize(_simulation, _dataRepository)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_notify_the_value_changed_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustNotHaveHappened();
      }
   }
   public class When_the_simulation_engine_is_asked_to_stop_a_simulation_run : concern_for_IndividualSimulationEngine
   {
      protected override void Because()
      {
         sut.Stop();
      }

      [Observation]
      public void should_stop_the_simulation()
      {
         A.CallTo(() => _simModelManager.StopSimulation()).MustHaveHappened();
      }
   }

}