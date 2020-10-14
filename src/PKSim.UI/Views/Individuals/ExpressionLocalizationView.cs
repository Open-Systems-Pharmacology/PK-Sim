﻿using System;
using System.Linq.Expressions;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class ExpressionLocalizationView : BaseResizableUserControl, IExpressionLocalizationView
   {
      private readonly ScreenBinder<IndividualProtein> _screenBinder;
      private IExpressionLocalizationPresenter _presenter;
      private const int HEIGHT = 93;
      public ExpressionLocalizationView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<IndividualProtein>();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         initializeLocalization(layoutItemIntracellular, PKSimConstants.UI.LocalizationIntracellular);
         initializeLocalization(layoutItemInterstitial, PKSimConstants.UI.LocalizationInterstitial);
         initializeLocalization(layoutItemBloodCellsIntracellular, PKSimConstants.UI.LocalizationBloodCellsIntracellular);
         initializeLocalization(layoutItemBloodCellsMembrane, PKSimConstants.UI.LocalizationBloodCellsMembrane);
         initializeLocalization(layoutItemVascEndosome, PKSimConstants.UI.LocalizationVascEndosome);
         initializeLocalization(layoutItemVascMembraneApical, PKSimConstants.UI.LocalizationVascMembraneApical);
         initializeLocalization(layoutItemVascMembraneBasolateral, PKSimConstants.UI.LocalizationVascMembraneBasolateral);
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

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         bind(x=>x.IsIntracellular, chkIntracellular, Localization.Intracellular);
         bind(x=>x.IsInterstitial, chkInterstitial, Localization.Interstitial);
         bind(x=>x.IsBloodCellsIntracellular, chkBloodCellsIntracellular, Localization.BloodCellsIntracellular);
         bind(x=>x.IsBloodCellsMembrane, chkBloodCellsMembrane, Localization.BloodCellsMembrane);
         bind(x=>x.IsVascEndosome, chkVascEndosome, Localization.VascEndosome);
         bind(x=>x.IsVascMembraneApical, chkVascMembraneApical, Localization.VascMembraneApical);
         bind(x=>x.IsVascMembraneBasolateral, chkVascMembraneBasolateral, Localization.VascMembraneBasolateral);
      }


      private void bind(Expression<Func<IndividualProtein, bool>> expression, CheckEdit control, Localization localization)
      {
         _screenBinder.Bind(expression)
            .To(control)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.UpdateLocalization(localization, e.NewValue));
      }

      public override int OptimalHeight => HEIGHT;
   }
}