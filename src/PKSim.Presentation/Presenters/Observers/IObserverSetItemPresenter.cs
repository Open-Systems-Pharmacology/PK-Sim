using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IObserverSetItemPresenter : ISubPresenter
   {
      void Edit(ObserverSet observerSet);
   }
}