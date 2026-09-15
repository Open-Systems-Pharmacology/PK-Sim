using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Commands
{
   public class ResetParameterCommand : EditParameterCommand
   {
      protected double _oldValue;
      private bool _oldIsDefault;

      public ResetParameterCommand(IParameter parameter) : base(parameter)
      {
      }

      protected override void ExecuteUpdateParameter(IParameter parameter, IExecutionContext context)
      {
         _oldValue = _parameter.Value;
         _oldIsDefault = _parameter.IsDefault;
         UpdateParameter(context);
         Description = ParameterMessages.ResetParameterValue(parameter, context.DisplayNameFor(parameter), _oldValue);
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

         //reset only available for truly default parameter with a default value
         parameter.IsDefault = true;
      }

      protected override void UpdateTrackerForParameter(SimulationParameterChangeTracker tracker, string parameterPath, bool isOverwrittenParameterPath)
      {
         if (isOverwrittenParameterPath)
            Track(tracker, parameterPath);
         else
            UnTrack(tracker, parameterPath);
      }

      protected override void ReverseTrackerUpdateForParameter(SimulationParameterChangeTracker tracker, string parameterPath, bool isOverwrittenParameterPath)
      {
         if (isOverwrittenParameterPath)
            UnTrack(tracker, parameterPath);
         else
            Track(tracker, parameterPath);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         //inverse of a reset command set the previous value and default state back into the parameter
         return new RestoreParameterValueCommand(_parameter, _oldValue, _oldIsDefault).AsInverseFor(this);
      }

      /// <summary>
      ///    Sets the value the parameter had before the reset and restores its default state. Its own inverse resets the
      ///    parameter again.
      /// </summary>
      private class RestoreParameterValueCommand : SetParameterValueCommand
      {
         private readonly bool _isDefault;

         public RestoreParameterValueCommand(IParameter parameter, double valueToSet, bool isDefault) : base(parameter, valueToSet)
         {
            _isDefault = isDefault;
         }

         protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
         {
            base.UpdateParameter(parameter, context);
            if (parameter == null)
               return;

            parameter.IsDefault = _isDefault;
         }

         protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
            new ResetParameterCommand(_parameter).AsInverseFor(this);
      }
   }
}