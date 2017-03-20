using System.Windows.Forms;
using PKSim.Presentation.Presenters.Charts;
using SBSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Charts
{
   public interface ISummaryChartMdiView : IView<ISummaryChartMdiPresenter>, IMdiChildView
   {
      bool ChartVisible { get; set; }
      void AddChartView(IView view);
   }

   public interface ISummaryChartView : IChartView<ISummaryChartPresenter>
   {
      event DragEventHandler DragOver;
      event DragEventHandler DragDrop;
   }
}