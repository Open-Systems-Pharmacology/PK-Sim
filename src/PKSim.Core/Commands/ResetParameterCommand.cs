using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class ResetParameterCommand : EditParameterCommand
   {
      private double _oldValue;

      public ResetParameterCommand(IParameter parameterToReset) : base(parameterToReset)
      {
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _oldValue = _parameter.Value;
         UpdateParameter(_parameter, context);
         UpdateParameter(OriginParameterFor(_parameter, context), context);
         Description = ParameterMessages.ResetParameterValue(_parameter, context.DisplayNameFor(_parameter), _oldValue);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.ResetToDefault();
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         //inverse of a reset command set the previous value back into the parameter
         return new SetParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}