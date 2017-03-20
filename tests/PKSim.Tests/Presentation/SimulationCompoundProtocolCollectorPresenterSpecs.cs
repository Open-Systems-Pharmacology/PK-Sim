using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundProtocolCollectorPresenter : ContextSpecification<ISimulationCompoundProtocolCollectorPresenter>
   {
      protected ISimulationCompoundProtocolCollectorView _view;
      protected IApplicationController _applicationController;
      protected IConfigurableLayoutPresenter _configurableLayoutPresenter;
      protected IEventPublisher _eventPublisher;
      protected IProtocolChartPresenter _protocolChartPresenter;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected Simulation _simulation;
      protected Compound _compound1;
      private Compound _compound2;
      protected ISimulationCompoundProtocolPresenter _subPresenter1;
      protected ISimulationCompoundProtocolPresenter _subPresenter2;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundProtocolCollectorView>();
         _applicationController = A.Fake<IApplicationController>();
         _configurableLayoutPresenter = A.Fake<IConfigurableLayoutPresenter>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _protocolChartPresenter = A.Fake<IProtocolChartPresenter>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();

         sut = new SimulationCompoundProtocolCollectorPresenter(_view, _applicationController, _configurableLayoutPresenter,
            _eventPublisher, _protocolChartPresenter, _simulationBuildingBlockUpdater);

         _simulation = A.Fake<Simulation>();
         _compound1 = A.Fake<Compound>();
         _compound2 = A.Fake<Compound>();

         _subPresenter1 = A.Fake<ISimulationCompoundProtocolPresenter>();
         A.CallTo(() => _subPresenter1.SelectedProtocol).Returns(new SimpleProtocol().WithName("P1"));
         _subPresenter2 = A.Fake<ISimulationCompoundProtocolPresenter>();
         A.CallTo(() => _subPresenter2.SelectedProtocol).Returns(new SimpleProtocol().WithName("P2"));

         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound1, _compound2});
         A.CallTo(() => _applicationController.Start<ISimulationCompoundProtocolPresenter>()).ReturnsNextFromSequence(_subPresenter1, _subPresenter2);
      }
   }

   public class When_editing_the_protocols_defined_for_a_given_simulation : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      private ICache<Compound, Protocol> _allProtocols;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _protocolChartPresenter.PlotProtocols((A<ICache<Compound,Protocol>>._)))
            .Invokes(x => _allProtocols = x.GetArgument<ICache<Compound, Protocol>>(0));
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_display_the_chart_corresponding_to_all_protocols_selected()
      {
         _allProtocols.ShouldOnlyContain(_subPresenter1.SelectedProtocol, _subPresenter2.SelectedProtocol);
      }
   }

   public class When_checking_if_the_compound_protocol_selection_presenter_can_be_closed : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.HasError).Returns(false);
         A.CallTo(() => _protocolChartPresenter.CanClose).Returns(true);
         A.CallTo(() => _subPresenter1.CanClose).Returns(true);
         A.CallTo(() => _subPresenter2.CanClose).Returns(true);
      }

      [Observation]
      public void should_return_false_if_no_protocol_was_selected()
      {
         A.CallTo(() => _subPresenter1.SelectedProtocol).Returns(null);
         A.CallTo(() => _subPresenter2.SelectedProtocol).Returns(null);

         sut.EditSimulation(_simulation, CreationMode.New);
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_two_selected_protocols_are_the_same()
      {
         A.CallTo(() => _subPresenter2.SelectedProtocol).Returns(_subPresenter1.SelectedProtocol);
         sut.EditSimulation(_simulation, CreationMode.New);
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_otherwise()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
         sut.CanClose.ShouldBeTrue();
      }
   }

   public class When_editing_compound_protocols_defined_in_the_simulation : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      [Observation]
      public void should_show_a_warning_if_no_protocol_is_available()
      {
         A.CallTo(() => _subPresenter1.SelectedProtocol).Returns(null);
         A.CallTo(() => _subPresenter2.SelectedProtocol).Returns(null);
         sut.EditSimulation(_simulation, CreationMode.New);
         _view.WarningVisible.ShouldBeTrue();
         _view.Warning.ShouldBeEqualTo(PKSimConstants.Error.AtLeastOneProtocolRequiredToCreateSimulation);
      }

      [Observation]
      public void should_show_a_warning_if_two_protocols_are_the_same()
      {
         A.CallTo(() => _subPresenter2.SelectedProtocol).Returns(_subPresenter1.SelectedProtocol);
         sut.EditSimulation(_simulation, CreationMode.New);
         _view.WarningVisible.ShouldBeTrue();
         _view.Warning.ShouldBeEqualTo(PKSimConstants.Error.AProtocolCanOnlyBeUsedOnceInASimulation);
      }

      [Observation]
      public void should_hide_warnings_otherwise()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_editing_a_simulation_with_only_one_compound : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound1});
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_not_allow_empty_protocol_selection()
      {
         _subPresenter1.AllowEmptyProtocolSelection.ShouldBeFalse();
      }
   }

   public class When_editing_a_simulation_with_more_than_one_compound : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_allow_empty_protocol_selection()
      {
         _subPresenter1.AllowEmptyProtocolSelection.ShouldBeTrue();
         _subPresenter2.AllowEmptyProtocolSelection.ShouldBeTrue();
      }
   }

   public class When_updating_the_selected_protocol_with_a_template_protocol_that_is_not_being_used : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      private Protocol _templateProtocol;

      protected override void Context()
      {
         base.Context();
         _templateProtocol = new SimpleProtocol().WithName("NOT USED");
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      protected override void Because()
      {
         sut.UpdateSelectedProtocol(_templateProtocol);
      }

      [Observation]
      public void should_not_update_the_selected_protocols()
      {
         A.CallTo(() => _subPresenter1.ProtocolSelectionChanged(_templateProtocol)).MustNotHaveHappened();
         A.CallTo(() => _subPresenter2.ProtocolSelectionChanged(_templateProtocol)).MustNotHaveHappened();
      }
   }

   public class When_updating_the_selected_protocol_with_a_template_protocol_that_is_being_used : concern_for_SimulationCompoundProtocolCollectorPresenter
   {
      private Protocol _templateProtocol;
      protected override void Context()
      {
         base.Context();
         _templateProtocol = new SimpleProtocol().WithName(_subPresenter1.SelectedProtocol.Name);
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      protected override void Because()
      {
         sut.UpdateSelectedProtocol(_templateProtocol);
      }

      [Observation]
      public void should_update_the_selected_protocol_in_the_approriate_presenter()
      {
         A.CallTo(() => _subPresenter1.ProtocolSelectionChanged(_templateProtocol)).MustHaveHappened();
      }
   }
}