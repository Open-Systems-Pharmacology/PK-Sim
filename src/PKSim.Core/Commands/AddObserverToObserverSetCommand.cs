using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddObserverToObserverSetCommand : AddEntityToContainerCommand<IObserverBuilder, ObserverSet, AddObserverToObserverSetEvent>
   {
      public AddObserverToObserverSetCommand(IObserverBuilder observer, ObserverSet observerSet, IExecutionContext context)
         : base(observer, observerSet, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveObserverFromObserverSetCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}