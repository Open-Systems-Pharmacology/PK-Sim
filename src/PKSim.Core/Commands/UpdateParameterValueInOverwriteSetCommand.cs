using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

public class UpdateParameterValueInOverwriteSetCommand : BuildingBlockChangeCommand<Compound>
{
   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;
   private readonly string _parameterPath;
   private readonly double _newValue;
   private double? _oldValue;

   public UpdateParameterValueInOverwriteSetCommand(
      OverwriteParameterSet overwriteParameterSet,
      Compound compound,
      string parameterPath,
      double newValue)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      _parameterPath = parameterPath;
      _newValue = newValue;
      CommandType = PKSimConstants.Command.CommandTypeEdit;
      Description = PKSimConstants.Command.UpdateParameterValueInOverwriteParameterSet(parameterPath, overwriteParameterSet.Name, compound.Name);
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);
      var parameterValue = _overwriteParameterSet.ParameterValueByPath(_parameterPath);
      _oldValue = parameterValue.Value;
      parameterValue.Value = _newValue;
      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
   {
      return new UpdateParameterValueInOverwriteSetCommand(_overwriteParameterSet, _buildingBlock, _parameterPath, _oldValue.GetValueOrDefault()).AsInverseFor(this);
   }

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
