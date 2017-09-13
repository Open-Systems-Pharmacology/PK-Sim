using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditRandomPopulationPresenter : ContextSpecification<IEditRandomPopulationPresenter>
   {
      private IWorkspace _workspace;
      protected IEditRandomPopulationView _view;
      private ISubPresenterItemManager<IPopulationItemPresenter> _subPresenterManager;
      protected IRandomPopulationSettingsPresenter _popSettingsPresenter;
      protected IPopulationAdvancedParametersPresenter _popAdvancedParameterPresenter;
      protected IPopulationAdvancedParameterDistributionPresenter _popDistributionPresenter;
      protected RandomPopulation _randomPopulation;

      protected override void Context()
      {
         _workspace = A.Fake<IWorkspace>();
         _view = A.Fake<IEditRandomPopulationView>();
         _subPresenterManager = SubPresenterHelper.Create<IPopulationItemPresenter>();
         _popSettingsPresenter = _subPresenterManager.CreateFake(RamdomPopulationItems.Settings);
         _popAdvancedParameterPresenter = _subPresenterManager.CreateFake(RamdomPopulationItems.AdvancedParameters);
         _popDistributionPresenter = _subPresenterManager.CreateFake(RamdomPopulationItems.ParameterDistribution);
         //This line is required because of some generic issues
         A.CallTo(_subPresenterManager).WithReturnType<IPopulationSettingsPresenter<RandomPopulation>>().Returns(_popSettingsPresenter);
         _randomPopulation = A.Fake<RandomPopulation>();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
         sut = new EditRandomPopulationPresenter(_view, _subPresenterManager);
         sut.InitializeWith(_workspace);
      }
   }

   public class When_editing_a_random_population : concern_for_EditRandomPopulationPresenter
   {
      protected override void Because()
      {
         sut.Edit(_randomPopulation);
      }

      [Observation]
      public void should_load_the_population_settings_and_enable_the_setting_tab()
      {
         A.CallTo(() => _popSettingsPresenter.LoadPopulation(_randomPopulation)).MustHaveHappened();
         A.CallTo(() => _view.SetControlEnabled(RamdomPopulationItems.Settings, true)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_parameter_distributions_and_activate_the_distribution_tab()
      {
         A.CallTo(() => _popDistributionPresenter.EditPopulation(_randomPopulation)).MustHaveHappened();
         A.CallTo(() => _view.ActivateControl(RamdomPopulationItems.ParameterDistribution)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_advanced_parameters()
      {
         A.CallTo(() => _popAdvancedParameterPresenter.EditPopulation(_randomPopulation)).MustHaveHappened();
         A.CallTo(() => _view.SetControlEnabled(RamdomPopulationItems.AdvancedParameters, true)).MustHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_has_changed_for_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = new AdvancedParameter();
         sut.Edit(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AdvancedParameterDistributionChangedEvent(_randomPopulation, _advancedParameter));
      }

      [Observation]
      public void should_refresh_the_distribution_tab()
      {
         A.CallTo(() => _popDistributionPresenter.Select(_advancedParameter)).MustHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_was_selected_for_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = new AdvancedParameter();
         sut.Edit(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AdvancedParameteSelectedEvent(_randomPopulation, _advancedParameter));
      }

      [Observation]
      public void should_refresh_the_distribution_tab()
      {
         A.CallTo(() => _popDistributionPresenter.Select(_advancedParameter)).MustHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parmaeter_has_changed_for_a_population_that_is_not_the_one_being_edited : concern_for_EditRandomPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AdvancedParameterDistributionChangedEvent(A.Fake<RandomPopulation>(), new AdvancedParameter()));
      }

      [Observation]
      public void should_not_change_the_distribution_presenter()
      {
         A.CallTo(() => _popDistributionPresenter.Select(A<AdvancedParameter>._)).MustNotHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parmaeter_was_selected_for_a_population_that_is_not_the_one_being_edited : concern_for_EditRandomPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AdvancedParameteSelectedEvent(A.Fake<RandomPopulation>(), new AdvancedParameter()));
      }

      [Observation]
      public void should_not_change_the_distribution_presenter()
      {
         A.CallTo(() => _popDistributionPresenter.Select(A<AdvancedParameter>._)).MustNotHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_has_been_added_to_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = new AdvancedParameter();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AddAdvancedParameterToContainerEvent {Container = _randomPopulation, Entity = _advancedParameter});
      }

      [Observation]
      public void should_add_the_parameter_to_the_distribution_diagram()
      {
         A.CallTo(() => _popDistributionPresenter.AddAdvancedParameter(_advancedParameter)).MustHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_container_has_been_added_to_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AddAdvancedParameterContainerToPopulationEvent(_randomPopulation));
      }

      [Observation]
      public void should_refresh_the_distribution_presenters()
      {
         A.CallTo(() => _popDistributionPresenter.EditPopulation(_randomPopulation)).MustHaveHappened();
         A.CallTo(() => _popAdvancedParameterPresenter.EditPopulation(_randomPopulation)).MustHaveHappened();
      }
   }


   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_container_has_been_removed_from_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveAdvancedParameterContainerFromPopulationEvent(_randomPopulation));
      }

      [Observation]
      public void should_refresh_the_distribution_presenters()
      {
         A.CallTo(() => _popDistributionPresenter.EditPopulation(_randomPopulation)).MustHaveHappened();
         A.CallTo(() => _popAdvancedParameterPresenter.EditPopulation(_randomPopulation)).MustHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_has_been_added_to_a_population_that_is_not_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = new AdvancedParameter();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new AddAdvancedParameterToContainerEvent {Container = A.Fake<RandomPopulation>(), Entity = _advancedParameter});
      }

      [Observation]
      public void should_not_add_the_parameter_to_the_distribution_diagram()
      {
         A.CallTo(() => _popDistributionPresenter.AddAdvancedParameter(A<AdvancedParameter>._)).MustNotHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_has_been_deleted_from_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = new AdvancedParameter();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveAdvancedParameterFromContainerEvent {Container = _randomPopulation, Entity = _advancedParameter});
      }

      [Observation]
      public void should_remove_the_parameter_from_the_distribution_diagram()
      {
         A.CallTo(() => _popDistributionPresenter.RemoveAdvancedParameter(_advancedParameter)).MustHaveHappened();
      }
   }

   public class When_the_edit_random_population_presenter_is_being_notified_that_an_advanced_parameter_has_been_deleted_from_a_population_that_is_not_the_edited_population : concern_for_EditRandomPopulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveAdvancedParameterFromContainerEvent {Container = A.Fake<RandomPopulation>()});
      }

      [Observation]
      public void should_not_remove_the_parameter_from_the_distribution_diagram()
      {
         A.CallTo(() => _popDistributionPresenter.RemoveAdvancedParameter(A<AdvancedParameter>._)).MustNotHaveHappened();
      }
   }
}