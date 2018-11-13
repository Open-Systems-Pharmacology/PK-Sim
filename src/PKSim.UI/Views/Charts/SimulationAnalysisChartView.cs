using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Charts
{
   public partial class SimulationAnalysisChartView : BaseUserControl, ISimulationAnalysisChartView
   {
      private IEditPopulationAnalysisChartPresenter _presenter;

      public SimulationAnalysisChartView()
      {
         InitializeComponent();
         ApplicationIcon = ApplicationIcons.TimeProfileAnalysis;
      }

      public void SetChartView(IView view)
      {
         this.FillWith(view);
      }

      public void UpdateIcon(ApplicationIcon icon)
      {
         ApplicationIcon = icon;
      }

      public void AttachPresenter(IEditPopulationAnalysisChartPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}