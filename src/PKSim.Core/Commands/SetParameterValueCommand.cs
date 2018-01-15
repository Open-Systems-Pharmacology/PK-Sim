using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetParameterValueCommand : EditParameterCommand
   {
      private bool _fixedValueSetHere;
      protected double _oldValue;
      protected double _valueToSet;

      public SetParameterValueCommand(IParameter parameter, double valueToSet)
         : base(parameter)
      {
         _valueToSet = valueToSet;
      }

      protected SetParameterValueCommand(IParameter parameter)
         : base(parameter)
      {
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _oldValue = retrieveValue();


         if (_parameter.IsFixedValue && _fixedValueSetHere)
         {
            //Inverse command need to reset the value as it was before the execution
            var bbParameter = OriginParameterFor(_parameter, context);
            ResetParameter(_parameter);
            ResetParameter(bbParameter);
            UpdateValueOriginInParameters(_parameter, bbParameter);
            _fixedValueSetHere = false;
         }
         else
         {
            if (_parameter.IsFixedValue == false)
               _fixedValueSetHere = true;

            UpdateParameter(context);
         }


         Description = ParameterMessages.SetParameterValue(_parameter, context.DisplayNameFor(_parameter), _oldValue, _valueToSet);
      }

      private double retrieveValue()
      {
         try
         {
            return _parameter.Value;
         }
         catch (OSPSuiteException)
         {
            return double.NaN;
         }
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null)
            return;

         parameter.Value = _valueToSet;

         //only parameters set by the user should be set as IsFixed = true. Otherwise
         //these parameters are considered inputs
         if (parameter.BuildingBlockType != PKSimBuildingBlockType.Simulation)
            return;

         parameter.IsFixedValue = true;
      }


      protected virtual void ResetParameter(IParameter parameter)
      {
         if (parameter == null) return;

         //Update value before resetting fixed value flag to ensure consistency
         parameter.Value = _valueToSet;
         parameter.IsFixedValue = false;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }

      public override void UpdateInternalFrom(IBuildingBlockChangeCommand originalCommand)
      {
         base.UpdateInternalFrom(originalCommand);
         var setParameterValueCommand = originalCommand as SetParameterValueCommand;
         if (setParameterValueCommand == null) return;
         _fixedValueSetHere = setParameterValueCommand._fixedValueSetHere;
      }
   }
}