using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationModelConfigurationView : IView<ISimulationModelConfigurationPresenter>
   {
      void AddSubView(ISubPresenterItem subPresenterItem, IView view);
   }
}