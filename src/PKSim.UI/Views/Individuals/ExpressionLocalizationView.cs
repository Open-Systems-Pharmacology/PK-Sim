using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using System;
using System.Linq.Expressions;
using static PKSim.UI.UIConstants.Size;

namespace PKSim.UI.Views.Individuals
{
   public partial class ExpressionLocalizationView : BaseResizableUserControl, IExpressionLocalizationView
   {
      private readonly ScreenBinder<IndividualProtein> _screenBinder;
      private IExpressionLocalizationPresenter _presenter;

      public ExpressionLocalizationView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<IndividualProtein>{BindingMode = BindingMode.OneWay};
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         initializeLocalization(layoutItemIntracellular, PKSimConstants.UI.LocalizationIntracellular);
         initializeLocalization(layoutItemInterstitial, PKSimConstants.UI.LocalizationInterstitial);
         initializeLocalization(layoutItemBloodCellsIntracellular, PKSimConstants.UI.LocalizationBloodCellsIntracellular);
         initializeLocalization(layoutItemBloodCellsMembrane, PKSimConstants.UI.LocalizationBloodCellsMembrane);
         initializeLocalization(layoutItemVascEndosome, PKSimConstants.UI.LocalizationVascularEndosomes);
         initializeLocalization(layoutItemVascMembranePlasmaSide, PKSimConstants.UI.LocalizationVascularMembranePlasmaSide);
         initializeLocalization(layoutItemVascMembraneTissueSide, PKSimConstants.UI.LocalizationVascularMembraneTissueSide);
      }

      private void initializeLocalization(LayoutControlItem layoutItem, string caption)
      {
         layoutItem.TextVisible = false;
         layoutItem.Control.Text = caption;
      }

      public void AttachPresenter(IExpressionLocalizationPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IndividualProtein individualProtein)
      {
         _screenBinder.BindToSource(individualProtein);
         AdjustHeight();
      }

      public void RefreshData() => _screenBinder.Update();

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         bind(x => x.IsIntracellular, chkIntracellular, Localization.Intracellular);
         bind(x => x.IsInterstitial, chkInterstitial, Localization.Interstitial);
         bind(x => x.IsBloodCellsIntracellular, chkBloodCellsIntracellular, Localization.BloodCellsIntracellular);
         bind(x => x.IsBloodCellsMembrane, chkBloodCellsMembrane, Localization.BloodCellsMembrane);
         bind(x => x.IsVascEndosome, chkVascEndosome, Localization.VascEndosome);
         bind(x => x.IsVascMembranePlasmaSide, chkVascMembranePlasmaSide, Localization.VascMembranePlasmaSide);
         bind(x => x.IsVascMembraneTissueSide, chkVascMembraneTissueSide, Localization.VascMembraneTissueSide);
      }

      private void bind(Expression<Func<IndividualProtein, bool>> expression, CheckEdit control, Localization localization)
      {
         _screenBinder.Bind(expression)
            .To(control)
            .OnValueUpdating += (o, e) => OnEvent(() =>
         {
           var updated =  _presenter.UpdateLocalization(localization, e.NewValue);
           if(updated)
              return;

           //Need to reset the selection explicitly
           control.Checked = !e.NewValue;
         });
      }

      public override int OptimalHeight => EXPRESSION_PROFILE_LOCALIZATION_HEIGHT;
   }
}