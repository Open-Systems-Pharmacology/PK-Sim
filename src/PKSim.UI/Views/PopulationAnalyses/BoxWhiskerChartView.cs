using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using OSPSuite.Assets;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Binders;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class BoxWhiskerChartView : BaseChartView<BoxWhiskerXValue, BoxWhiskerYValue>, IBoxWhiskerChartView
   {
      private BarItemLink _exportIndividualsMenu;

      public BoxWhiskerChartView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator) : base(imageListRetriever, toolTipCreator)
      {
         InitializeComponent();
         ChartsDataBinder = new BoxWhiskerChartDataBinder(this);
         Chart.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Left;
         Chart.Legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
      }

      public void AttachPresenter(IBoxWhiskerChartPresenter presenter)
      {
         _presenter = presenter;
      }

      protected override void ConfigurePopup(CancelEventArgs cancelEventArgs)
      {
         base.ConfigurePopup(cancelEventArgs);
         _exportIndividualsMenu.Visible = canExtract;
      }

      public override void AddDynamicMenus(bool allowEdit = true)
      {
         base.AddDynamicMenus(allowEdit);
         _exportIndividualsMenu = Chart.AddPopupMenu(PKSimConstants.MenuNames.ExtractIndividualByPercentile, extractIndividuals, ApplicationIcons.Individual, beginGroup: true);
      }

      private void extractIndividuals()
      {
         if (!canExtract)
            return;

         int index = _latestTrackedCurvedData.GetPointIndexForDisplayValues(_latestSeriesPoint.NumericalArgument, _latestSeriesPoint.Values[0]);

         boxWhiskerChartPresenter.ExtractIndividuals(_latestTrackedCurvedData.YValues[index]);
      }

      private IBoxWhiskerChartPresenter boxWhiskerChartPresenter => _presenter.DowncastTo<IBoxWhiskerChartPresenter>();

      private bool canExtract => _latestTrackedCurvedData != null && _latestSeriesPoint != null;
   }
}