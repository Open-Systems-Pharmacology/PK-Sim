using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;
using PKSim.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditIndividualPresenter : ContextSpecification<IEditIndividualPresenter>
   {
      protected IEditIndividualView _view;
      protected ICoreWorkspace _workspace;
      protected IIndividualSettingsPresenter _individualSettingsPresenter;
      protected IIndividualMoleculesPresenter _individualMoleculesPresenter;
      protected Individual _individualToEdit;
      protected ISubPresenterItemManager<IIndividualItemPresenter> _subPresenterItemManager;
      private Species _species;

      protected override void Context()
      {
         _view = A.Fake<IEditIndividualView>();
         _workspace = A.Fake<ICoreWorkspace>();
         _individualSettingsPresenter = A.Fake<IIndividualSettingsPresenter>();
         _individualMoleculesPresenter = A.Fake<IIndividualMoleculesPresenter>();
         _individualToEdit = A.Fake<Individual>();
         _species = A.Fake<Species>();
         _species.Icon = "Human";
         A.CallTo(() => _individualToEdit.Species).Returns(_species);
         _subPresenterItemManager = A.Fake<ISubPresenterItemManager<IIndividualItemPresenter>>();
         A.CallTo(() => _subPresenterItemManager.AllSubPresenters).Returns(new IIndividualItemPresenter[] {_individualSettingsPresenter, _individualMoleculesPresenter});
         A.CallTo(() => _individualSettingsPresenter.Individual).Returns(_individualToEdit);
         sut = new EditIndividualPresenter(_view, _subPresenterItemManager);
         sut.InitializeWith(_workspace);
      }
   }

   public class When_initializing_the_edit_individual_presenter : concern_for_EditIndividualPresenter
   {
      [Observation]
      public void should_tell_the_view_to_render_the_sub_presenter_views()
      {
         A.CallTo(() => _subPresenterItemManager.InitializeWith(sut,IndividualItems.All)).MustHaveHappened();
      }
   }

   public class When_the_edit_individual_presenter_is_editing_an_individual : concern_for_EditIndividualPresenter
   {
      protected override void Because()
      {
         sut.Edit(_individualToEdit);
      }

      [Observation]
      public void should_ask_the_view_to_render()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_enable_the_setting_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(IndividualItems.Settings, true)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_enable_the_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(IndividualItems.Parameters, true)).MustHaveHappened();
      }

      [Observation]
      public void Should_tell_the_view_to_enable_the_expression_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(IndividualItems.Expression, true)).MustHaveHappened();
      }

      [Observation]
      public void Should_tell_the_view_to_activate_the_parameter_tab()
      {
         A.CallTo(() => _view.ActivateControl(IndividualItems.Parameters)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_all_sub_presenter_to_edit_the_individual()
      {
         A.CallTo(() => _individualSettingsPresenter.EditIndividual(_individualToEdit)).MustHaveHappened();
         A.CallTo(() => _individualMoleculesPresenter.EditIndividual(_individualToEdit)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_icon_in_the_view_according_to_the_selected_species()
      {
         A.CallTo(() => _view.UpdateIcon(ApplicationIcons.Human)).MustHaveHappened();
      }
   }

   public class When_collecting_the_commands_performed_by_the_user : concern_for_EditIndividualPresenter
   {
      private IPKSimCommand _command1;
      private IPKSimCommand _command2;

      protected override void Context()
      {
         base.Context();
         _command1 = A.Fake<IPKSimCommand>();
         _command2 = A.Fake<IPKSimCommand>();
      }

      protected override void Because()
      {
         sut.AddCommand(_command1);
         sut.AddCommand(_command2);
      }

      [Observation]
      public void should_add_the_commands_to_the_history()
      {
         A.CallTo(() => _workspace.AddCommand(_command1)).MustHaveHappened();
         A.CallTo(() => _workspace.AddCommand(_command2)).MustHaveHappened();
      }
   }

   public class When_the_edit_individual_presenter_is_being_notified_that_the_form_is_closing : concern_for_EditIndividualPresenter
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
      public void should_raise_the_closing_event()
      {
         _eventWasRaised.ShouldBeTrue();
      }
   }

   public class When_the_individual_presenter_is_asked_if_it_can_be_closed : concern_for_EditIndividualPresenter
   {
      [Observation]
      public void should_return_true_if_all_the_sub_presenter_can_be_closed_and_the_view_has_no_error()
      {
         A.CallTo(() => _subPresenterItemManager.CanClose).Returns(true);
         A.CallTo(() => _view.HasError).Returns(false);
         sut.CanClose.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_one_of_the_sub_presenter_has_an_error()
      {
         A.CallTo(() => _subPresenterItemManager.CanClose).Returns(false);
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_view_has_an_error()
      {
         A.CallTo(() => _view.HasError).Returns(true);
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_view_and_any_of_the_sub_presenter_has_an_error()
      {
         A.CallTo(() => _subPresenterItemManager.CanClose).Returns(false);
         A.CallTo(() => _view.HasError).Returns(true);
         sut.CanClose.ShouldBeFalse();
      }
   }

}