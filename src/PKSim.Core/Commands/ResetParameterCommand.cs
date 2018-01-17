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
         SaveValueOriginFor(_parameter);

         //do not update value origin automatically when resetting a parameter
         UpdateParameter(context, updateValueOrigin: false);
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
         //TODO MBD: That does not work quite yet
         var parametersInContainerRepository = context.Resolve<IParametersInContainerRepository>();

         var parameterMetaData = parametersInContainerRepository.ParameterMetaDataFor(parameter);

         if (parameterMetaData?.ValueOrigin == null)
            return;

         if (!parameterMetaData.ValueOrigin.Default)
            return;

         parameter.ValueOrigin.UpdateFrom(parameterMetaData.ValueOrigin);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         //inverse of a reset command set the previous value back into the parameter
         return new SetParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}