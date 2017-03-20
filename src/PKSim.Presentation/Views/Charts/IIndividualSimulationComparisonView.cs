using System.Windows.Forms;
using PKSim.Presentation.Presenters.Charts;

namespace PKSim.Presentation.Views.Charts
{
   public interface IIndividualSimulationComparisonView : IChartView<IIndividualSimulationComparisonPresenter>
   {
      event DragEventHandler DragOver;
      event DragEventHandler DragDrop;
   }
}