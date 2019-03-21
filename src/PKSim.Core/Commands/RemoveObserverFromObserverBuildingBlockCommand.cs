using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveObserverFromObserverBuildingBlockCommand : RemoveEntityFromContainerCommand<IObserverBuilder, PKSimObserverBuildingBlock, RemoveObserverFromObserverBuildingBlockEvent>
   {
      public RemoveObserverFromObserverBuildingBlockCommand(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock, IExecutionContext context)
         : base(observer, observerBuildingBlock, context)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddObserverToObserverBuildingBlockCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}