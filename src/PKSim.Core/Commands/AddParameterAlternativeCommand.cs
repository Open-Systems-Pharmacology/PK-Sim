using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddParameterAlternativeCommand : AddEntityToContainerCommand<ParameterAlternative, ParameterAlternativeGroup, AddCompoundParameterGroupAlternativeEvent>
   {
      public AddParameterAlternativeCommand(ParameterAlternative parameterAlternative, ParameterAlternativeGroup parameterGroup, IExecutionContext context)
         : base(parameterAlternative, parameterGroup, context)
      {
         Description = PKSimConstants.Command.AddCompoundParameterGroupAlternativeDescription(parameterAlternative.Name, context.DisplayNameFor(parameterGroup));
         ObjectType = PKSimConstants.ObjectTypes.Compound;
         ShouldChangeVersion = false;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveParameterAlternativeCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}