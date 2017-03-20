using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddParameterAlternativeCommand : AddEntityToContainerCommand<PKSim.Core.Model.ParameterAlternative, PKSim.Core.Model.ParameterAlternativeGroup, AddCompoundParameterGroupAlternativeEvent>
   {
      public AddParameterAlternativeCommand(PKSim.Core.Model.ParameterAlternative parameterAlternative, PKSim.Core.Model.ParameterAlternativeGroup parameterGroup, IExecutionContext context)
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