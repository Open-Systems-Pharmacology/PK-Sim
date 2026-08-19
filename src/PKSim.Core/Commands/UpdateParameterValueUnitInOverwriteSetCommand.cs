using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

/// <summary>
///    Changes the display unit of a parameter value in an <see cref="OverwriteParameterSet" />. As for a parameter, the
///    displayed number is kept and the value in base unit is recalculated, so switching '2 mm' to cm yields '2 cm'.
/// </summary>
public class UpdateParameterValueUnitInOverwriteSetCommand : BuildingBlockChangeCommand<Compound>
{
   private static readonly NumericFormatter<double> _numericFormatter = new(NumericFormatterOptions.Instance);

   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;
   private readonly string _parameterPath;
   private readonly string _newUnitName;
   private string _oldUnitName;

   public UpdateParameterValueUnitInOverwriteSetCommand(
      OverwriteParameterSet overwriteParameterSet,
      Compound compound,
      string parameterPath,
      string newUnitName)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      _parameterPath = parameterPath;
      _newUnitName = newUnitName;
      CommandType = PKSimConstants.Command.CommandTypeEdit;
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);
      var parameterValue = _overwriteParameterSet.ParameterValueByPath(_parameterPath);
      var oldDisplayUnit = parameterValue.DisplayUnit;
      _oldUnitName = oldDisplayUnit.Name;

      var newDisplayUnit = parameterValue.Dimension.Unit(_newUnitName);
      var displayValue = parameterValue.ConvertToDisplayUnit(parameterValue.Value);

      parameterValue.DisplayUnit = newDisplayUnit;
      parameterValue.Value = parameterValue.Dimension.UnitValueToBaseUnitValue(newDisplayUnit, displayValue);

      Description = PKSimConstants.Command.UpdateParameterValueUnitInOverwriteParameterSet(_parameterPath, _numericFormatter.Format(displayValue),
         oldDisplayUnit.Name, newDisplayUnit.Name, _overwriteParameterSet.Name, _buildingBlock.Name);

      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
      new UpdateParameterValueUnitInOverwriteSetCommand(_overwriteParameterSet, _buildingBlock, _parameterPath, _oldUnitName).AsInverseFor(this);

   protected override void ClearReferences()
   {
      base.ClearReferences();
      _overwriteParameterSet = null;
   }

   public override void RestoreExecutionData(IExecutionContext context)
   {
      base.RestoreExecutionData(context);
      _overwriteParameterSet = context.Get<OverwriteParameterSet>(_overwriteParameterSetId);
   }
}
