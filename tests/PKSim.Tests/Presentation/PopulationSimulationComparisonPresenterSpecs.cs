using System.Drawing;
using OSPSuite.BDDHelper;
using OSPSuite.Assets;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationSimulationComparisonPresenter : ContextSpecification<IPopulationSimulationComparisonPresenter>
   {
      protected IPopulationSimulationComparisonView _view;
      private ISubPresenterItemManager<IPopulationSimulationComparisonItemPresenter> _subPresenterManager;
      protected ISimulationAnalysisPresenterFactory _analysisPresenterFactory;
      protected PopulationSimulationComparison _populationSimulationComparison;
      private ISimulationAnalysis _analysis;
      protected ISimulationAnalysisPresenter _simulationAnalysisPresenter;
      protected ISimulationAnalysisPresenterContextMenuFactory _contextMenuFactory;
      private IPresentationSettingsTask _presenterSettingsTask;
      private ISimulationAnalysisCreator _simulationAnalysisCreator;

      protected override void Context()
      {
         _view = A.Fake<IPopulationSimulationComparisonView>();
         _subPresenterManager = A.Fake<ISubPresenterItemManager<IPopulationSimulationComparisonItemPresenter>>();
         _analysisPresenterFactory = A.Fake<ISimulationAnalysisPresenterFactory>();
         _populationSimulationComparison = A.Fake<PopulationSimulationComparison>();
         _contextMenuFactory = A.Fake<ISimulationAnalysisPresenterContextMenuFactory>();
         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         _simulationAnalysisCreator = A.Fake<ISimulationAnalysisCreator>();
         sut = new PopulationSimulationComparisonPresenter(_view, _subPresenterManager, _analysisPresenterFactory,  _contextMenuFactory, _presenterSettingsTask, _simulationAnalysisCreator);

         _analysis = A.Fake<ISimulationAnalysis>();
         _simulationAnalysisPresenter = A.Fake<ISimulationAnalysisPresenter>();

         A.CallTo(() => _populationSimulationComparison.Analyses).Returns(new[] {_analysis});
         A.CallTo(() => _analysisPresenterFactory.PresenterFor(_analysis)).Returns(_simulationAnalysisPresenter);
         sut.Edit(_populationSimulationComparison);
      }
   }

   public class When_notify_that_a_simulation_status_was_changed_and_the_simulation_is_used_in_the_comparison : concern_for_PopulationSimulationComparisonPresenter
   {
      private PopulationSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _populationSimulationComparison.HasSimulation(_simulation)).Returns(true);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationStatusChangedEvent(_simulation));
      }

      [Observation]
      public void should_update_the_anlaysis_status_in_all_sub_presenters()
      {
         //once at lease in edit. A second time for event
         A.CallTo(() => _view.UpdateTrafficLightFor(_simulationAnalysisPresenter, A<ApplicationIcon>._)).MustHaveHappenedTwiceOrMore();
      }
   }

   public class When_notify_that_a_simulation_status_was_changed_and_the_simulation_is_not_used_in_the_comparison : concern_for_PopulationSimulationComparisonPresenter
   {
      private PopulationSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _populationSimulationComparison.HasSimulation(_simulation)).Returns(false);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationStatusChangedEvent(_simulation));
      }

      [Observation]
      public void should_not_update_the_anlaysis_status_in_all_sub_presenters()
      {
         //once at lease in edit. No second time for even
         A.CallTo(() => _view.UpdateTrafficLightFor(_simulationAnalysisPresenter, A<ApplicationIcon>._)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_showing_a_popup_menu_for_a_simulation_analysis_presenter_that_is_not_defined : concern_for_PopulationSimulationComparisonPresenter
   {
      protected override void Because()
      {
         sut.ShowContextMenu(null, new Point());
      }

      [Observation]
      public void should_not_create_a_context_menu()
      {
         A.CallTo(_contextMenuFactory).WithReturnType<IContextMenu>().MustNotHaveHappened();
      }
   }
}