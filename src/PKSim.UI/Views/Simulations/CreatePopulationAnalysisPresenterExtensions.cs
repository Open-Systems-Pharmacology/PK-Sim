using DevExpress.XtraLayout;
using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Assets;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   public static class CreatePopulationAnalysisPresenterExtensions
   {
      public static LayoutControlItem CreateTemplateButtonItem(this ICreatePopulationAnalysisPresenter presenter, IToolTipCreator toolTipCreator, LayoutControl layoutControl)
      {
         var toolTip = toolTipCreator.CreateToolTip(PKSimConstants.UI.PopulationAnalysisSaveLoadToolTip, PKSimConstants.UI.PopulationAnalysisSaveLoad);
         var dropDownButton = new UxDropDownButton(PKSimConstants.UI.PopulationAnalysisSaveLoad, ApplicationIcons.Save, toolTip);
         dropDownButton.AddMenu(PKSimConstants.UI.SaveAsTemplate, presenter.SaveAnalysis, ApplicationIcons.SaveAsTemplate);
         dropDownButton.AddMenu(PKSimConstants.UI.LoadFromTemplate, presenter.LoadAnalysisTask, ApplicationIcons.LoadFromTemplate);
         return layoutControl.AddButtonItemFor(dropDownButton);
      }
   }
}