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
      private bool _oldOriginIsDefault;
      private ValueOrigin _oldValueOrigin;

      public ResetParameterCommand(IParameter parameter) : base(parameter)
      {
      }

      protected override void ExecuteUpdateParameter(IParameter parameter, IExecutionContext context)
      {
         _oldValue = _parameter.Value;
         _oldIsDefault = _parameter.IsDefault;
         _oldOriginIsDefault = OriginParameterFor(_parameter, context)?.IsDefault ?? _oldIsDefault;
         _oldValueOrigin = _parameter.ValueOrigin.Clone();
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
         //inverse of a reset command set the previous value, default state and value origin back into the parameter
         return new RestoreParameterValueCommand(_parameter, _oldValue, _oldIsDefault, _oldOriginIsDefault, _oldValueOrigin).AsInverseFor(this);
      }

      /// <summary>
      ///    Sets the value the parameter had before the reset and restores the default state of the parameter and of its
      ///    origin parameter as well as the value origin. Its own inverse resets the parameter again.
      /// </summary>
      private class RestoreParameterValueCommand : SetParameterValueCommand
      {
         private readonly bool _isDefault;
         private readonly bool _originIsDefault;
         private readonly ValueOrigin _valueOrigin;

         public RestoreParameterValueCommand(IParameter parameter, double valueToSet, bool isDefault, bool originIsDefault, ValueOrigin valueOrigin) : base(parameter, valueToSet)
         {
            _isDefault = isDefault;
            _originIsDefault = originIsDefault;
            _valueOrigin = valueOrigin;
         }

         protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
         {
            base.UpdateParameter(parameter, context);
            if (parameter == null)
               return;

            parameter.IsDefault = ReferenceEquals(parameter, _parameter) ? _isDefault : _originIsDefault;
            parameter.UpdateValueOriginFrom(_valueOrigin);
         }

         protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
            new ResetParameterCommand(_parameter).AsInverseFor(this);
      }
   }
}