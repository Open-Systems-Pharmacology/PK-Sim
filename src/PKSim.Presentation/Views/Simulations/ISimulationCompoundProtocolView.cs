using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundProtocolView : IView<ISimulationCompoundProtocolPresenter>, IResizableWithDefaultHeightView
   {
      void BindTo(ProtocolSelectionDTO protocolSelectionDTO);
      void AddFormulationMappingView(IView view);
      bool AllowEmptyProtocolSelection { get;set; }
   }
}