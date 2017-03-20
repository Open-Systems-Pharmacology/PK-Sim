using OSPSuite.UI.Services;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Binders;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class ScatterChartView : BaseChartView<ScatterXValue, ScatterYValue>, IScatterChartView
   {
      public ScatterChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
         : base(imageListRetriever, toolTipCreator)
      {
         InitializeComponent();
         ChartsDataBinder = new ScatterChartDataBinder(this);
      }

      public void AttachPresenter(IScatterChartPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}