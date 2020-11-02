using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class AddParameterToContainerCommand : AddEntityToContainerCommand<IParameter, IContainer, AddParameterToContainerEvent>
   {
      public AddParameterToContainerCommand(IParameter parameterToAdd, IContainer parentContainer, IExecutionContext context)
         : base(parameterToAdd, parentContainer, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveParameterFromContainerCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}