using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Views.Observers
{
   public interface IObserverInfoView : IView<IObserverInfoPresenter>
   {
      void BindTo(ObserverDTO observerDTO);
      void Clear();
   }
}