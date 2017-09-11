using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationTask : ContextSpecification<ISimulationTask>
   {
      protected IExecutionContext _executionContext;
      protected ICreateSimulationPresenter _createSimulationPresenter;
      protected IBuildingBlockTask _buildingBlockTask;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected IConfigureSimulationTask _configureSimulationTask;
      protected ISimulationResultsImportTask _simulationResultsImportTask;
      protected IApplicationController _applicationController;
      private ISimulationParametersToBuildingBlockUpdater _simulationParametersToBlockUpdater;
      protected IBuildingBlockParametersToSimulationUpdater _blockParametersToSimulationUpdater;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _configureSimulationTask = A.Fake<IConfigureSimulationTask>();
         _createSimulationPresenter = A.Fake<ICreateSimulationPresenter>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _applicationController = A.Fake<IApplicationController>();
         _simulationParametersToBlockUpdater = A.Fake<ISimulationParametersToBuildingBlockUpdater>();
         _blockParametersToSimulationUpdater = A.Fake<IBuildingBlockParametersToSimulationUpdater>();
         A.CallTo(() => _applicationController.Start<ICreateSimulationPresenter>()).Returns(_createSimulationPresenter);
         sut = new SimulationTask(_executionContext, _buildingBlockTask, _applicationController, _simulationBuildingBlockUpdater,
            _configureSimulationTask, _blockParametersToSimulationUpdater, _simulationParametersToBlockUpdater);
      }
   }

   public class When_creating_a_simulation_with_the_simulation_task : concern_for_SimulationTask
   {
      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_start_the_create_simulation_view()
      {
         A.CallTo(() => _applicationController.Start<ICreateSimulationPresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_close_the_create_simulation_view_when_the_operation_is_finished()
      {
         A.CallTo(() => _createSimulationPresenter.Dispose()).MustHaveHappened();
      }
   }

   public class When_the_simulation_task_is_asked_to_perform_an_update_from_a_template_building_block_that_can_be_performed_as_a_quick_update : concern_for_SimulationTask
   {
      private Simulation _simulation;
      private IPKSimBuildingBlock _templateBuildingBlock;
      private IPKSimCommand _updateCommand;
      private UsedBuildingBlock _usedBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _updateCommand = A.Fake<IPKSimCommand>();
         _usedBuildingBlock = A.Fake<UsedBuildingBlock>();
         A.CallTo(() => _simulationBuildingBlockUpdater.QuickUpdatePossibleFor(_templateBuildingBlock, _usedBuildingBlock)).Returns(true);
         A.CallTo(() => _blockParametersToSimulationUpdater.UpdateParametersFromBuildingBlockInSimulation(_templateBuildingBlock, _simulation)).Returns(_updateCommand);
      }

      protected override void Because()
      {
         sut.UpdateUsedBuildingBlockInSimulation(_templateBuildingBlock, _usedBuildingBlock, _simulation);
      }

      [Observation]
      public void should_perform_a_quick_update_and_add_the_update_command_to_the_workspace()
      {
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(_updateCommand)).MustHaveHappened();
      }
   }

   public class When_the_simulation_task_is_asked_to_perform_an_update_from_a_template_building_block_that_requires_a_full_update : concern_for_SimulationTask
   {
      private Simulation _simulation;
      private IPKSimBuildingBlock _templateBuildingBlock;
      private PKSimProject _project;
      private UsedBuildingBlock _usedBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _project = A.Fake<PKSimProject>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         _usedBuildingBlock = A.Fake<UsedBuildingBlock>();
         A.CallTo(() => _simulationBuildingBlockUpdater.QuickUpdatePossibleFor(_templateBuildingBlock, _usedBuildingBlock)).Returns(false);
      }

      protected override void Because()
      {
         sut.UpdateUsedBuildingBlockInSimulation(_templateBuildingBlock, _usedBuildingBlock, _simulation);
      }

      [Observation]
      public void should_start_the_configure_workflow_with_the_given_template_building_block()
      {
         A.CallTo(() => _configureSimulationTask.Configure(_simulation, _templateBuildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_simulation_task_is_asked_to_edit_an_individual_simulation : concern_for_SimulationTask
   {
      private IndividualSimulation _individualSimulation;

      protected override void Context()
      {
         base.Context();
         _individualSimulation = A.Fake<IndividualSimulation>();
      }

      protected override void Because()
      {
         sut.Edit(_individualSimulation);
      }

      [Observation]
      public void should_starts_the_edit_individual_simulation_presenter()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_individualSimulation)).MustHaveHappened();
      }
   }

   public class When_the_simulation_task_is_asked_to_edit_a_population_simulation : concern_for_SimulationTask
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
      }

      protected override void Because()
      {
         sut.Edit(_populationSimulation);
      }

      [Observation]
      public void should_starts_the_edit_population_simulation_presenter()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_populationSimulation)).MustHaveHappened();
      }
   }

   public class When_the_simulation_task_is_imported_results_into_a_simulation : concern_for_SimulationTask
   {
      private PopulationSimulation _populationSimulation;
      private IImportSimulationResultsPresenter _importSimulationResultsPresenter;
      private SimulationResults _simulationResults;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _simulationResults = new SimulationResults();
         _importSimulationResultsPresenter = A.Fake<IImportSimulationResultsPresenter>();
         A.CallTo(() => _applicationController.Start<IImportSimulationResultsPresenter>()).Returns(_importSimulationResultsPresenter);
         A.CallTo(() => _importSimulationResultsPresenter.ImportResultsFor(_populationSimulation)).Returns(_simulationResults);
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(A<IPKSimCommand>._))
            .Invokes(x => _command = x.GetArgument<IPKSimCommand>(0));
      }

      protected override void Because()
      {
         sut.ImportResultsIn(_populationSimulation);
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _buildingBlockTask.Load(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_command_to_the_history()
      {
         var command = _command as SetPopulationSimulationResultsCommand;
         command.ShouldNotBeNull();
      }
   }

   public class When_the_simulation_task_is_importing_pk_parameters_into_a_simulation : concern_for_SimulationTask
   {
      private PopulationSimulation _populationSimulation;
      private IImportSimulationPKAnalysesPresenter _importSimulationResultsPresenter;
      private IPKSimCommand _command;
      private List<QuantityPKParameter> _pkAnalyses;
      private QuantityPKParameter _pkParameter1;
      private QuantityPKParameter _pkParameter2;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _pkParameter1 = new QuantityPKParameter();
         _pkParameter2 = new QuantityPKParameter();
         _pkAnalyses = new List<QuantityPKParameter> {_pkParameter1, _pkParameter2};
         _importSimulationResultsPresenter = A.Fake<IImportSimulationPKAnalysesPresenter>();
         A.CallTo(() => _applicationController.Start<IImportSimulationPKAnalysesPresenter>()).Returns(_importSimulationResultsPresenter);
         A.CallTo(() => _importSimulationResultsPresenter.ImportPKAnalyses(_populationSimulation)).Returns(_pkAnalyses);
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(A<IPKSimCommand>._))
            .Invokes(x => _command = x.GetArgument<IPKSimCommand>(0));
      }

      protected override void Because()
      {
         sut.ImportPKAnalyses(_populationSimulation);
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _buildingBlockTask.Load(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_command_to_the_history()
      {
         var command = _command as AddPKAnalysesToSimulationCommand;
         command.ShouldNotBeNull();
      }
   }

   public class When_showing_the_difference_with_template_for_a_building_block_supporting_quick_update : concern_for_SimulationTask
   {
      private IPKSimBuildingBlock _templateBuildingBlock;
      private UsedBuildingBlock _usedBuildingBlock;
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _usedBuildingBlock = new UsedBuildingBlock("X", PKSimBuildingBlockType.Protocol);
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulationBuildingBlockUpdater.BuildingBlockSupportsQuickUpdate(_templateBuildingBlock)).Returns(true);
      }

      protected override void Because()
      {
         sut.ShowDifferencesBetween(_templateBuildingBlock, _usedBuildingBlock, _simulation);
      }

      [Observation]
      public void should_start_the_comparison()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<StartComparisonEvent>._)).MustHaveHappened();
      }
   }

   public class When_showing_the_difference_with_template_for_a_building_block_not_supporting_quick_update : concern_for_SimulationTask
   {
      private IPKSimBuildingBlock _templateBuildingBlock;
      private UsedBuildingBlock _usedBuildingBlock;
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _usedBuildingBlock = new UsedBuildingBlock("X", PKSimBuildingBlockType.Protocol);
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulationBuildingBlockUpdater.BuildingBlockSupportsQuickUpdate(_templateBuildingBlock)).Returns(false);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ShowDifferencesBetween(_templateBuildingBlock, _usedBuildingBlock, _simulation)).ShouldThrowAn<PKSimException>();
      }
   }
}