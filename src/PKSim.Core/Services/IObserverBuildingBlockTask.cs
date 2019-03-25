using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IObserverBuildingBlockTask : IBuildingBlockTask<PKSimObserverBuildingBlock>
   {
      PKSimObserverBuildingBlock Create();
   }
}