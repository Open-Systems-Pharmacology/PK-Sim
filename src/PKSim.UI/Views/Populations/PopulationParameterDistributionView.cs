using OSPSuite.UI.Services;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Chart;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.UI.Views.Populations
{
   public partial class PopulationParameterDistributionView : BaseUserControl, IPopulationParameterDistributionView
   {
      private IPopulationDistributionPresenter _presenter;

      public PopulationParameterDistributionView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         InitializeComponent();
         chart.Initialize(imageListRetriever, toolTipCreator);
      }

      public void AttachPresenter(IPopulationDistributionPresenter presenter)
      {
         _presenter = presenter;
         chart.AddCopyToClipboardPopupMenu(presenter);
         chart.EndColorFor = _presenter.EndColorFor;
         chart.StartColorFor = _presenter.StartColorFor;
      }

      public void Plot(ContinuousDistributionData dataToPlot, DistributionSettings settings)
      {
         chart.Plot(dataToPlot, settings);
      }

      public void Plot(DiscreteDistributionData dataToPlot, DistributionSettings settings)
      {
         chart.Plot(dataToPlot, settings);
      }

      public void ResetPlot()
      {
         chart.ResetPlot();
      }

      public void CopyToClipboard(string watermark)
      {
         chart.CopyToClipboard(watermark);
      }
   }
}