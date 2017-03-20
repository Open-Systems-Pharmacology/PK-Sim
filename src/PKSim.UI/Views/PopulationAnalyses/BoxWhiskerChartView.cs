using OSPSuite.UI.Services;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Binders;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class BoxWhiskerChartView : BaseChartView<BoxWhiskerXValue, BoxWhiskerYValue>, IBoxWhiskerChartView
   {
      public BoxWhiskerChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator) : base(imageListRetriever, toolTipCreator)
      {
         InitializeComponent();
         ChartsDataBinder = new BoxWhiskerChartDataBinder(this);
         _chartControl.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
         _chartControl.Legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
      }

      public void AttachPresenter(IBoxWhiskerChartPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}