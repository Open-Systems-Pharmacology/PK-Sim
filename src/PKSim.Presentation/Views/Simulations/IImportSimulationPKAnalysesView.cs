
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IImportSimulationPKAnalysesView : IModalView<IImportSimulationPKAnalysesPresenter>
   {
      /// <summary>
      ///    Specifiies whether the view import task is running or not
      /// </summary>
      bool ImportingResults { get; set; }

      void BindTo(ImportPKAnalysesDTO importPKAnalysesDTO);

      string Description { get; set; }
   }
}