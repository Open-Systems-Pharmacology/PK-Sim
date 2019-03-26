using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IObserverSetTask : IBuildingBlockTask<ObserverSet>
   {
      ObserverSetMapping CreateObserverSetMapping();
      ObserverSetMapping CreateObserverSetMapping(ObserverSet observerSet);
   }
}