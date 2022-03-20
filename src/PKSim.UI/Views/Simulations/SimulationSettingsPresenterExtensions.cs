using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   public static class SimulationSettingsPresenterExtensions
   {
      public static void UpdateSaveSettingsButtonItem<TSimulation>(this ISimulationOutputSelectionPresenter<TSimulation> presenter, IToolTipCreator toolTipCreator, UxDropDownButton dropDownButton)
         where TSimulation : Simulation
      {
         var toolTip = toolTipCreator.CreateToolTip(PKSimConstants.UI.SaveSimulationSettingsToolTip, PKSimConstants.UI.SaveSimulationSettings);
         dropDownButton.Text = PKSimConstants.UI.SaveSimulationSettings;
         dropDownButton.Image = ApplicationIcons.Save.ToImage(IconSizes.Size16x16);
         dropDownButton.SuperTip = toolTip;
         dropDownButton.AddMenu(PKSimConstants.UI.SaveSimulationSettingsToProject, presenter.SaveSettingsToProject, ApplicationIcons.PKSim);
         dropDownButton.AddMenu(PKSimConstants.UI.SaveSimulationSettingsToUserSettings, presenter.SaveSettingsToUserSettings, ApplicationIcons.UserSettings);
      }
   }
}