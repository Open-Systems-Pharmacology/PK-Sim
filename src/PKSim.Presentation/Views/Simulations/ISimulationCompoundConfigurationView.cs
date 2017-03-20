using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundConfigurationView : IView<ISimulationCompoundConfigurationPresenter>, IResizableView
   {
      void AddParameterAlernativesView(IResizableView view);
      void AddCalculationMethodsView(IResizableView view);
   }
}