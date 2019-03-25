using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddObserverToObserverBuildingBlockCommand : AddEntityToContainerCommand<IObserverBuilder, PKSimObserverBuildingBlock, AddObserverToObserverBuildingBlockEvent>
   {
      public AddObserverToObserverBuildingBlockCommand(IObserverBuilder observer, PKSimObserverBuildingBlock observerBuildingBlock, IExecutionContext context)
         : base(observer, observerBuildingBlock, context)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveObserverFromObserverBuildingBlockCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}