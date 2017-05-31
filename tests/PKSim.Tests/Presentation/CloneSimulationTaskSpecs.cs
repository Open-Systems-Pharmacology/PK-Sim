using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_CloneSimulationTask : ContextSpecification<ICloneSimulationTask>
   {
      protected IBuildingBlockTask _buildingBlockTask;
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      protected ISimulationSettingsRetriever _simulationSettingsRetriever;
      protected ISimulationResultsTask _simulationResultsTask;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         _simulationSettingsRetriever = A.Fake<ISimulationSettingsRetriever>();
         _simulationResultsTask = A.Fake<ISimulationResultsTask>();
         _applicationController= A.Fake<IApplicationController>();
         sut = new CloneSimulationTask(_buildingBlockTask,  _buildingBlockInSimulationManager, _simulationSettingsRetriever, _simulationResultsTask, _applicationController);
      }
   }

   public class When_the_simulation_task_is_asked_to_clone_a_simulation_that_has_its_building_block_in_a_green_state : concern_for_CloneSimulationTask
   {
      private Simulation _simulationToClone;
      private ICloneSimulationPresenter _cloneSimulationPresenter;
      private Simulation _clonedSimulation;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<IPKSimCommand>();
         _simulationToClone = A.Fake<Simulation>().WithName("OLD");
         _clonedSimulation = A.Fake<Simulation>().WithName("NEW");

         _cloneSimulationPresenter = A.Fake<ICloneSimulationPresenter>();
         A.CallTo(() => _applicationController.Start<ICloneSimulationPresenter>()).Returns(_cloneSimulationPresenter);
         A.CallTo(() => _cloneSimulationPresenter.CloneSimulation(_simulationToClone)).Returns(_command);
         A.CallTo(() => _cloneSimulationPresenter.Simulation).Returns(_clonedSimulation);
         A.CallTo(() => _buildingBlockInSimulationManager.StatusFor(_simulationToClone)).Returns(BuildingBlockStatus.Green);
      }

      protected override void Because()
      {
         sut.Clone(_simulationToClone);
      }

      [Observation]
      public void should_load_the_simulation_to_clone()
      {
         A.CallTo(() => _buildingBlockTask.Load(_simulationToClone)).MustHaveHappened();
      }

      [Observation]
      public void should_start_the_configure_simulation_presenter()
      {
         A.CallTo(() => _cloneSimulationPresenter.CloneSimulation(_simulationToClone)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_new_simulation_to_the_project()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject(_clonedSimulation,true, false )).MustHaveHappened();
      }

      [Observation]
      public void should_add_a_command_to_the_history()
      {
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(A<IPKSimCommand>._)).MustHaveHappened();
      }

      [Observation]
      public void should_synchronize_the_settings_in_the_simulation()
      {
         A.CallTo(() => _simulationSettingsRetriever.SynchronizeSettingsIn(_clonedSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_have_clone_the_results_from_the_source_simulation()
      {
         A.CallTo(() => _simulationResultsTask.CloneResults(_simulationToClone, _clonedSimulation)).MustHaveHappened();
      }
   }

   public class When_the_simulation_task_is_asked_to_clone_a_simulation_that_has_is_not_in_a_green_state : concern_for_CloneSimulationTask
   {
      private Simulation _simulationToClone;

      protected override void Context()
      {
         base.Context();
         _simulationToClone = A.Fake<Simulation>();
         A.CallTo(() => _buildingBlockInSimulationManager.StatusFor(_simulationToClone)).Returns(BuildingBlockStatus.Red);
      }

      [Observation]
      public void should_notify_that_the_clone_operation_is_not_allowed()
      {
         The.Action(() => sut.Clone(_simulationToClone)).ShouldThrowAn<PKSimException>();
      }
   }
}