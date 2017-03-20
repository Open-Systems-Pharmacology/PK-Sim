using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters
{
   public static class SettingsItems
   {
      public static readonly SettingsItem<IUserSettingsPresenter> UserGeneralSettings = new SettingsItem<IUserSettingsPresenter>();
      public static readonly SettingsItem<IUserDisplayUnitsPresenter> UserDisplayUnitsSettings = new SettingsItem<IUserDisplayUnitsPresenter>();
      public static readonly SettingsItem<IApplicationSettingsPresenter> ApplicationSettings = new SettingsItem<IApplicationSettingsPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { UserGeneralSettings, UserDisplayUnitsSettings,ApplicationSettings };
   }

   public class SettingsItem<TPresenter> : SubPresenterItem<TPresenter> where TPresenter : ISettingsItemPresenter
   {
   }
}