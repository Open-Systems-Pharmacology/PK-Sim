using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundCollectorView : BaseUserControl, ISimulationCompoundCollectorView
   {
      public SimulationCompoundCollectorView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationItemPresenter presenter)
      {
         /*nothing to do here*/
      }

      public void AddCollectorView(IView view)
      {
         panelControl.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemPanel.TextVisible = false;
      }
   }
}