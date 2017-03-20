using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;

using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateProtocolPresenter : ContextSpecification<ICreateProtocolPresenter>
   {
      private ICreateProtocolView _view;
      private IBuildingBlockPropertiesMapper _propertiesMapper;
      private IProtocolFactory _protocolFactory;
      private ISimpleProtocolPresenter _simpleProtocolPresenter;
      private IProtocolToProtocolPropertiesDTOMapper _propertiesDTOMapper;
      private IAdvancedProtocolPresenter _advancedProtocolPresenter;
      protected IProtocolUpdater _protocolUpdater;
      private IProtocolChartPresenter _protocolChartPresenter;
      private ISubPresenterItemManager<IProtocolItemPresenter> _subPresenterManager;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _view = A.Fake<ICreateProtocolView>();
         _propertiesMapper = A.Fake<IBuildingBlockPropertiesMapper>();
         _simpleProtocolPresenter = A.Fake<ISimpleProtocolPresenter>();
         _protocolFactory = A.Fake<IProtocolFactory>();
         _propertiesDTOMapper = A.Fake<IProtocolToProtocolPropertiesDTOMapper>();
         _advancedProtocolPresenter = A.Fake<IAdvancedProtocolPresenter>();
         _protocolUpdater = A.Fake<IProtocolUpdater>();
         _protocolChartPresenter = A.Fake<IProtocolChartPresenter>();
         _subPresenterManager = A.Fake<ISubPresenterItemManager<IProtocolItemPresenter>>();
         _dialogCreator = A.Fake<IDialogCreator>();
         A.CallTo(() => _protocolFactory.Create(ProtocolMode.Simple)).Returns(new SimpleProtocol());
         A.CallTo(() => _subPresenterManager.AllSubPresenters).Returns(new IProtocolItemPresenter[] {_simpleProtocolPresenter, _advancedProtocolPresenter});
         sut = new CreateProtocolPresenter(_view, _subPresenterManager, _propertiesMapper,
            _protocolChartPresenter, _protocolFactory, _protocolUpdater, _propertiesDTOMapper, _dialogCreator);

         sut.Initialize();
      }
   }

   public class When_the_create_protocol_presenter_is_asked_if_the_mode_switch_is_allowed_for_an_regular_switch_to_simple_mode : concern_for_CreateProtocolPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallySwitchProtocolMode)).Returns(ViewResult.Yes);
         A.CallTo(() => _protocolUpdater.ValidateSwitchFrom(sut.BuildingBlock)).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.SwitchModeConfirm(ProtocolMode.Simple);
      }

      [Observation]
      public void should_ask_the_user_if_a_switch_if_necessary()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallySwitchProtocolMode)).MustHaveHappened();
      }

      [Observation]
      public void should_return_that_he_protocol_can_be_switched_to_simple_mode()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_the_create_protocol_presenter_is_asked_if_the_mode_switch_is_allowed_for_a_forbidden_switch_to_advanced_mode : concern_for_CreateProtocolPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _protocolUpdater.ValidateSwitchFrom(sut.BuildingBlock)).Returns(false);
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

   public class When_the_create_protocol_presenter_is_asked_if_the_mode_switch_is_allowed_and_the_user_cancels_the_action : concern_for_CreateProtocolPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallySwitchProtocolMode)).Returns(ViewResult.No);
      }

      protected override void Because()
      {
         _result = sut.SwitchModeConfirm(ProtocolMode.Simple);
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_the_create_protocol_presenter_is_asked_if_the_mode_switch_is_allowed_to_advanced : concern_for_CreateProtocolPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _protocolUpdater.ValidateSwitchFrom(A<Protocol>._)).Returns(true);
      }

      [Observation]
      public void should_return_true()
      {
         sut.SwitchModeConfirm(ProtocolMode.Advanced).ShouldBeTrue();
      }
   }
}