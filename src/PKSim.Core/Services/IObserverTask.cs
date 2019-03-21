using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IObserverTask
   {
      IPKSimCommand AddObserver(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock);
      IPKSimCommand RemoveObserver(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock);
   }
}