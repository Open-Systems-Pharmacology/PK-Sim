using PKSim.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Charts
{
   public interface IIndividualSimulationComparisonMdiView : IView<IIndividualSimulationComparisonMdiPresenter>, IMdiChildView
   {
      bool ChartVisible { get; set; }
      void AddChartView(IView view);
   }
}