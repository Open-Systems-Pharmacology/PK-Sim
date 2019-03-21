using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class ObserverTask : IObserverTask
   {
      private readonly IExecutionContext _executionContext;

      public ObserverTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public IPKSimCommand AddObserver(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock)
      {
         return new AddObserverToObserverBuildingBlockCommand(observer, observerBuildingBlock, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand RemoveObserver(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock)
      {
         return new RemoveObserverFromObserverBuildingBlockCommand(observer, observerBuildingBlock, _executionContext).Run(_executionContext);
      }
   }
}