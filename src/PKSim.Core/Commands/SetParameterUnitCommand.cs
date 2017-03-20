using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Commands
{
   public class SetParameterUnitCommand : SetParameterValueCommand
   {
      private Unit _newDisplayUnit;
      protected Unit _oldDisplayUnit;
      private string _oldDisplayUnitName;

      public SetParameterUnitCommand(IParameter parameter, Unit newDisplayUnit)
         : base(parameter)
      {
         _newDisplayUnit = newDisplayUnit;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldDisplayUnit = _parameter.DisplayUnit;
         _oldDisplayUnitName = _oldDisplayUnit.Name;
         double oldDisplayValue = _parameter.Dimension.BaseUnitValueToUnitValue(_oldDisplayUnit, _parameter.Value);
         _valueToSet = _parameter.Dimension.UnitValueToBaseUnitValue(_newDisplayUnit, oldDisplayValue);

         base.PerformExecuteWith(context);
         Description = ParameterMessages.SetParameterUnit(_parameter, context.DisplayNameFor(_parameter), _oldDisplayUnit, _newDisplayUnit);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.DisplayUnit = _newDisplayUnit;
         base.UpdateParameter(parameter, context);
      }

      protected override void ResetParameter(IParameter parameter)
      {
         if (parameter == null) return;
         parameter.DisplayUnit = _newDisplayUnit;
         base.ResetParameter(parameter);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterUnitCommand(_parameter, _oldDisplayUnit).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _oldDisplayUnit = _parameter.Dimension.Unit(_oldDisplayUnitName);
      }

      protected override void ClearReferences()
      {
         base.ClearReferences();
         _newDisplayUnit = null;
         _oldDisplayUnit = null;
      }
   }
}