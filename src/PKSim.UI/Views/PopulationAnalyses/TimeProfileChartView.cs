using OSPSuite.UI.Services;
using DevExpress.XtraCharts;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Binders;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class TimeProfileChartView : BaseChartView<TimeProfileXValue, TimeProfileYValue>, ITimeProfileChartView
   {
      public TimeProfileChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator) : base(imageListRetriever, toolTipCreator)
      {
         InitializeComponent();
         ChartsDataBinder = new TimeProfileChartDataBinder(this);
         Chart.SeriesSelectionMode = SeriesSelectionMode.Series;
      }

      public void AttachPresenter(ITimeProfileChartPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}