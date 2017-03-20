using System.Collections.Generic;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundParameterAlternativesSelectionView : IView<ISimulationCompoundParameterAlternativesSelectionPresenter>, IResizableView
   {
      void BindTo(IEnumerable<CompoundParameterSelectionDTO> compoundParameterMappingDTO);
   }
}