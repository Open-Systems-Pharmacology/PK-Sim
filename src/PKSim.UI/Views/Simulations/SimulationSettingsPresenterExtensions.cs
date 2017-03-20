using DevExpress.XtraLayout;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Assets;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   public static class SimulationSettingsPresenterExtensions
   {
      public static LayoutControlItem CreateSaveSettingsButtonItem<TSimulation>(this ISimulationOutputSelectionPresenter<TSimulation> presenter, IToolTipCreator toolTipCreator, LayoutControl layoutControl)
         where TSimulation : Simulation 
      {
         var toolTip = toolTipCreator.CreateToolTip(PKSimConstants.UI.SaveSimulationSettingsToolTip, PKSimConstants.UI.SaveSimulationSettings);
         var dropDownButton = new UxDropDownButton(PKSimConstants.UI.SaveSimulationSettings, ApplicationIcons.Save, toolTip);
         dropDownButton.AddMenu(PKSimConstants.UI.SaveSimulationSettingsToProject, presenter.SaveSettingsToProject, ApplicationIcons.PKSim);
         dropDownButton.AddMenu(PKSimConstants.UI.SaveSimulationSettingsToUserSettings, presenter.SaveSettingsToUserSettings, ApplicationIcons.UserSettings);
         return layoutControl.AddButtonItemFor(dropDownButton);
      }
   }
}