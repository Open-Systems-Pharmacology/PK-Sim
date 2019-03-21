using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IObserverBuildingBlockTask : IBuildingBlockTask<PKSimObserverBuildingBlock>
   {
      IObserverBuilder LoadObserverFrom(string fileName);

      PKSimObserverBuildingBlock Create();
   }
}