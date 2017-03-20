using System.Collections.Generic;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundsSelectionView : IView<ISimulationCompoundsSelectionPresenter>, IResizableView
   {
      void BindTo(IEnumerable<CompoundSelectionDTO> compoundSelectionDTOs);

      /// <summary>
      /// Sets the error message for the compound selection
      /// </summary>
      /// <param name="errorText">The error message text. If null or empty, the error message view will be hidden</param>
      void SetError(string errorText);

      /// <summary>
      /// Hides the error message
      /// </summary>
      void HideError();
   }
}