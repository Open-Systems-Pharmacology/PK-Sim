using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Views.Observers
{
   public interface IImportObserversView : IView<IImportObserversPresenter>
   {
      void BindTo(IReadOnlyList<ImportObserverDTO> observers);
      void Rebind();
      void AddObserverView(IView view);
      void SelectObserver(ImportObserverDTO observerDTO);
   }
}