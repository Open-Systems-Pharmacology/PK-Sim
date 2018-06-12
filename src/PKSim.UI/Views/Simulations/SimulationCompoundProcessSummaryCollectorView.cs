using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProcessSummaryCollectorView : BaseContainerUserControl, ISimulationCompoundProcessSummaryCollectorView
   {
      private ISimulationCompoundProcessSummaryCollectorPresenter _presenter;

      public SimulationCompoundProcessSummaryCollectorView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationItemPresenter presenter)
      {
         _presenter = presenter as ISimulationCompoundProcessSummaryCollectorPresenter;
      }

      public void AddInteractionView(IView view)
      {
         AddViewTo(layoutItemInhibitionSelection, view);
      }

      public bool ShowInteractionView
      {
         get => layoutGroupInhibitionSelection.Visible;
         set
         {
            layoutGroupInhibitionSelection.Visibility=LayoutVisibility.Never;
            layoutGroupInhibitionSelection.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         }
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         showDiagramButton.Click += (o, e) => OnEvent(showDiagram);
      }

      private void showDiagram()
      {
         _presenter.ShowDiagram();
      }

      public void AddCollectorView(IView view)
      {
         panelViewCollector.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemInhibitionSelection.TextVisible = false;
         layoutItemViewCollector.TextVisible = false;
         layoutGroupInhibitionSelection.Text = PKSimConstants.UI.InhibitionAndInduction;
         showDiagramButton.Text = PKSimConstants.UI.ShowDiagram;
      }
   }
}