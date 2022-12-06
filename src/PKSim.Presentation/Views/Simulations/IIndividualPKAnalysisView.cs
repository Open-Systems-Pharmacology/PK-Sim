using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IIndividualPKAnalysisView : IView<IIndividualPKAnalysisPresenter>, IPKAnalysisView
   {
      bool ShowControls { set; }
      void BindTo(PKAnalysisDTO pkAnalysisDTO);
   }
}