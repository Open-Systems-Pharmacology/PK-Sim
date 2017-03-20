using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationOutputSelectionView<TPresenter> : IModalView<TPresenter> where TPresenter : IDisposablePresenter
   {
      void AddSettingsView(IView settingsView);
   }

   public interface IPopulationSimulationSettingsView : ISimulationOutputSelectionView<IPopulationSimulationSettingsPresenter>
   {
   }

   public interface IIndividualSimulationSettingsView : ISimulationOutputSelectionView<IIndividualSimulationSettingsPresenter>
   {
   }
}