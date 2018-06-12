using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetParameterDisplayUnitCommand : EditParameterCommand
   {
      private readonly string _oldDisplayUnitName;
      private Unit _displayUnit;
      private Unit _oldDisplayUnit;

      public SetParameterDisplayUnitCommand(IParameter parameter, Unit displayUnit)
         : base(parameter)
      {
         _displayUnit = displayUnit;
         _oldDisplayUnit = parameter.DisplayUnit;
         _oldDisplayUnitName = _oldDisplayUnit.Name;
      }

      protected override void ClearReferences()
      {
         base.ClearReferences();
         _displayUnit = null;
         _oldDisplayUnit = null;
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         UpdateParameter(context);
         Description = ParameterMessages.SetParameterDisplayUnit(context.DisplayNameFor(_parameter), _oldDisplayUnit, _displayUnit);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _oldDisplayUnit = _parameter.Dimension.Unit(_oldDisplayUnitName);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterDisplayUnitCommand(_parameter, _oldDisplayUnit).AsInverseFor(this);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.DisplayUnit = _displayUnit;
      }
   }
}