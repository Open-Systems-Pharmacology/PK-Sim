using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface IUserDisplayUnitsPresenter : IPresenter<IUserDisplayUnitsView>, ISettingsItemPresenter
   {
      
   }

   public class UserDisplayUnitsPresenter: AbstractSubPresenter<IUserDisplayUnitsView, IUserDisplayUnitsPresenter>, IUserDisplayUnitsPresenter
   {
      private readonly IDisplayUnitsPresenter _displayUnitsPresenter;
      private readonly IUserSettings _userSettings;
      private readonly DisplayUnitsManager _settingsToEdit;
      private readonly ICloneManager _cloneManager;

      public UserDisplayUnitsPresenter(IUserDisplayUnitsView view, IDisplayUnitsPresenter displayUnitsPresenter, IUserSettings userSettings, ICloneManager cloneManager)
         : base(view)
      {
         _displayUnitsPresenter = displayUnitsPresenter;
         _userSettings = userSettings;
         _cloneManager = cloneManager;
         AddSubPresenters(_displayUnitsPresenter);
         _view.AddView(_displayUnitsPresenter.View);
         _settingsToEdit = cloneManager.Clone(_userSettings.DisplayUnits);
      }

      public void EditSettings()
      {
         _displayUnitsPresenter.Edit(_settingsToEdit);
      }

      public void SaveSettings()
      {
         _userSettings.DisplayUnits.UpdatePropertiesFrom(_settingsToEdit, _cloneManager); 
      }
   }
}