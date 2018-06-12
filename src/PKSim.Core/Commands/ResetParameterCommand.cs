using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Commands
{
   public class ResetParameterCommand : EditParameterCommand
   {
      private double _oldValue;

      public ResetParameterCommand(IParameter parameter) : base(parameter)
      {
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _oldValue = _parameter.Value;
         UpdateParameter(context);
         Description = ParameterMessages.ResetParameterValue(_parameter, context.DisplayNameFor(_parameter), _oldValue);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.ResetToDefault();
         resetValueOriginForDefaultParameter(parameter, context);
      }

      private void resetValueOriginForDefaultParameter(IParameter parameter, IExecutionContext context)
      {
         var valueOriginRepository = context.Resolve<IValueOriginRepository>();

         var valueOrigin = valueOriginRepository.ValueOriginFor(parameter);
         parameter.UpdateValueOriginFrom(valueOrigin);

         //reset only available for trully default parameter with a default value
         parameter.IsDefault = true;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         //inverse of a reset command set the previous value back into the parameter
         return new SetParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}