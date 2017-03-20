using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundProcessSummaryView : IView<ISimulationCompoundProcessSummaryPresenter>, IResizableView
   {
      void AddProcessView(IResizableView view);
   }
}