using System;
using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraLayout;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundConfigurationView : BaseContainerUserControl, ISimulationCompoundConfigurationView
   {
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };
      private readonly IList<IResizableView> _subViews;

      public SimulationCompoundConfigurationView()
      {
         InitializeComponent();
         _subViews = new List<IResizableView>();
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.Compound; }
      }

      public void AttachPresenter(ISimulationCompoundConfigurationPresenter presenter)
      {
      }

      public void AddParameterAlernativesView(IResizableView view)
      {
         _subViews.Add(view);
         AddViewTo(layoutMainGroup, view);
      }

      public void AddCalculationMethodsView(IResizableView view)
      {
         _subViews.Add(view);
         AddViewTo(layoutMainGroup, view);
      }

      protected override void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, IResizableView view, int height)
      {
         base.AdjustLayoutItemSize(layoutControlItem, view, height);
         raiseHeightChangedEvent();
      }

      private void raiseHeightChangedEvent()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public int OptimalHeight
      {
         get { return layoutMainGroup.Height; }
      }

      public void AdjustHeight()
      {
         _subViews.Each(view => view.AdjustHeight());
      }

      public void Repaint()
      {
         _subViews.Each(view => view.Repaint());
      }
   }
}