using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationApplicationFormulationPresenter : ContextSpecification<ISimulationCompoundProtocolFormulationPresenter>
   {
      protected ISimulationCompoundProtocolFormulationView _view;
      protected IFormulationTask _formulationTask;
      private IFormulationMappingDTOToFormulationMappingMapper _formulationMappingMapper;
      protected IFormulationFromMappingRetriever _formulationFromMappingRetriever;

      protected Protocol _protocol;
      protected Simulation _simulation;
      protected ProtocolProperties _protocolProperties;
      protected Formulation _formulation1;
      protected Formulation _formulation2;
      protected Compound _compound;
      private IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundProtocolFormulationView>();
         _formulationTask = A.Fake<IFormulationTask>();
         _formulationMappingMapper = A.Fake<IFormulationMappingDTOToFormulationMappingMapper>();
         _formulationFromMappingRetriever = A.Fake<IFormulationFromMappingRetriever>();
         _formulation1 = new Formulation();
         _formulation2 = new Formulation();
         _protocol = A.Fake<Protocol>();
         _compound = A.Fake<Compound>();
         _simulation = A.Fake<Simulation>();
         _protocolProperties = A.Fake<ProtocolProperties>();
         var compoundProperties = new CompoundProperties();
         A.CallTo(() => _simulation.CompoundPropertiesFor(_compound)).Returns(compoundProperties);
         compoundProperties.ProtocolProperties = _protocolProperties;
         _protocolProperties.Protocol = _protocol;
         A.CallTo(() => _formulationTask.All()).Returns(new[] {_formulation1, _formulation2});
         _buildingBlockSelectionDisplayer = A.Fake<IBuildingBlockSelectionDisplayer>();
         sut = new SimulationCompoundProtocolFormulationPresenter(_view, _formulationTask, _formulationMappingMapper, _formulationFromMappingRetriever, _buildingBlockSelectionDisplayer);
      }
   }

   public class When_the_application_formulation_presenter_is_editing_the_protocol_for_a_compound_in_a_simulation : concern_for_SimulationApplicationFormulationPresenter
   {
      private string _emptyFormulation;
      private string _formulationKey1;
      private string _formulationKey2;
      private IList<FormulationMappingDTO> _formMappingDtoList;

      protected override void Context()
      {
         base.Context();
         _emptyFormulation = string.Empty;
         _formulationKey1 = "Tralal";
         _formulationKey2 = "toto";
         _formulation1.AddRoute(ApplicationTypes.Intravenous.Route);
         _formulation2.AddRoute(ApplicationTypes.Oral.Route);
         var formulationMapping1 = new FormulationMapping();
         var formulationMapping2 = new FormulationMapping();
         A.CallTo(() => _protocol.UsedFormulationKeys).Returns(new[] {_emptyFormulation, _formulationKey1, _formulationKey2});
         A.CallTo(() => _protocolProperties.MappingWith(_formulationKey1)).Returns(formulationMapping1);
         A.CallTo(() => _protocolProperties.MappingWith(_formulationKey2)).Returns(formulationMapping2);
         A.CallTo(() => _protocol.ApplicationTypeUsing(_formulationKey1)).Returns(ApplicationTypes.Intravenous);
         A.CallTo(() => _protocol.ApplicationTypeUsing(_formulationKey2)).Returns(ApplicationTypes.Oral);
         A.CallTo(() => _formulationFromMappingRetriever.TemplateFormulationUsedBy(_simulation, formulationMapping1)).Returns(_formulation1);
         A.CallTo(() => _formulationFromMappingRetriever.TemplateFormulationUsedBy(_simulation, formulationMapping2)).Returns(null);
         _formulation2.AddRoute(ApplicationTypes.Oral.Route);
         A.CallTo(() => _view.BindTo(A<IEnumerable<FormulationMappingDTO>>._))
            .Invokes(x => _formMappingDtoList = x.GetArgument<IEnumerable<FormulationMappingDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_display_the_formulation_mapping_based_on_the_simulation_settings()
      {
         _formMappingDtoList.Count().ShouldBeEqualTo(2);
         _formMappingDtoList[0].ApplicationType.ShouldBeEqualTo(ApplicationTypes.Intravenous);
         _formMappingDtoList[0].Formulation.ShouldBeEqualTo(_formulation1);
         _formMappingDtoList[0].FormulationKey.ShouldBeEqualTo(_formulationKey1);

         _formMappingDtoList[1].ApplicationType.ShouldBeEqualTo(ApplicationTypes.Oral);
         _formMappingDtoList[1].Formulation.ShouldBeEqualTo(_formulation2);
         _formMappingDtoList[1].FormulationKey.ShouldBeEqualTo(_formulationKey2);
      }
   }

   public class When_the_application_formulation_presenter_is_editing_a_protocol_for_a_compound_in_a_simulation_that_is_not_defined : concern_for_SimulationApplicationFormulationPresenter
   {
      private IList<FormulationMappingDTO> _formMappingDtoList;

      protected override void Context()
      {
         base.Context();
         _protocolProperties.Protocol = null;

         A.CallTo(() => _view.BindTo(A<IEnumerable<FormulationMappingDTO>>._))
            .Invokes(x => _formMappingDtoList = x.GetArgument<IEnumerable<FormulationMappingDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_not_display_any_formulation_mapping()
      {
         _formMappingDtoList.Any().ShouldBeFalse();
      }
   }

   public class When_the_simulation_application_formulation_presenter_is_retrieving_the_list_of_all_formulation_for_a_given_route : concern_for_SimulationApplicationFormulationPresenter
   {
      private IEnumerable<FormulationSelectionDTO> _result;

      protected override void Context()
      {
         base.Context();
         _formulation1.AddRoute(ApplicationTypes.Oral.Route);
         _formulation2.AddRoute(ApplicationTypes.Intravenous.Route);
      }

      protected override void Because()
      {
         _result = sut.AllFormulationsFor(new FormulationMappingDTO {ApplicationType = ApplicationTypes.Oral});
      }

      [Observation]
      public void should_return_the_list_of_formulation_defined_for_that_route()
      {
         _result.Select(x => x.BuildingBlock).ShouldOnlyContain(_formulation1);
      }
   }

   public class When_the_simulation_application_formulation_presenter_is_retrieving_the_list_of_all_formulations_for_a_given_route_and_the_used_building_block_is_not_a_template_building_block : concern_for_SimulationApplicationFormulationPresenter
   {
      private IEnumerable<FormulationSelectionDTO> _result;
      private Formulation _nonTemplateFormulation;

      protected override void Context()
      {
         base.Context();
         _formulation1.AddRoute(ApplicationTypes.Oral.Route);
         _formulation2.AddRoute(ApplicationTypes.Intravenous.Route);
         _nonTemplateFormulation = new Formulation();
      }

      protected override void Because()
      {
         _result = sut.AllFormulationsFor(new FormulationMappingDTO { ApplicationType = ApplicationTypes.Oral, Selection = new FormulationSelectionDTO {BuildingBlock = _nonTemplateFormulation } });
      }

      [Observation]
      public void should_return_the_list_of_formulation_defined_for_that_route()
      {
         _result.Select(x => x.BuildingBlock).ShouldOnlyContain(_formulation1, _nonTemplateFormulation);
      }
   }

   public class When_the_simulation_application_formulation_presenter_is_being_notified_that_its_view_has_changed : concern_for_SimulationApplicationFormulationPresenter
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o, e) => { _eventRaised = true; };
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_notify_the_status_change_event()
      {
         _eventRaised.ShouldBeTrue();
      }
   }

   public class When_the_presenter_is_told_to_create_a_new_formulation_for_a_given_formulation_mapping_dto : concern_for_SimulationApplicationFormulationPresenter
   {
      private FormulationMappingDTO _formulationDTO;
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         _formulationDTO = new FormulationMappingDTO {ApplicationType = ApplicationTypes.Intravenous};
         A.CallTo(() => _formulationTask.CreateFormulationForRoute(ApplicationTypes.Intravenous.Route)).Returns(_formulation1);
         sut.StatusChanged += (o, e) => { _eventRaised = true; };
      }

      protected override void Because()
      {
         sut.CreateFormulationFor(_formulationDTO);
      }

      [Observation]
      public void should_leverage_the_formulation_task_to_create_a_formulation_for_the_given_mapping()
      {
         _formulationDTO.Formulation.ShouldBeEqualTo(_formulation1);
      }

      [Observation]
      public void should_refresh_the_data_in_the_view()
      {
         A.CallTo(() => _view.RefreshData()).MustHaveHappened();
      }

      [Observation]
      public void should_notify_a_status_change()
      {
         _eventRaised.ShouldBeTrue();
      }
   }
}