using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationObserversConfigurationView : IView<ISimulationObserversConfigurationPresenter>
   {
      void BindTo(IEnumerable<ObserverSetMappingDTO> allObserverSetMappingDTOs);
      void RefreshData();
   }
}