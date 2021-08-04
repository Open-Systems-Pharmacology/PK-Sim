using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualProteinExpressionsView : BaseContainerUserControl, IIndividualProteinExpressionsView
   {
      private IIndividualProteinExpressionsPresenter _presenter;

      public IndividualProteinExpressionsView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IIndividualProteinExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }
      
      public void AddMoleculePropertiesView(IView view) => AddViewTo(layoutItemMoleculeProperties, view);

      public void AddLocalizationView(IView view) => AddViewTo(layoutItemPanelLocalization, view);

      public void AddExpressionParametersView(IView view) => AddViewTo(layoutItemPanelExpressionParameters, view);

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemMoleculeProperties.TextVisible = false;
         layoutItemPanelLocalization.TextVisible = false;
         layoutItemPanelExpressionParameters.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeProperties.ExpandButtonVisible = true;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
         layoutGroupMoleculeLocalization.ExpandButtonVisible = true;
      }
   }
}