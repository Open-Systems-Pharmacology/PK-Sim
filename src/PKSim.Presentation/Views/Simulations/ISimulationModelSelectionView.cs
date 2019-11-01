using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationModelSelectionView : IView<ISimulationModelSelectionPresenter>
   {
      void UpdateModelImage(string imageName);
      void BindTo(ModelConfigurationDTO modelConfigurationDTO);
   }
}