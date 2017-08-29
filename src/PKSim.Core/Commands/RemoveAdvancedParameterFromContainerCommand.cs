using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveAdvancedParameterFromContainerCommand : RemoveEntityFromContainerCommand<AdvancedParameter, IAdvancedParameterContainer, RemoveAdvancedParameterFromContainerEvent>
   {
      public RemoveAdvancedParameterFromContainerCommand(AdvancedParameter advancedParameter, IAdvancedParameterContainer advancedParameterContainer, IExecutionContext context)
         : base(advancedParameter, advancedParameterContainer, context, x => x.RemoveAdvancedParameter)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddAdvancedParameterToContainerCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}