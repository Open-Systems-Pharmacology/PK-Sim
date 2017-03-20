using System.Collections.Generic;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundInteractionSelectionView : IView<ISimulationCompoundInteractionSelectionPresenter>
   {
      void BindTo(IEnumerable<SimulationInteractionProcessSelectionDTO> allPartialProcessesDTO);
      bool WarningVisible { get; set; }
      string Warning { get; set; }
   }
}