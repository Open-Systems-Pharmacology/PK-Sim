using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Views;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.Presentation.Views.Populations
{
   public interface IPopulationParameterDistributionView : 
      IView<IPopulationDistributionPresenter>,
      ICanCopyToClipboardWithWatermark
   {
      void Plot(ContinuousDistributionData dataToPlot, DistributionSettings settings);
      void Plot(DiscreteDistributionData dataToPlot, DistributionSettings settings);
      void ResetPlot();
   }
}