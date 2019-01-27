using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditIndividualSimulationPresenter : ContextSpecification<IEditIndividualSimulationPresenter>
   {
      protected IEditIndividualSimulationView _view;
      protected ISimulationAnalysisPresenterFactory _simulationAnalysisPresenterFactory;
      protected IRepository<ISimulationItemPresenter> _repository;
      protected ISubPresenterItemManager<IEditIndividualSimulationItemPresenter> _subPresenterManager;
      private ISimulationAnalysisPresenterContextMenuFactory _contextMenuFactory;
      protected ISimulationAnalysis _analysis;
      protected ISimulationTimeProfileChartPresenter _chartPresenter;
      protected IndividualSimulation _simulation;
      protected IPresentationSettingsTask _presenterSettingsTask;
      protected ISimulationAnalysisCreator _simulationAnalysisCreator;

      protected override void Context()
      {
         _view = A.Fake<IEditIndividualSimulationView>();
         _repository = A.Fake<IRepository<ISimulationItemPresenter>>();
         _simulationAnalysisPresenterFactory = A.Fake<ISimulationAnalysisPresenterFactory>();
         _subPresenterManager = A.Fake<ISubPresenterItemManager<IEditIndividualSimulationItemPresenter>>();
         _contextMenuFactory = A.Fake<ISimulationAnalysisPresenterContextMenuFactory>();
         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         _simulationAnalysisCreator = A.Fake<ISimulationAnalysisCreator>();
         _analysis = A.Fake<ISimulationAnalysis>();
         _chartPresenter = A.Fake<ISimulationTimeProfileChartPresenter>();
         _simulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulationAnalysisPresenterFactory.PresenterFor(_analysis)).Returns(_chartPresenter);
         A.CallTo(() => _simulation.Analyses).Returns(new List<ISimulationAnalysis> {_analysis});
         A.CallTo(() => _subPresenterManager.AllSubPresenters).Returns(new IEditIndividualSimulationItemPresenter[] {});

         sut = new EditIndividualSimulationPresenter(_view, _subPresenterManager, _simulationAnalysisPresenterFactory,
            _contextMenuFactory, _presenterSettingsTask, _simulationAnalysisCreator);
      }
   }

   public class When_the_edit_simulation_presenter_is_told_to_close : concern_for_EditIndividualSimulationPresenter
   {
      protected override void Because()
      {
         sut.Close();
      }

      [Observation]
      public void should_tell_its_view_to_close()
      {
         A.CallTo(() => _view.CloseView()).MustHaveHappened();
      }
   }

   public class When_the_user_wants_to_close_a_simulation_screen : concern_for_EditIndividualSimulationPresenter
   {
      private bool _eventWasRaised;

      protected override void Context()
      {
         base.Context();
         sut.Closing += (o, e) => { _eventWasRaised = true; };
      }

      protected override void Because()
      {
         sut.OnFormClosed();
      }

      [Observation]
      public void should_notify_that_the_screen_is_about_to_close()
      {
         _eventWasRaised.ShouldBeTrue();
      }
   }

   public class When_the_individual_simulation_presenter_is_being_notified_that_a_plot_was_created_for_the_simulation_being_edited : concern_for_EditIndividualSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_simulation);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationAnalysisCreatedEvent(_simulation, _analysis));
      }

      [Observation]
      public void should_retrieve_a_simulation_plot_presenter_for_the_given_plot()
      {
         A.CallTo(() => _simulationAnalysisPresenterFactory.PresenterFor(_analysis)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_plot_view_to_the_edit_simulation_view()
      {
         A.CallTo(() => _view.AddAnalysis(_chartPresenter)).MustHaveHappened();
      }

      [Observation]
      public void should_initialize_the_settings_in_the_created_presenter_for_the_given_analysis()
      {
         A.CallTo(() => _chartPresenter.LoadSettingsForSubject(_analysis)).MustHaveHappened();
      }
   }

   public class When_the_individual_simulation_presenter_is_being_notified_that_a_plot_was_created_for_a_simulation_which_is_not_the_one_edited : concern_for_EditIndividualSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulation.Analyses).Returns(new List<ISimulationAnalysis>());
         sut.Edit(_simulation);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationAnalysisCreatedEvent(A.Fake<Simulation>(), _analysis));
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _simulationAnalysisPresenterFactory.PresenterFor(_analysis)).MustNotHaveHappened();
      }
   }

   public class When_the_individual_simulation_presenter_is_told_to_edit_the_same_simulation : concern_for_EditIndividualSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_simulation);
      }

      protected override void Because()
      {
         sut.Edit(_simulation);
      }

      [Observation]
      public void should_just_reactivate_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappenedTwiceExactly();
      }

      [Observation]
      public void should_not_reinitalize_each_sub_presenter()
      {
         A.CallTo(() => _simulationAnalysisPresenterFactory.PresenterFor(_analysis)).MustHaveHappened();
      }
   }

   public class When_the_individual_simulation_presenter_is_being_notified_that_the_edited_simualion_was_calculated : concern_for_EditIndividualSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_simulation);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationResultsUpdatedEvent(_simulation));
      }

      [Observation]
      public void should_refresh_all_available_plots()
      {
         A.CallTo(() => _chartPresenter.UpdateAnalysisBasedOn(_simulation)).MustHaveHappened();
      }
   }

   public class When_the_individual_simulation_presenter_is_being_notified_that_a_simulation_that_is_not_the_one_edited_was_calculated : concern_for_EditIndividualSimulationPresenter
   {
      private IndividualSimulation _anotherSimulation;
      private DataRepository _results;

      protected override void Context()
      {
         base.Context();
         sut.Edit(_simulation);
         _anotherSimulation = A.Fake<IndividualSimulation>();
         _results = A.Fake<DataRepository>();
         A.CallTo(() => _anotherSimulation.DataRepository).Returns(_results);
      }

      protected override void Because()
      {
         sut.Handle(new SimulationResultsUpdatedEvent(_anotherSimulation));
      }

      [Observation]
      public void should_not_replot_the_charts()
      {
         A.CallTo(() => _chartPresenter.UpdateAnalysisBasedOn(_simulation)).MustNotHaveHappened();
      }
   }

   public class When_the_individual_simulation_presenter_is_loading_the_presenter_settings_for_a_given_simulation : concern_for_EditIndividualSimulationPresenter
   {
      private TabbedPresenterSettings _presenterSettings;

      protected override void Context()
      {
         base.Context();
         _presenterSettings = new TabbedPresenterSettings {SelectedTabIndex = 5};
         A.CallTo(() => _presenterSettingsTask.PresentationSettingsFor<TabbedPresenterSettings>(sut, _simulation)).Returns(_presenterSettings);
      }

      protected override void Because()
      {
         sut.LoadSettingsForSubject(_simulation);
      }

      [Observation]
      public void should_retrieve_the_index_of_the_selected_tab_and_set_it_in_the_view()
      {
         A.CallTo(() => _view.SelectTabByIndex(_presenterSettings.SelectedTabIndex)).MustHaveHappened();
      }
   }

   public class When_notify_that_the_selected_tab_has_changed : concern_for_EditIndividualSimulationPresenter
   {
      private TabbedPresenterSettings _presenterSettings;

      protected override void Context()
      {
         base.Context();
         _presenterSettings = new TabbedPresenterSettings();
         A.CallTo(_presenterSettingsTask).WithReturnType<TabbedPresenterSettings>().Returns(_presenterSettings);
         sut.LoadSettingsForSubject(_simulation);
      }

      protected override void Because()
      {
         sut.SetSelectedTabIndex(4);
      }

      [Observation]
      public void should_update_the_selected_tab_index_in_the_underlying_settings()
      {
         _presenterSettings.SelectedTabIndex.ShouldBeEqualTo(4);
      }
   }

   public class When_cloning_a_given_simulation_analysis : concern_for_EditIndividualSimulationPresenter
   {
      private ISimulationAnalysis _cloneAnalysis;
      private ISimulationAnalysis _sourceAnalysis;

      protected override void Context()
      {
         base.Context();
         _cloneAnalysis = A.Fake<ISimulationAnalysis>();
         _sourceAnalysis = A.Fake<ISimulationAnalysis>();
         A.CallTo(() => _simulationAnalysisCreator.CreateAnalysisBasedOn(_sourceAnalysis)).Returns(_cloneAnalysis);
         sut.Edit(_simulation);
      }

      protected override void Because()
      {
         sut.CloneAnalysis(_sourceAnalysis);
      }

      [Observation]
      public void should_create_a_new_analysis_based_on_the_original_one_and_add_it_to_the_simulation()
      {
         A.CallTo(() => _simulationAnalysisCreator.AddSimulationAnalysisTo(_simulation, _cloneAnalysis)).MustHaveHappened();
      }
   }
}