using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using OSPSuite.Utility.Exceptions;

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
         var bbParameter = OriginParameterFor(_parameter, context);

         if (_parameter.IsFixedValue && _fixedValueSetHere)
         {
            //Inverse command need to reset the value as it was before the execution
            ResetParameter(_parameter);
            ResetParameter(bbParameter);
            _fixedValueSetHere = false;
         }
         else
         {
            if (_parameter.IsFixedValue == false)
               _fixedValueSetHere = true;

            UpdateParameter(_parameter, context);
            UpdateParameter(bbParameter, context);
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
         if (parameter == null) return;
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

         //for non formula parameter, it is necessary to update the value as well
         //if (parameter.Formula.IsAnImplementationOf<IExplici)
         parameter.Value = _valueToSet;

         parameter.IsFixedValue = false;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }

      public override void UpdateInternalFrom(IBuildingBlockChangeCommand buildingBlockChangeCommand)
      {
         base.UpdateInternalFrom(buildingBlockChangeCommand);
         var setParameterValueCommand = buildingBlockChangeCommand as SetParameterValueCommand;
         if (setParameterValueCommand == null) return;
         _fixedValueSetHere = setParameterValueCommand._fixedValueSetHere;
      }
   }
}