using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationEventsConfigurationView : IView<ISimulationEventsConfigurationPresenter>
   {
      void BindTo(IEnumerable<EventMappingDTO> allEventsMappingDTO);
      void RefreshData();
   }
}