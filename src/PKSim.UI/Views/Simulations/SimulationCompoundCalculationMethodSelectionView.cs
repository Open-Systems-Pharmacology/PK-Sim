using System;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundCalculationMethodSelectionView : BaseContainerUserControl, ISimulationCompoundCalculationMethodSelectionView
   {
      private IResizableView _resizableView;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public SimulationCompoundCalculationMethodSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationCompoundCalculationMethodSelectionPresenter presenter)
      {
      }

      public void AddCalculationMethodsView(IView view)
      {
         _resizableView = view as IResizableView;
         AddViewToGroup(layoutControlGroup, view);
         layoutControlGroup.Visibility = LayoutVisibility.Always;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.CalculationMethods;
      }

      protected override void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, IResizableView view, int height)
      {
         base.AdjustLayoutItemSize(layoutControlItem, view, height);
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void AdjustHeight() => _resizableView?.AdjustHeight();

      public void Repaint() => _resizableView?.Repaint();

      public int OptimalHeight => layoutControlGroup.Height;
   }
}