using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveParameterAlternativeCommand : RemoveEntityFromContainerCommand<PKSim.Core.Model.ParameterAlternative, PKSim.Core.Model.ParameterAlternativeGroup, RemoveCompoundParameterGroupAlternativeEvent>
   {
      public RemoveParameterAlternativeCommand(PKSim.Core.Model.ParameterAlternative parameterAlternative, PKSim.Core.Model.ParameterAlternativeGroup parameterGroup, IExecutionContext context)
         : base(parameterAlternative, parameterGroup, context)
      {
         Description = PKSimConstants.Command.RemoveCompoundParameterGroupAlternativeDescription(parameterAlternative.Name, context.DisplayNameFor(parameterGroup));
         ObjectType = PKSimConstants.ObjectTypes.Compound;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddParameterAlternativeCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}