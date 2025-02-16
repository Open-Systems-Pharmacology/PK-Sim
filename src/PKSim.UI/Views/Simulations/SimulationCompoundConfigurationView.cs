using System;
using System.Collections.Generic;
using DevExpress.XtraLayout;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

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

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Compound;

      public void AttachPresenter(ISimulationCompoundConfigurationPresenter presenter)
      {
      }

      public void AddParameterAlternativesView(IResizableView view)
      {
         _subViews.Add(view);
         AddViewTo(layoutMainGroup, layoutSimulationCompound,  view);
      }

      public void AddCalculationMethodsView(IResizableView view)
      {
         _subViews.Add(view);
         AddViewTo(layoutMainGroup, layoutSimulationCompound, view);
      }

      protected override void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, LayoutControl layoutControl, IResizableView view, int height)
      {
         base.AdjustLayoutItemSize(layoutControlItem, layoutControl, view, height);
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public int OptimalHeight => layoutMainGroup.Height;

      public void AdjustHeight()
      {
         _subViews.Each(view => view.AdjustHeight());
      }

      public void Repaint()
      {
         _subViews.Each(view => view.Repaint());
      }

      public int DefaultHeight => UIConstants.Size.SIMULATION_COMPOUND_CONFIGURATION_DEFAULT_HEIGHT;
   }
}