using System;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface ISettingsPresenter : IPresenter<ISettingsView>, IContainerPresenter
   {
      void ResetLayout();
   }

   public class SettingsPresenter : AbstractSubPresenterContainerPresenter<ISettingsView, ISettingsPresenter, ISettingsItemPresenter>, ISettingsPresenter
   {
      private readonly ICoreWorkspace _workspace;

      public SettingsPresenter(ISettingsView view, ISubPresenterItemManager<ISettingsItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator, ICoreWorkspace workspace)
         : base(view, subPresenterItemManager, SettingsItems.All, dialogCreator)
      {
         _workspace = workspace;
      }

      public override void Initialize()
      {
         InitializeWith(_workspace);
         _subPresenterItemManager.AllSubPresenters.Each(x => x.EditSettings());

         _view.OkEnabled = false;
         _view.Display();

         if (_view.Canceled) return;

         saveSettings();
      }

      public void ResetLayout()
      {
         PresenterAt(SettingsItems.UserGeneralSettings).ResetLayout();
      }

      private void saveSettings()
      {
         //order matters here as display units should be saved before general settings
         PresenterAt(SettingsItems.UserDisplayUnitsSettings).SaveSettings();
         PresenterAt(SettingsItems.UserGeneralSettings).SaveSettings();
         PresenterAt(SettingsItems.ApplicationSettings).SaveSettings();
      }

      protected override void OnStatusChanged(object sender, EventArgs e)
      {
         base.OnStatusChanged(sender, e);
         _view.OkEnabled = CanClose;
      }
   }
}