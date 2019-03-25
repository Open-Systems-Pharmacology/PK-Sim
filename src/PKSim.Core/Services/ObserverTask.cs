using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class ObserverTask : IObserverTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly IObserverLoader _observerLoader;
      private readonly IObjectIdResetter _objectIdResetter;

      public ObserverTask(IExecutionContext executionContext, IObserverLoader observerLoader, IObjectIdResetter objectIdResetter)
      {
         _executionContext = executionContext;
         _observerLoader = observerLoader;
         _objectIdResetter = objectIdResetter;
      }

      public IPKSimCommand AddObserver(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock)
      {
         return new AddObserverToObserverBuildingBlockCommand(observer, observerBuildingBlock, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand RemoveObserver(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock)
      {
         return new RemoveObserverFromObserverBuildingBlockCommand(observer, observerBuildingBlock, _executionContext).Run(_executionContext);
      }

      public IObserverBuilder LoadObserverFrom(string pkmlFileFullPath)
      {
         var observer = _observerLoader.Load(pkmlFileFullPath);
         _objectIdResetter.ResetIdFor(observer);
         return observer;
      }
   }
}