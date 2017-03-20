
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IImportSimulationResultsView : IModalView<IImportSimulationResultsPresenter>
   {
      /// <summary>
      ///    Specifiies whether the view import task is running or not
      /// </summary>
      bool ImportingResults { get; set; }

      void BindTo(ImportSimulationResultsDTO importSimulationResultsDTO);

      /// <summary>
      /// Is the import button enabled?
      /// </summary>
      bool ImportEnabled { get; set; }
   }
}