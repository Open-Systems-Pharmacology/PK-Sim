using System.Collections.Generic;

using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationEventsConfigurationView : IView<ISimulationEventsConfigurationPresenter>
   {
      void BindTo(IEnumerable<EventMappingDTO> allEventsMappingDTO);
      void RefreshData();
   }
}