using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_ConfigureSimulationTask : ContextSpecification<IConfigureSimulationTask>
   {
      protected IExecutionContext _executionContext;
      protected IBuildingBlockTask _buildingBlockTask;
      protected IApplicationController _applicationController;
      protected IActiveSubjectRetriever _activeSubjectRetriever;
      protected ISimulationSettingsRetriever _simulationSettingsRetriever;
      protected IParameterIdUpdater _parameterIdUpdater;
      protected ISimulationResultsTask _simulationResultsTask;
      protected IParameterIdentificationTask _parameterIdentificationTask;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _applicationController = A.Fake<IApplicationController>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         _simulationSettingsRetriever = A.Fake<ISimulationSettingsRetriever>();
         _parameterIdUpdater = A.Fake<IParameterIdUpdater>();
         _simulationResultsTask= A.Fake<ISimulationResultsTask>();
         _parameterIdentificationTask = A.Fake<IParameterIdentificationTask>();
         sut = new ConfigureSimulationTask(_buildingBlockTask, _activeSubjectRetriever, _simulationSettingsRetriever, _applicationController,
            _executionContext, _parameterIdUpdater,_simulationResultsTask);
      }
   }

   public class When_the_simulation_task_is_configuring_a_simulation_containing_plots_and_results_ : concern_for_ConfigureSimulationTask
   {
      private Simulation _simulationToConfigure;
      private IConfigureSimulationPresenter _configurePresenter;
      private Simulation _newSimulation;
      private IPKSimCommand _pkSimCommand;
      private PKSimProject _project;

      protected override void Context()
      {
         base.Context();
         _pkSimCommand = A.Fake<IPKSimCommand>();
         _project = A.Fake<PKSimProject>();
         _configurePresenter = A.Fake<IConfigureSimulationPresenter>();
         _simulationToConfigure = new IndividualSimulation().WithId("oldSim");
         _newSimulation = new IndividualSimulation().WithId("newSim");
         A.CallTo(() => _applicationController.Start<IConfigureSimulationPresenter>()).Returns(_configurePresenter);
         A.CallTo(() => _configurePresenter.Simulation).Returns(_newSimulation);
         A.CallTo(() => _configurePresenter.ConfigureSimulation(_simulationToConfigure)).Returns(_pkSimCommand);
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         //this is used in the swap command
         var simulationCommandDescriptionBuilder = A.Fake<ISimulationCommandDescriptionBuilder>();
         A.CallTo(() => simulationCommandDescriptionBuilder.BuildDifferenceBetween(_simulationToConfigure, _newSimulation)).Returns(new ReportPart());
         A.CallTo(() => _executionContext.Resolve<ISimulationCommandDescriptionBuilder>()).Returns(simulationCommandDescriptionBuilder);
      }

      protected override void Because()
      {
         sut.Configure(_simulationToConfigure);
      }

      [Observation]
      public void should_load_the_simulation_to_configure_before_launching_the_configuation()
      {
         A.CallTo(() => _buildingBlockTask.Load(_simulationToConfigure)).MustHaveHappened();
      }


      [Observation]
      public void should_start_the_presenter_to_configure_the_simulation()
      {
         A.CallTo(() => _configurePresenter.ConfigureSimulation(_simulationToConfigure)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_new_simulation_to_the_project()
      {
         A.CallTo(() => _project.AddBuildingBlock(_newSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_synchronize_the_settings_in_the_simulation()
      {
         A.CallTo(() => _simulationSettingsRetriever.SynchronizeSettingsIn(_newSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_creation_mode_of_the_new_simulation_to_configure()
      {
         _newSimulation.Creation.CreationMode.ShouldBeEqualTo(CreationMode.Configure);
      }

      [Observation]
      public void should_remove_the_old_simulation_from_the_project()
      {
         A.CallTo(() => _project.RemoveBuildingBlock(_simulationToConfigure)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_id_of_all_parmaeters_in_the_new_simulation()
      {
         A.CallTo(() => _parameterIdUpdater.UpdateSimulationId(_newSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_the_id_of_the_new_simulation_equal_to_the_id_of_the_old_simulation()
      {
         _newSimulation.Id.ShouldBeEqualTo(_simulationToConfigure.Id);
      }

      [Observation]
      public void should_have_copied_the_simulation_results()
      {
         A.CallTo(() => _simulationResultsTask.CopyResults(_simulationToConfigure, _newSimulation)).MustHaveHappened();
      }
   }
}