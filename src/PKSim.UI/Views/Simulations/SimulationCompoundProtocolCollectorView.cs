using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProtocolCollectorView : BaseContainerUserControl, ISimulationCompoundProtocolCollectorView
   {
      public SimulationCompoundProtocolCollectorView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationItemPresenter presenter)
      {
         /*nothing to do here*/
      }

      public void AddCollectorView(IView view)
      {
         panelCollectorView.FillWith(view);
      }

      public bool WarningVisible
      {
         set => layoutItemWarning.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemWarning.Visibility);
      }

      public string Warning
      {
         set => uxHintPanel.NoteText = value;
         get => uxHintPanel.NoteText;
      }

      public void AddProtocolChart(IView view)
      {
         panelChartView.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemChart.TextVisible = false;
         layoutItemCollector.TextVisible = false;
         uxHintPanel.Image = ApplicationIcons.ErrorHint;
      }
   }
}