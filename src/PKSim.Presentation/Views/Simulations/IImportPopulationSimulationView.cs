
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IImportPopulationSimulationView : IModalView<IImportPopulationSimulationPresenter>
   {
      void BindTo(ImportPopulationSimulationDTO importPopulationSimulationDTO);
      bool ImportEnabled { get; set; }

      /// <summary>
      /// Specifies wether simulation file selection is visible or hidden from the user
      /// </summary>
      bool SimulationSelectionVisible { get; set; }
   }
}