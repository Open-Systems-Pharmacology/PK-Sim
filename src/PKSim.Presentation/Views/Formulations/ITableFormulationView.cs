using PKSim.Presentation.Presenters.Formulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Formulations
{
   public interface ITableFormulationView : IView<ITableFormulationPresenter>
   {
      void AddTableView(IView view);
      void AddParametersView(IView view);
   }
}