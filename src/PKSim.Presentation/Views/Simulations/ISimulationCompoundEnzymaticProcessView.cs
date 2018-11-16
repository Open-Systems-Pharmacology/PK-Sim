using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundEnzymaticProcessView : ISimulationCompoundProcessView<EnzymaticProcess, SimulationEnzymaticProcessSelectionDTO>
   {
      void HideMultipleCompoundsColumns();
   }
}