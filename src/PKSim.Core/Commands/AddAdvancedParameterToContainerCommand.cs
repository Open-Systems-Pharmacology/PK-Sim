using System;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Commands.Core;

namespace PKSim.Core.Commands
{
   public class AddAdvancedParameterToContainerCommand : AddEntityToContainerCommand<IAdvancedParameter, IAdvancedParameterContainer, AddAdvancedParameterToContainerEvent>
   {
      public AddAdvancedParameterToContainerCommand(IAdvancedParameter entityToAdd, IAdvancedParameterContainer parentContainer, IExecutionContext context)
         : base(entityToAdd, parentContainer, context, addAdvancedParameter)
      {
      }

      private static Action<IAdvancedParameter> addAdvancedParameter(IAdvancedParameterContainer advancedParameterContainer)
      {
         return p => advancedParameterContainer.AddAdvancedParameter(p);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveAdvancedParameterFromContainerCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}