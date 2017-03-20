using OSPSuite.UI.Services;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Binders;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class RangeChartView : BaseChartView<RangeXValue, RangeYValue>, IRangeChartView
   {
      public RangeChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator) : base(imageListRetriever, toolTipCreator)
      {
         InitializeComponent();
         ChartsDataBinder = new RangeChartDataBinder(this);
         _chartControl.SeriesSelectionMode = SeriesSelectionMode.Series;
      }

      public void AttachPresenter(IRangeChartPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}