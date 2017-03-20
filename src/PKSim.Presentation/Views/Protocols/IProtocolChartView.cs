using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Protocols
{
   public interface IProtocolChartView : IView<IProtocolChartPresenter>
   {
      void Plot(IProtocolChartData dataToPlot);
      void Clear();
      string XAxisTitle { get; set; }
      string YAxisTitle { get; set; }
      string Y2AxisTitle { get; set; }
      double BarWidth { get; set; }
   }
}