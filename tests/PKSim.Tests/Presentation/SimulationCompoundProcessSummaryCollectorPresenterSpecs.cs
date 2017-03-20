using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Diagrams;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationProcessSummaryCollectorPresenter : ContextSpecification<ISimulationCompoundProcessSummaryCollectorPresenter>
   {
      protected ISimulationCompoundProcessSummaryCollectorView _view;
      protected IApplicationController _applicationController;
      protected IConfigurableLayoutPresenter _layoutPresenter;
      protected IEventPublisher _eventPublisher;
      protected ISimulationCompoundInteractionSelectionPresenter _interactionSelectionPresenter;
      protected Simulation _simulation;
      protected Individual _individual;
      protected Compound _compound;
      private IReactionDiagramContainerPresenter _reactionDiagramPresenter;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundProcessSummaryCollectorView>();
         _applicationController = A.Fake<IApplicationController>();
         _layoutPresenter = A.Fake<IConfigurableLayoutPresenter>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _interactionSelectionPresenter = A.Fake<ISimulationCompoundInteractionSelectionPresenter>();
         _reactionDiagramPresenter = A.Fake<IReactionDiagramContainerPresenter>();
         sut = new SimulationCompoundProcessSummaryCollectorPresenter(
            _view, 
            _applicationController, 
            _layoutPresenter, 
            _eventPublisher, 
            _interactionSelectionPresenter,
         _reactionDiagramPresenter);

         _simulation = A.Fake<Simulation>();
         _individual = new Individual();
         _compound = new Compound();

         A.CallTo(() => _simulation.Individual).Returns(_individual);
         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound});
      }
   }

   public class When_editing_a_simulation_where_a_compound_has_no_processes : concern_for_SimulationProcessSummaryCollectorPresenter
   {
      private ISimulationCompoundProcessSummaryPresenter _subPresenter;
      private ISimulationCompoundProcessSummaryPresenter _anotherSubPresenter;

      protected override void Context()
      {
         base.Context();
         _subPresenter = A.Fake<ISimulationCompoundProcessSummaryPresenter>();
         _anotherSubPresenter = A.Fake<ISimulationCompoundProcessSummaryPresenter>();

         A.CallTo(() => _simulation.AllBuildingBlocks<Compound>()).Returns(new List<Compound> { _compound });
         A.CallTo(() => _applicationController.Start<ISimulationCompoundProcessSummaryPresenter>()).ReturnsNextFromSequence(_subPresenter, _anotherSubPresenter);
         A.CallTo(() => _subPresenter.HasProcessesDefined).Returns(true);
         A.CallTo(() => _anotherSubPresenter.HasProcessesDefined).Returns(false);
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_not_show_views_where_presenter_has_no_processes_defined()
      {
         A.CallTo(() => _layoutPresenter.AddViews(A<IEnumerable<IView>>.That.Matches( x => x.Count() == 1))).MustHaveHappened();
      }
   }

   public class When_editing_the_processes_defined_in_a_simulation : concern_for_SimulationProcessSummaryCollectorPresenter
   {
      protected override void Context()
      {
         base.Context();
         _individual.AddMolecule(new IndividualEnzyme());
         _compound.AddProcess(new InhibitionProcess());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_also_edit_the_interaction_defined_for_the_given_simulation()
      {
         A.CallTo(() => _interactionSelectionPresenter.EditSimulation(_simulation, CreationMode.New)).MustHaveHappened();
      }

      [Observation]
      public void should_not_hide_the_interaction_view()
      {
         _view.ShowInteractionView.ShouldBeTrue();
      }
   }

   public class When_editing_a_simulation_for_using_an_individual_without_molecules : concern_for_SimulationProcessSummaryCollectorPresenter
   {
      protected override void Context()
      {
         base.Context();
         _compound.AddProcess(new InhibitionProcess());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_hide_the_interaction_definition()
      {
         _view.ShowInteractionView.ShouldBeFalse();
      }
   }

   public class When_editing_a_simulation_using_compounds_without_any_interaction_processes : concern_for_SimulationProcessSummaryCollectorPresenter
   {
      protected override void Context()
      {
         base.Context();
         _individual.AddMolecule(new IndividualEnzyme());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_hide_the_interaction_definition()
      {
         _view.ShowInteractionView.ShouldBeFalse();
      }
   }
}