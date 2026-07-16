using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundConfigurationView : IView<ISimulationCompoundConfigurationPresenter>, IResizableWithDefaultHeightView
   {
      void AddParameterAlternativesView(IResizableView view);
      void AddCalculationMethodsView(IResizableView view);
      void AddOverwriteParameterSetSelectionView(IResizableView view);
   }
}