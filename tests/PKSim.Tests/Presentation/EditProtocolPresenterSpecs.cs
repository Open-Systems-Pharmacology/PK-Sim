using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditProtocolPresenter : ContextSpecification<IEditProtocolPresenter>
   {
      protected IEditProtocolView _view;
      protected ISimpleProtocolPresenter _simpleProtocolPresenter;
      protected IAdvancedProtocolPresenter _advancedProtocolPresenter;
      protected IProtocolChartPresenter _protocolChartPresenter;
      protected ISubPresenterItemManager<IProtocolItemPresenter> _subPresenterManager;
      protected ISchemaTask _schemaTask;
      protected Protocol _protocolToEdit;
      protected IProtocolUpdater _protocolUpdater;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _view = A.Fake<IEditProtocolView>();
         _subPresenterManager = SubPresenterHelper.Create<IProtocolItemPresenter>();
         _simpleProtocolPresenter = _subPresenterManager.CreateFake(ProtocolItems.Simple);
         _advancedProtocolPresenter = _subPresenterManager.CreateFake(ProtocolItems.Advanced);
         _protocolChartPresenter = A.Fake<IProtocolChartPresenter>();
         _schemaTask = A.Fake<ISchemaTask>();
         _protocolUpdater = A.Fake<IProtocolUpdater>();
         _dialogCreator = A.Fake<IDialogCreator>();
         sut = new EditProtocolPresenter(_view, _subPresenterManager, _protocolChartPresenter, _schemaTask, _protocolUpdater, _dialogCreator);
      }
   }

   public class When_the_edit_protocol_presenter_is_editing_a_simple_protocol : concern_for_EditProtocolPresenter
   {
      protected override void Context()
      {
         base.Context();
         _protocolToEdit = new SimpleProtocol();
      }

      protected override void Because()
      {
         sut.Edit(_protocolToEdit);
      }

      [Observation]
      public void should_ask_the_simple_protocol_presenter_to_edit_the_protocol()
      {
         A.CallTo(() => _simpleProtocolPresenter.EditProtocol(_protocolToEdit)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_the_protocol_mode_according_to_the_type_of_the_protocol_and_set_it_to_the_view()
      {
         A.CallTo(() => _view.SetProtocolMode(ProtocolMode.Simple)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_simple_protocol_presenter_into_the_view()
      {
         A.CallTo(() => _view.UpdateEditControl(_simpleProtocolPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_edit_protocol_presenter_is_editing_an_advanced_protocol : concern_for_EditProtocolPresenter
   {
      protected override void Context()
      {
         base.Context();
         _protocolToEdit = new AdvancedProtocol();
      }

      protected override void Because()
      {
         sut.Edit(_protocolToEdit);
      }

      [Observation]
      public void should_ask_the_advanced_protocol_presenter_to_edit_the_protocol()
      {
         A.CallTo(() => _advancedProtocolPresenter.EditProtocol(_protocolToEdit)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_the_protocol_mode_according_to_the_type_of_the_protocol_and_set_it_to_the_view()
      {
         A.CallTo(() => _view.SetProtocolMode(ProtocolMode.Advanced)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_advanced_protocol_presenter_into_the_view()
      {
         A.CallTo(() => _view.UpdateEditControl(_advancedProtocolPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_edit_protocol_presenter_is_being_notified_that_the_protocol_mode_of_the_edited_protocol_changed_ : concern_for_EditProtocolPresenter
   {
      private Protocol _newProtocol;

      protected override void Context()
      {
         base.Context();
         _protocolToEdit = new AdvancedProtocol().WithId("toto");
         _newProtocol = new SimpleProtocol().WithId("toto");
         sut.Edit(_protocolToEdit);
      }

      protected override void Because()
      {
         sut.Handle(new ProtocolModeChangedEvent(_newProtocol, ProtocolMode.Simple));
      }

      [Observation]
      public void should_update_the_view_according_to_the_setting_of_the_new_protocol()
      {
         A.CallTo(() => _simpleProtocolPresenter.EditProtocol(_newProtocol)).MustHaveHappened();
      }
   }

   public class When_the_edit_protocol_presenter_is_being_notified_that_the_protocol_mode_another_protocol_changed : concern_for_EditProtocolPresenter
   {
      private Protocol _newProtocol;

      protected override void Context()
      {
         base.Context();
         _protocolToEdit = new AdvancedProtocol().WithId("toto");
         _newProtocol = new SimpleProtocol().WithId("tata");
         sut.Edit(_protocolToEdit);
      }

      protected override void Because()
      {
         sut.Handle(new ProtocolModeChangedEvent(_newProtocol, ProtocolMode.Simple));
      }

      [Observation]
      public void should_not_update_the_active_view()
      {
         A.CallTo(() => _simpleProtocolPresenter.EditProtocol(_newProtocol)).MustNotHaveHappened();
      }
   }

   public class When_notifed_that_the_underlying_protocol_was_renamed : concern_for_EditProtocolPresenter
   {
      protected override void Context()
      {
         base.Context();
         _protocolToEdit = A.Fake<Protocol>();
         _protocolToEdit.Name = "tralal";
         sut.Edit(_protocolToEdit);
      }

      protected override void Because()
      {
         _protocolToEdit.Name = "new name";
         sut.Handle(new RenamedEvent(_protocolToEdit));
      }

      [Observation]
      public void should_update_the_caption_of_the_view()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.EditProtocol(_protocolToEdit.Name));
      }
   }

   public class When_notifed_that_a_protocol_that_is_not_the_one_being_edited_was_renamed_ : concern_for_EditProtocolPresenter
   {
      private Protocol _anotherProtocol;

      protected override void Context()
      {
         base.Context();
         _protocolToEdit = A.Fake<Protocol>().WithName("toto");
         _anotherProtocol = A.Fake<Protocol>().WithName("tata");
         sut.Edit(_protocolToEdit);
      }

      protected override void Because()
      {
         sut.Handle(new RenamedEvent(_anotherProtocol));
      }

      [Observation]
      public void should_not_update_the_caption()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.EditProtocol(_protocolToEdit.Name));
      }
   }

   public class When_the_edit_protocol_presenter_is_asked_if_the_mode_switch_is_allowed_for_a_protocol_that_can_be_switched_ : concern_for_EditProtocolPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _protocolToEdit = A.Fake<Protocol>();
         A.CallTo(() => _protocolUpdater.ValidateSwitchFrom(_protocolToEdit)).Returns(true);
         sut.Edit(_protocolToEdit);
      }

      protected override void Because()
      {
         _result = sut.SwitchModeConfirm(ProtocolMode.Advanced);
      }

      [Observation]
      public void should_return_that_the_protocol_can_be_switched_to_simple_mode()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_the_edit_protocol_presenter_is_asked_if_the_mode_switch_is_allowed_for_a_protocol_that_cannot_be_switched_ : concern_for_EditProtocolPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _protocolToEdit = new SimpleProtocol();
         A.CallTo(() => _protocolUpdater.ValidateSwitchFrom(_protocolToEdit)).Returns(false);
         sut.Edit(_protocolToEdit);
      }

      protected override void Because()
      {
         _result = sut.SwitchModeConfirm(ProtocolMode.Advanced);
      }

      [Observation]
      public void should_return_that_the_protocol_cannot_be_switched_to_advanced_mode()
      {
         _result.ShouldBeFalse();
      }
   }
}