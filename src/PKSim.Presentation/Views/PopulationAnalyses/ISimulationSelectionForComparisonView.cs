using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface ISimulationSelectionForComparisonView : IModalView<ISimulationSelectionForComparisonPresenter>
   {
      void BindTo(SimulationComparisonSelectionDTO simulationComparisonSelectionDTO);
   }
}