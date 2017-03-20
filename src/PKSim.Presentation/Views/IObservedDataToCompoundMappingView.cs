using System.Collections.Generic;
using PKSim.Presentation.DTO.ObservedData;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IObservedDataToCompoundMappingView : IModalView<IObservedDataToCompoundMappingPresenter>
   {
      void BindTo(IEnumerable<ObservedDataToCompoundMappingDTO> observedDataToCompoundMappingDtos);
   }
}