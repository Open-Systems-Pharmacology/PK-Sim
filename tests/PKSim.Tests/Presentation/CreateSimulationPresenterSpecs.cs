using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateSimulationPresenter : ContextSpecification<ICreateSimulationPresenter>
   {
      protected ICreateSimulationView _view;
      protected ISimulationModelConfigurationPresenter _simulationModelConfigurationPresenter;
      protected ISimulationCompoundConfigurationCollectorPresenter _simulationCompoundConfigurationCollectorPresenter;
      protected IBuildingBlockPropertiesMapper _simulationPropertiesMapper;
      protected ObjectBaseDTO _simulationPropertiesDTO;
      protected ISimulationCompoundProtocolCollectorPresenter _simulationCompoundProtocolCollectorPresenter;
      protected ISubPresenterItemManager<ISimulationItemPresenter> _subPresenterManager;
      protected IObjectBaseDTOFactory _buildingBlockDTOFactory;
      protected ISimulationModelCreator _simulationModelCreator;
      private IHeavyWorkManager _heavyWorkManager;
      protected ISimulationEventsConfigurationPresenter _simulationEventsConfigurationPresenter;
      private IDialogCreator _dialogCreator;
      protected ISimulationCompoundProcessSummaryCollectorPresenter _simulationCompoundProcessCollectorPresenter;

      protected override void Context()
      {
         _simulationPropertiesDTO = new ObjectBaseDTO();
         _subPresenterManager = SubPresenterHelper.Create<ISimulationItemPresenter>();
         _simulationModelConfigurationPresenter = _subPresenterManager.CreateFake(SimulationItems.Model);
         _simulationCompoundConfigurationCollectorPresenter = _subPresenterManager.CreateFake(SimulationItems.Compounds);
         _simulationCompoundProtocolCollectorPresenter = _subPresenterManager.CreateFake(SimulationItems.CompoundProtocols);
         _simulationCompoundProcessCollectorPresenter = _subPresenterManager.CreateFake(SimulationItems.CompoundsProcesses);
         _simulationEventsConfigurationPresenter = _subPresenterManager.CreateFake(SimulationItems.Events);
         _view = A.Fake<ICreateSimulationView>();
         _buildingBlockDTOFactory = A.Fake<IObjectBaseDTOFactory>();
         _simulationPropertiesMapper = A.Fake<IBuildingBlockPropertiesMapper>();
         A.CallTo(() => _buildingBlockDTOFactory.CreateFor<Simulation>()).Returns(_simulationPropertiesDTO);
         _simulationModelCreator = A.Fake<ISimulationModelCreator>();
         _heavyWorkManager = new HeavyWorkManagerForSpecs();
         _dialogCreator = A.Fake<IDialogCreator>();

         sut = new CreateSimulationPresenter(_view, _subPresenterManager, _simulationModelCreator, _heavyWorkManager, _simulationPropertiesMapper, _buildingBlockDTOFactory, _dialogCreator);
         sut.Initialize();
      }
   }

   public class When_initializing_the_create_simulation_presenter : concern_for_CreateSimulationPresenter
   {
      [Observation]
      public void should_tell_the_view_to_render_the_sub_presenter_views()
      {
         A.CallTo(() => _subPresenterManager.InitializeWith(sut, SimulationItems.All)).MustHaveHappened();
      }
   }

   public class When_starting_the_create_simulation_presenter : concern_for_CreateSimulationPresenter
   {
      protected override void Because()
      {
         sut.Create();
      }

      [Observation]
      public void should_ask_the_view_to_render()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_enable_the_model_configuration_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.Model, true)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_application_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.CompoundProtocols, false)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_compound_setup_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.Compounds, false)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_events_setup_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.Events, false)).MustHaveHappened();
      }
   }

   public class When_the_user_press_the_ok_button_to_confirm_the_creation_of_the_simulation : concern_for_CreateSimulationPresenter
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulationModelConfigurationPresenter.Simulation).Returns(_simulation);
         sut.Create();
      }

      protected override void Because()
      {
         sut.CreateSimulation();
      }

      [Observation]
      public void should_update_the_simulation_properties_before_closing_the_view()
      {
         A.CallTo(() => _simulationPropertiesMapper.MapProperties(_simulationPropertiesDTO, _simulationModelConfigurationPresenter.Simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_compound_for_the_simulation()
      {
         A.CallTo(() => _simulationCompoundConfigurationCollectorPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_application_for_the_simulation()
      {
         A.CallTo(() => _simulationCompoundProtocolCollectorPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_create_the_event_mapping_for_the_simulation()
      {
         A.CallTo(() => _simulationEventsConfigurationPresenter.SaveConfiguration()).MustHaveHappened();
      }


      [Observation]
      public void should_create_the_process_mapping_for_the_simulatio()
      {
         A.CallTo(() => _simulationCompoundProcessCollectorPresenter.SaveConfiguration()).MustHaveHappened();
      }
   }

   public class When_the_user_cancel_the_creation_of_the_simulation : concern_for_CreateSimulationPresenter
   {
      private IPKSimCommand _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.Create();
      }

      [Observation]
      public void should_not_update_the_simulation_properties()
      {
         A.CallTo(() => _simulationPropertiesMapper.MapProperties(A<ObjectBaseDTO>._, A<IPKSimBuildingBlock>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_the_user_model_configuration_was_performed : concern_for_CreateSimulationPresenter
   {
      protected Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulationModelConfigurationPresenter.Simulation).Returns(_simulation);
      }

      protected override void Because()
      {
         sut.ModelConfigurationDone();
      }

      [Observation]
      public void should_create_a_simulation()
      {
         A.CallTo(() =>  _simulationModelConfigurationPresenter.CreateSimulation()).MustHaveHappened();
      }


      [Observation]
      public void should_save_the_available_configuration_in_all_building_block_presetners_to_avoid_reset()
      {
         A.CallTo(() => _simulationCompoundProtocolCollectorPresenter.SaveConfiguration()).MustHaveHappened();
         A.CallTo(() => _simulationCompoundConfigurationCollectorPresenter.SaveConfiguration()).MustHaveHappened();
         A.CallTo(() => _simulationEventsConfigurationPresenter.SaveConfiguration()).MustHaveHappened();
         A.CallTo(() => _simulationCompoundProcessCollectorPresenter.SaveConfiguration()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_application_configuration()
      {
         A.CallTo(() => _simulationCompoundProtocolCollectorPresenter.EditSimulation(_simulation, CreationMode.New)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_compound_configuration()
      {
         A.CallTo(() => _simulationCompoundConfigurationCollectorPresenter.EditSimulation(_simulation, CreationMode.New)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_events_configuration()
      {
         A.CallTo(() => _simulationEventsConfigurationPresenter.EditSimulation(_simulation, CreationMode.New)).MustHaveHappened();
      }
   }

   public class When_the_create_simulation_presenter_is_being_notified_that_one_of_it_sub_presenter_has_changed : concern_for_CreateSimulationPresenter
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulationModelConfigurationPresenter.Simulation).Returns(_simulation);
         A.CallTo(() => _simulationModelConfigurationPresenter.SimulationCreated).Returns(true);
         A.CallTo(() => _simulationCompoundConfigurationCollectorPresenter.CanClose).Returns(false);
      }

      protected override void Because()
      {
         _simulationModelConfigurationPresenter.StatusChanged += Raise.WithEmpty();
      }

      [Observation]
      public void should_enable_the_compound_setup_view_if_the_model_properties_have_been_defined()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.Compounds, _simulationModelConfigurationPresenter.SimulationCreated)).MustHaveHappened();
      }

      [Observation]
      public void should_enable_the_application_setup_view_if_the_model_properties_have_been_defined()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.CompoundProtocols, _simulationModelConfigurationPresenter.SimulationCreated)).MustHaveHappened();
      }

      [Observation]
      public void should_enable_the_events_setup_view_if_the_model_properties_have_been_defined()
      {
         A.CallTo(() => _view.SetControlEnabled(SimulationItems.Events, _simulationModelConfigurationPresenter.SimulationCreated)).MustHaveHappened();
      }

      [Observation]
      public void should_enable_the_next_button_if_the_simulation_was_created()
      {
         _view.NextEnabled.ShouldBeEqualTo(_simulationModelConfigurationPresenter.CanClose);
      }
   }

   public class When_the_create_simulation_presenter_is_being_notified_that_the_view_has_changed : concern_for_CreateSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulationModelConfigurationPresenter.SimulationCreated).Returns(true);
         A.CallTo(() => _simulationCompoundConfigurationCollectorPresenter.CanClose).Returns(true);
      }

      [TestCase(true, false)]
      [TestCase(false, false)]
      [TestCase(false, true)]
      [TestCase(true, true)]
      public void should_enable_the_ok_button_only_if_all_presenters_can_be_closed_and_the_view_is_not_dirty(bool presenterCanClose, bool viewIsDirty)
      {
         A.CallTo(() => _subPresenterManager.CanClose).Returns(presenterCanClose);
         A.CallTo(() => _view.HasError).Returns(viewIsDirty);

         sut.ViewChanged();

         _view.OkEnabled.ShouldBeEqualTo(presenterCanClose && !viewIsDirty);
      }
   }
}