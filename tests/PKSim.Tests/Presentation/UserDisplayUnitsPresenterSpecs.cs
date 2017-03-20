using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_UserDisplayUnitsPresenter : ContextSpecification<IUserDisplayUnitsPresenter>
   {
      protected ICloneManager _cloneManager;
      private IUserDisplayUnitsView _view;
      protected IDisplayUnitsPresenter _displayUnitPresenter;
      protected IUserSettings _userSettings;
      protected DisplayUnitsManager _cloneDisplayUnitsManager;

      protected override void Context()
      {
         _cloneManager = A.Fake<ICloneManager>();
         _view = A.Fake<IUserDisplayUnitsView>();
         _displayUnitPresenter = A.Fake<IDisplayUnitsPresenter>();
         _userSettings = A.Fake<IUserSettings>();

         _cloneDisplayUnitsManager = new DisplayUnitsManager();
         A.CallTo(() => _cloneManager.Clone(_userSettings.DisplayUnits)).Returns(_cloneDisplayUnitsManager);
         sut = new UserDisplayUnitsPresenter(_view, _displayUnitPresenter, _userSettings, _cloneManager);
      }
   }

   public class When_editing_the_display_units_of_the_user_settings : concern_for_UserDisplayUnitsPresenter
   {
      protected override void Because()
      {
         sut.EditSettings();
      }

      [Observation]
      public void should_edit_a_clone_of_the_user_settings()
      {
         A.CallTo(() => _displayUnitPresenter.Edit(_cloneDisplayUnitsManager)).MustHaveHappened();
      }
   }

   public class When_saving_the_edited_display_units_of_the_user_settings : concern_for_UserDisplayUnitsPresenter
   {
      protected override void Because()
      {
         sut.SaveSettings();
      }

      [Observation]
      public void should_update_the_user_display_units_based_on_the_edited_units()
      {
         A.CallTo(() => _userSettings.DisplayUnits.UpdatePropertiesFrom(_cloneDisplayUnitsManager, _cloneManager)).MustHaveHappened();
      }
   }
}