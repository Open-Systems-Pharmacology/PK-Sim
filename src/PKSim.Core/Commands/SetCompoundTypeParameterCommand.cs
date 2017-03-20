using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetCompoundTypeParameterCommand : SetParameterValueCommand
   {
      public SetCompoundTypeParameterCommand(IParameter parameter, CompoundType compoundType)
         : base(parameter, (double)compoundType)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         Description = PKSimConstants.Command.SetCompoundTypeParameterDescription(context.DisplayNameFor(_parameter), ((CompoundType) _oldValue).ToString(), ((CompoundType) _valueToSet).ToString());
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetCompoundTypeParameterCommand(_parameter, (CompoundType) _oldValue).AsInverseFor(this);
      }
   }
}