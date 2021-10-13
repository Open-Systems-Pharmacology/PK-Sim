using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationObserversConfigurationPresenter : ContextSpecification<ISimulationObserversConfigurationPresenter>
   {
      protected ISimulationObserversConfigurationView _view;
      protected IObserverSetMappingToObserverSetMappingDTOMapper _observerSetDTOMapper;
      protected IObserverSetTask _observerSetTask;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      protected Simulation _simulation;
      protected ObserverSetProperties _observerSetProperties;
      protected ObserverSetMapping _observerSetMapping;
      protected ObserverSet _observerSetTemplate1;
      protected ObserverSetMappingDTO _observerSetMappingDTO;
      protected IEnumerable<ObserverSetMappingDTO> _allObserverSetMappingDTO;
      protected ObserverSet _observerSetTemplate2;

      protected const string TEMPLATE_OBSERVER_SET_ID_1 = "TEMPLATE_OBSERVER_SET_ID_1";
      protected const string TEMPLATE_OBSERVER_SET_ID_2 = "TEMPLATE_OBSERVER_SET_ID_2";

      protected override void Context()
      {
         _view = A.Fake<ISimulationObserversConfigurationView>();
         _observerSetDTOMapper = A.Fake<IObserverSetMappingToObserverSetMappingDTOMapper>();
         _observerSetTask = A.Fake<IObserverSetTask>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _buildingBlockSelectionDisplayer = A.Fake<IBuildingBlockSelectionDisplayer>();
         sut = new SimulationObserversConfigurationPresenter(_view, _observerSetDTOMapper, _observerSetTask, _simulationBuildingBlockUpdater, _buildingBlockSelectionDisplayer);


         _observerSetMapping = new ObserverSetMapping {TemplateObserverSetId = TEMPLATE_OBSERVER_SET_ID_1};

         _observerSetProperties = new ObserverSetProperties();
         _observerSetProperties.AddObserverSetMapping(_observerSetMapping);
         _simulation = new IndividualSimulation {Properties = new SimulationProperties {ObserverSetProperties = _observerSetProperties}};

         _observerSetTemplate1 = new ObserverSet().WithName("OBSERVER_SET_1").WithId(TEMPLATE_OBSERVER_SET_ID_1);
         _observerSetTemplate2 = new ObserverSet().WithName("ANOTER_OBSERVER_SET_2").WithId(TEMPLATE_OBSERVER_SET_ID_2);

         A.CallTo(() => _observerSetTask.All()).Returns(new[] {_observerSetTemplate1, _observerSetTemplate2});

         _observerSetMappingDTO = new ObserverSetMappingDTO(_observerSetMapping) {ObserverSet = _observerSetTemplate1};
         A.CallTo(() => _observerSetDTOMapper.MapFrom(_observerSetMapping, _simulation)).Returns(_observerSetMappingDTO);

         A.CallTo(() => _view.BindTo(A<IEnumerable<ObserverSetMappingDTO>>._))
            .Invokes(x => _allObserverSetMappingDTO = x.GetArgument<IEnumerable<ObserverSetMappingDTO>>(0));

         sut.EditSimulation(_simulation, CreationMode.New);
      }
   }

   public class When_the_simulation_observer_configuration_presenter_is_editing_a_simulation : concern_for_SimulationObserversConfigurationPresenter
   {
      [Observation]
      public void should_update_the_view_with_all_observer_set_used_in_the_simulation()
      {
         _allObserverSetMappingDTO.ShouldContain(_observerSetMappingDTO);
      }
   }

   public class When_retrieving_all_observer_set_that_are_availalbe_for_mapping : concern_for_SimulationObserversConfigurationPresenter
   {
      [Observation]
      public void should_return_all_existing_template_observer_set_that_are_not_mapped_already()
      {
         sut.AllUnmappedObserverSets().ShouldOnlyContain(_observerSetTemplate2);
      }
   }

   public class When_retrieving_all_observer_set_that_are_availalbe_for_mapping_for_a_specific_observer_set_mapping : concern_for_SimulationObserversConfigurationPresenter
   {
      [Observation]
      public void should_return_all_existing_template_observer_set_that_are_not_mapped_already_augmented_of_the_current_selection()
      {
         sut.AllUnmappedObserverSets(_observerSetMappingDTO).ShouldOnlyContain(_observerSetTemplate1, _observerSetTemplate2);
      }
   }

   public class When_the_user_adds_a_new_observer_set_mapping_to_the_simulation_configuration : concern_for_SimulationObserversConfigurationPresenter
   {
      private ObserverSetMappingDTO _newObserverSetMappingDTO;
      private bool _statusChangedNotified;

      protected override void Context()
      {
         base.Context();

         var newObserverSetMapping = new ObserverSetMapping();
         A.CallTo(() => _observerSetTask.CreateObserverSetMapping(_observerSetTemplate2)).Returns(newObserverSetMapping);
         _newObserverSetMappingDTO = new ObserverSetMappingDTO(newObserverSetMapping);
         sut.StatusChanged += (o, e) => { _statusChangedNotified = true; };
         A.CallTo(() => _observerSetDTOMapper.MapFrom(newObserverSetMapping, _simulation)).Returns(_newObserverSetMappingDTO);
      }

      protected override void Because()
      {
         sut.AddObserverSet();
      }

      [Observation]
      public void should_create_a_new_mapping_using_the_first_unused_obserer_set_templates_and_add_it_to_the_view()
      {
         _allObserverSetMappingDTO.ShouldOnlyContain(_observerSetMappingDTO, _newObserverSetMappingDTO);
      }

      [Observation]
      public void should_notify_a_status_change_event()
      {
         _statusChangedNotified.ShouldBeTrue();
      }
   }

   public class When_checking_if_the_simulaiton_observer_configuration_presenter_can_close : concern_for_SimulationObserversConfigurationPresenter
   {
      private ObserverSetMappingDTO _newObserverSetMappingDTO;

      protected override void Context()
      {
         base.Context();

         var newObserverSetMapping = new ObserverSetMapping();
         A.CallTo(() => _observerSetTask.CreateObserverSetMapping(_observerSetTemplate1)).Returns(newObserverSetMapping);
         _newObserverSetMappingDTO = new ObserverSetMappingDTO(newObserverSetMapping);
         A.CallTo(() => _observerSetDTOMapper.MapFrom(newObserverSetMapping, _simulation)).Returns(_newObserverSetMappingDTO);
      }

      [Observation]
      public void should_return_false_if_the_view_has_error()
      {
         A.CallTo(() => _view.HasError).Returns(true);
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_same_observer_set_by_name_is_used_more_than_once()
      {
         sut.AddObserverSet();
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_otherwise()
      {
         sut.CanClose.ShouldBeTrue();
      }
   }

   public class When_removing_an_observer_set_mapping_from_the_configuration : concern_for_SimulationObserversConfigurationPresenter
   {
      private bool _statusChangedNotified;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o, e) => { _statusChangedNotified = true; };
      }

      protected override void Because()
      {
         sut.RemoveObserverSetMapping(_observerSetMappingDTO);
      }

      [Observation]
      public void should_remove_the_obserer_set_from_the_view()
      {
         _allObserverSetMappingDTO.ShouldBeEmpty();
      }

      [Observation]
      public void should_notify_a_status_change_event()
      {
         _statusChangedNotified.ShouldBeTrue();
      }
   }

   public class When_loading_an_observer_set_from_template_for_an_existing_mapping_and_the_user_cancels_the_action : concern_for_SimulationObserversConfigurationPresenter
   {
      private bool _statusChangedNotified;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _observerSetTask.LoadSingleFromTemplateAsync()).Returns<Task<ObserverSet>>(null);
         sut.StatusChanged += (o, e) => { _statusChangedNotified = true; };
      }

      protected override void Because()
      {
         sut.LoadObserverSetFor(_observerSetMappingDTO);
      }

      [Observation]
      public void should_not_update_the_mapping()
      {
         _observerSetMappingDTO.ObserverSet.ShouldBeEqualTo(_observerSetTemplate1);
      }

      [Observation]
      public void should_not_notify_a_status_change_event()
      {
         _statusChangedNotified.ShouldBeFalse();
      }
   }

   public class When_loading_an_observer_set_from_template_for_an_existing_mapping_and_the_user_selects_a_valid_observer_set : concern_for_SimulationObserversConfigurationPresenter
   {
      private bool _statusChangedNotified;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _observerSetTask.LoadSingleFromTemplateAsync()).Returns(_observerSetTemplate2);
         sut.StatusChanged += (o, e) => { _statusChangedNotified = true; };
      }

      protected override void Because()
      {
         sut.LoadObserverSetFor(_observerSetMappingDTO);
      }

      [Observation]
      public void should_update_the_mapping()
      {
         _observerSetMappingDTO.ObserverSet.ShouldBeEqualTo(_observerSetTemplate2);
      }

      [Observation]
      public void should_notify_a_status_change_event()
      {
         _statusChangedNotified.ShouldBeTrue();
      }
   }

   public class When_creating_an_observer_set_for_an_existing_mapping : concern_for_SimulationObserversConfigurationPresenter
   {
      private bool _statusChangedNotified;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _observerSetTask.AddToProject()).Returns(_observerSetTemplate2);
         sut.StatusChanged += (o, e) => { _statusChangedNotified = true; };
      }

      protected override void Because()
      {
         sut.CreateObserverFor(_observerSetMappingDTO);
      }

      [Observation]
      public void should_update_the_mapping()
      {
         _observerSetMappingDTO.ObserverSet.ShouldBeEqualTo(_observerSetTemplate2);
      }

      [Observation]
      public void should_notify_a_status_change_event()
      {
         _statusChangedNotified.ShouldBeTrue();
      }
   }

   public class When_the_observer_set_configuration_presenter_is_saving_the_current_configuration : concern_for_SimulationObserversConfigurationPresenter
   {
      protected override void Context()
      {
         base.Context();
         //simulate updating existing mapping
         _observerSetMappingDTO.ObserverSet = _observerSetTemplate2;
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_add_one_mapping_for_each_mapping_defined_by_the_user()
      {
         _observerSetProperties.ObserverSetMappings.Select(x=>x.TemplateObserverSetId).ShouldOnlyContain(TEMPLATE_OBSERVER_SET_ID_2);
      }

      [Observation]
      public void should_synchronize_the_used_observer_set_building_block_in_the_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(
            _simulation, 
            A<IEnumerable<ObserverSet>>.That.Contains(_observerSetTemplate2), PKSimBuildingBlockType.ObserverSet)).MustHaveHappened();
      }
   }
}