using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IGlobalPKAnalysisView : IView<IGlobalPKAnalysisPresenter>
   {
      void BindTo(GlobalPKAnalysisDTO globalPKAnalysisDTO);
   }
}