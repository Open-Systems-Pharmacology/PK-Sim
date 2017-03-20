using System.Collections.Generic;

using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundProtocolFormulationView : IView<ISimulationCompoundProtocolFormulationPresenter>
   {
      void BindTo(IEnumerable<FormulationMappingDTO> formulationMappings);
      void RefreshData();

      /// <summary>
      /// set the visibility of the column containg the formulation key
      /// </summary>
      bool FormulationKeyVisible { set; }

      /// <summary>
      /// Sets the visibility of the formulation view
      /// </summary>
      bool FormulationVisible { get; set; }
   }
}