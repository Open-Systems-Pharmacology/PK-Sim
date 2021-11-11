using FakeItEasy;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateExpressionProfilePresenter : ContextSpecification<ICreateExpressionProfilePresenter>
   {
      protected ICreateExpressionProfileView _view;
      protected ISubPresenterItemManager<IExpressionProfileItemPresenter> _subPresenterManager;
      protected IDialogCreator _dialogCreator;
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected IExpressionProfileMoleculesPresenter _moleculesPresenter;
      protected ExpressionProfile _expressionProfile;
      protected IExpressionProfileUpdater _expressionProfileUpdater;

      protected override void Context()
      {
         _view = A.Fake<ICreateExpressionProfileView>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _expressionProfileFactory = A.Fake<IExpressionProfileFactory>();
         _expressionProfileUpdater= A.Fake<IExpressionProfileUpdater>();
         _subPresenterManager = SubPresenterHelper.Create<IExpressionProfileItemPresenter>();
         _moleculesPresenter = _subPresenterManager.CreateFake(ExpressionProfileItems.Molecules);
         sut = new CreateExpressionProfilePresenter(_view, _subPresenterManager, _dialogCreator, _expressionProfileFactory, _expressionProfileUpdater);

         _expressionProfile = A.Fake<ExpressionProfile>();
         A.CallTo(() => _expressionProfile.Icon).Returns(ApplicationIcons.Enzyme.IconName);
         A.CallTo(() => _expressionProfileFactory.Create<IndividualEnzyme>()).Returns(_expressionProfile);
      }
   }

   public class When_creating_a_new_expression_profile : concern_for_CreateExpressionProfilePresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         sut.Create<IndividualEnzyme>();
      }

      [Observation]
      public void should_create_a_new_expression_profile_and_edit_the_newly_created_expression_profile_in_all_sub_presenters()
      {
         A.CallTo(() => _moleculesPresenter.Edit(_expressionProfile)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_icon_used_in_the_view_to_reflect_the_enzyme_type()
      {
         _view.ApplicationIcon.ShouldBeEqualTo(ApplicationIcons.Enzyme);
      }

      [Observation]
      public void the_returned_building_block_should_be_the_expression_profile_created()
      {
         sut.BuildingBlock.ShouldBeEqualTo(_expressionProfile);
      }
   }

   public class When_the_create_expression_profile_presenter_is_being_notified_that_the_view_has_changed : concern_for_CreateExpressionProfilePresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Create<IndividualEnzyme>();
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_refresh_the_ok_button_enable_state()
      {
         _view.OkEnabled.ShouldBeEqualTo(_subPresenterManager.CanClose);
      }
   }

   public class When_creating_a_new_expression_profile_and_the_user_cancels_the_action : concern_for_CreateExpressionProfilePresenter
   {
      private IPKSimCommand _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.Create<IndividualEnzyme>();
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }
}