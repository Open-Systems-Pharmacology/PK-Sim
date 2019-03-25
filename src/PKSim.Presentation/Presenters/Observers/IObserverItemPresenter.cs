using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IObserverItemPresenter : ISubPresenter
   {
      void Edit(PKSimObserverBuildingBlock observerBuildingBlock);
   }
}