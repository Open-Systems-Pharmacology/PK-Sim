using System.Collections.Generic;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundProtocolEventView : IView<ISimulationCompoundProtocolEventPresenter>
   {
      void BindTo(IEnumerable<EventPlaceholderMappingDTO> eventMappings);
      void RefreshData();

      /// <summary>
      /// Sets the visibility of the event view
      /// </summary>
      bool EventVisible { get; set; }
   }
}
