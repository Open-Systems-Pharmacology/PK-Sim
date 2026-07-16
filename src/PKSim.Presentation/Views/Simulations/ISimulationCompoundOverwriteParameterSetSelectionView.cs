using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations;

public interface ISimulationCompoundOverwriteParameterSetSelectionView : IView<ISimulationCompoundOverwriteParameterSetSelectionPresenter>, IResizableView
{
   void BindTo(SimulationCompoundOverwriteParameterSetSelectionDTO dto);
}