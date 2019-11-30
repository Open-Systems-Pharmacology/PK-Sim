using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ConfigureSimulationPresenter : ContextSpecification<IConfigureSimulationPresenter>
   {
      protected IConfigureSimulationView _view;
      protected ISubPresenterItemManager<ISimulationItemPresenter> _subPresenterManager;
      protected ISimulationModelCreator _simulationModelCreator;
      protected IHeavyWorkManager _heavyWorkManager;
      protected ICloner _cloner;
      protected IDialogCreator _dialogCreator;
      protected ISimulationParametersUpdater _simulationParametersUpdater;
      protected IFullPathDisplayResolver _fullPathDisplayResolver;
      protected ISimulationModelConfigurationPresenter _simulationModelConfigurationPresenter;
      protected ISimulationCompoundConfigurationCollectorPresenter _simulationCompoundsPresenter;
      protected ISimulationCompoundProtocolCollectorPresenter _simulationCompoundProtocolsPresenter;
      protected ISimulationEventsConfigurationPresenter _simulationEventsPresenter;
      protected ISimulationCompoundProcessSummaryCollectorPresenter _simulationCompoundProcessesPresenter;

      protected Simulation _originalSimulation;
      protected Simulation _clonedSimulation;
      protected ValidationResult _validationResult;
      protected IBuildingBlockInSimulationSynchronizer _buildingBlockInSimulationSynchronizer;

      protected override void Context()
      {
         _view = A.Fake<IConfigureSimulationView>();
         _subPresenterManager = SubPresenterHelper.Create<ISimulationItemPresenter>();
         _simulationModelCreator = A.Fake<ISimulationModelCreator>();
         _heavyWorkManager = new HeavyWorkManagerForSpecs();
         _cloner = A.Fake<ICloner>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _simulationParametersUpdater = A.Fake<ISimulationParametersUpdater>();
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _buildingBlockInSimulationSynchronizer = A.Fake<IBuildingBlockInSimulationSynchronizer>();
         _validationResult=new ValidationResult();

         _simulationModelConfigurationPresenter = _subPresenterManager.CreateFake(SimulationItems.Model);
         _simulationCompoundsPresenter = _subPresenterManager.CreateFake(SimulationItems.Compounds);
         _simulationCompoundProtocolsPresenter = _subPresenterManager.CreateFake(SimulationItems.CompoundProtocols);
         _simulationCompoundProcessesPresenter = _subPresenterManager.CreateFake(SimulationItems.CompoundsProcesses);
         _simulationEventsPresenter = _subPresenterManager.CreateFake(SimulationItems.Events);

         sut = new ConfigureSimulationPresenter(_view,_subPresenterManager,_simulationModelCreator,_heavyWorkManager,_cloner,_dialogCreator,_simulationParametersUpdater,_fullPathDisplayResolver,_buildingBlockInSimulationSynchronizer);

         _originalSimulation= A.Fake<Simulation>();
         _clonedSimulation= A.Fake<Simulation>();
         A.CallTo(() => _cloner.CloneForModel(_originalSimulation)).Returns(_clonedSimulation);
         A.CallTo(() => _simulationModelConfigurationPresenter.Simulation).Returns(_clonedSimulation);

         A.CallTo(_simulationParametersUpdater).WithReturnType<ValidationResult>().Returns(_validationResult);
      }
   }

   public class When_configuring_a_simulation : concern_for_ConfigureSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.ConfigureSimulation(_originalSimulation);
      }

      protected override void Because()
      {
         sut.CreateSimulation();
      }

      [Observation]
      public void should_ensure_that_the_clone_simulation_that_will_be_configured_is_using_updated_building_blocks()
      {
         A.CallTo(() => _buildingBlockInSimulationSynchronizer.UpdateUsedBuildingBlockBasedOnTemplateIn(_clonedSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_create_a_clone_of_the_simulation_to_configure_and_edit_the_clone()
      {
         A.CallTo(() => _simulationModelConfigurationPresenter.EditSimulation(_clonedSimulation, CreationMode.Configure)).MustHaveHappened();
         A.CallTo(() => _simulationCompoundsPresenter.EditSimulation(_clonedSimulation, CreationMode.Configure)).MustHaveHappened();
         A.CallTo(() => _simulationCompoundProtocolsPresenter.EditSimulation(_clonedSimulation, CreationMode.Configure)).MustHaveHappened();
         A.CallTo(() => _simulationEventsPresenter.EditSimulation(_clonedSimulation, CreationMode.Configure)).MustHaveHappened();
         A.CallTo(() => _simulationCompoundProcessesPresenter.EditSimulation(_clonedSimulation, CreationMode.Configure)).MustHaveHappened();
      }

      [Observation]
      public void should_active_the_model_selection_view()
      {
         A.CallTo(() => _view.ActivateControl(SimulationItems.Model)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_configuration_defined_by_the_user()
      {
         A.CallTo(() => _simulationCompoundsPresenter.SaveConfiguration()).MustHaveHappened();
         A.CallTo(() => _simulationCompoundProtocolsPresenter.SaveConfiguration()).MustHaveHappened();
         A.CallTo(() => _simulationEventsPresenter.SaveConfiguration()).MustHaveHappened();
         A.CallTo(() => _simulationCompoundProcessesPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_parameters_from_the_original_simulation()
      {
         A.CallTo(() => _simulationParametersUpdater.ReconciliateSimulationParametersBetween(_originalSimulation, _clonedSimulation, PKSimBuildingBlockType.Simulation)).MustHaveHappened();
      }
   }

   public class When_configuring_a_simulation_and_the_new_configuration_results_in_some_overwritten_parameters_not_available_anymore : concern_for_ConfigureSimulationPresenter
   {
      private string _fullPathForParameter;
      private string _message;

      protected override void Context()
      {
         base.Context();
         _fullPathForParameter = "BLA BLA BLA";
         var parameter = new PKSimParameter();
         _validationResult.AddMessage(NotificationType.Warning,  parameter,"warning");
         A.CallTo(_fullPathDisplayResolver).WithReturnType<string>().Returns(_fullPathForParameter);

         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._))
            .Invokes(x => _message = x.GetArgument<string>(0));

         sut.ConfigureSimulation(_originalSimulation);
      }

      protected override void Because()
      {
         sut.CreateSimulation();
      }

      [Observation]
      public void should_display_a_warning_to_the_user_with_the_name_of_the_full_path_of_all_parameters_that_will_not_be_used_anymore()
      {
         _message.Contains(_fullPathForParameter).ShouldBeTrue();
         _message.Contains("These parameters were changed by the user. Because of a simulation reconfiguration, they will not be used for this simulation").ShouldBeTrue();
      }
      
   }
}	