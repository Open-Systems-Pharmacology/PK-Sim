using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class RemoveParameterFromContainerCommand : RemoveEntityFromContainerCommand<IParameter, IContainer, RemoveParameterFromContainerEvent>
   {
      public RemoveParameterFromContainerCommand(IParameter parameterToRemove, IContainer parentContainer, IExecutionContext context)
         : base(parameterToRemove, parentContainer, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddParameterToContainerCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}