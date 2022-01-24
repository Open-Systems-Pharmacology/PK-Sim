using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundProtocolPresenter : ContextSpecification<ISimulationCompoundProtocolPresenter>
   {
      protected ISimulationCompoundProtocolView _view;
      protected ILazyLoadTask _lazyLoadTask;
      protected ISimulationCompoundProtocolFormulationPresenter _simulationCompoundProtocolFormulationPresenter;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundProtocolView>();
         _simulationCompoundProtocolFormulationPresenter = A.Fake<ISimulationCompoundProtocolFormulationPresenter>();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();

         sut = new SimulationCompoundProtocolPresenter(_view, _simulationCompoundProtocolFormulationPresenter, _lazyLoadTask, _buildingBlockInProjectManager);
      }
   }

   public class When_the_simulation_application_presenter_is_being_created : concern_for_SimulationCompoundProtocolPresenter
   {
      [Observation]
      public void should_add_the_formulation_mapping_view_to_its_view()
      {
         A.CallTo(() => _view.AddFormulationMappingView(_simulationCompoundProtocolFormulationPresenter.View)).MustHaveHappened();
      }
   }

   public class When_the_simulation_application_presenter_is_editing_the_configuration_for_a_simulation : concern_for_SimulationCompoundProtocolPresenter
   {
      private Simulation _simulation;
      private Protocol _simulationProtocol;
      private Compound _compound;
      private Protocol _templateProtocol;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _simulationProtocol = A.Fake<Protocol>();
         _compound = A.Fake<Compound>();
         _templateProtocol = A.Fake<Protocol>();
         var compoundProperties = new CompoundProperties {ProtocolProperties = {Protocol = _simulationProtocol}};
         A.CallTo(() => _simulation.CompoundPropertiesFor(_compound)).Returns(compoundProperties);
         A.CallTo(() => _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy(_simulation,  _simulationProtocol)).Returns(_templateProtocol);
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, _compound);
      }

      [Observation]
      public void should_update_the_view_with_the_protocol_used_in_the_simulation()
      {
         A.CallTo(() => _view.BindTo(A<ProtocolSelectionDTO>.That.Matches(dto => Equals(dto.BuildingBlock, _templateProtocol)))).MustHaveHappened();
      }
   }

   public class When_the_user_select_the_protocol_that_should_be_used_in_the_simulation : concern_for_SimulationCompoundProtocolPresenter
   {
      private Simulation _simulation;
      private Protocol _selectedProtocol;
      private Compound _compound;
      private ProtocolProperties _protocolProperties;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _selectedProtocol = A.Fake<Protocol>();
         _compound = A.Fake<Compound>();
         _protocolProperties = A.Fake<ProtocolProperties>();
         var compoundProperties = new CompoundProperties {ProtocolProperties = _protocolProperties};
         A.CallTo(() => _simulation.CompoundPropertiesFor(_compound)).Returns(compoundProperties);
         sut.EditSimulation(_simulation, _compound);
      }

      protected override void Because()
      {
         sut.ProtocolSelectionChanged(_selectedProtocol);
      }

      [Observation]
      public void should_load_the_protocol()
      {
         A.CallTo(() => _lazyLoadTask.Load(_selectedProtocol)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_selected_protocol_in_the_protocol_properties()
      {
         _protocolProperties.Protocol.ShouldBeEqualTo(_selectedProtocol);
      }

      [Observation]
      public void should_update_the_formulation_mapping_for_the_selected_protocol()
      {
         A.CallTo(() => _simulationCompoundProtocolFormulationPresenter.EditSimulation(_simulation, _compound)).MustHaveHappened();
      }
   }

   public class When_the_simulation_application_presenter_is_asked_to_create_the_application_for_the_simulation : concern_for_SimulationCompoundProtocolPresenter
   {
      private Simulation _simulation;
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _compound = A.Fake<Compound>();
         sut.EditSimulation(_simulation, _compound);
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_update_the_protocol_in_the_simulation_with_the_one_selected()
      {
         A.CallTo(() => _simulationCompoundProtocolFormulationPresenter.SaveConfiguration()).MustHaveHappened();
      }
   }
}