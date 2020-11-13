using System.Collections.Generic;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualProteinExpressionsView : BaseContainerUserControl, IIndividualProteinExpressionsView
   {
      private IIndividualProteinExpressionsPresenter _presenter;

      private readonly ScreenBinder<IIndividualProteinExpressionsPresenter> _screenBinder =
         new ScreenBinder<IIndividualProteinExpressionsPresenter>();

      public IndividualProteinExpressionsView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IIndividualProteinExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.ShowInitialConcentration)
            .To(chkShowInitialConcentration)
            .WithCaption(PKSimConstants.UI.ShowInitialConcentrationParameter);
      }

      public void AddMoleculePropertiesView(IView view) => AddViewTo(layoutItemMoleculeProperties, view);

      public void Bind() => _screenBinder.BindToSource(_presenter);

      public void AddLocalizationView(IView view) => AddViewTo(layoutItemPanelLocalization, view);

      public void AddExpressionParametersView(IView view) => AddViewTo(layoutItemPanelExpressionParameters, view);

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemMoleculeProperties.TextVisible = false;
         layoutItemPanelLocalization.TextVisible = false;
         layoutItemPanelExpressionParameters.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
      }

      public override bool HasError => _screenBinder.HasError;
   }
}