using OSPSuite.Core.Services;
using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Protocols
{
   public interface IProtocolChartView : 
      IView<IProtocolChartPresenter>, 
      ICanCopyToClipboardWithWatermark
   {
      void Plot(IProtocolChartData dataToPlot);
      void Clear();
      string XAxisTitle { get; set; }
      string YAxisTitle { get; set; }
      string Y2AxisTitle { get; set; }
      double BarWidth { get; set; }
   }
}