using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

public class SetExtendedPropertyOnOverwriteSetCommand : BuildingBlockChangeCommand<Compound>
{
   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;
   private readonly string _propertyName;
   private readonly string _newValue;
   private string _oldValue;

   public SetExtendedPropertyOnOverwriteSetCommand(
      OverwriteParameterSet overwriteParameterSet,
      Compound compound,
      string propertyName,
      string newValue)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      _propertyName = propertyName;
      _newValue = newValue ?? string.Empty;
      CommandType = PKSimConstants.Command.CommandTypeEdit;
      Description = PKSimConstants.Command.SetExtendedPropertyOnOverwriteParameterSet(propertyName, overwriteParameterSet.Name, compound.Name);
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);

      _oldValue = _overwriteParameterSet.GetExtendedProperty(_propertyName);

      _overwriteParameterSet.SetExtendedProperty(_propertyName, _newValue);

      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
      new SetExtendedPropertyOnOverwriteSetCommand(_overwriteParameterSet, _buildingBlock, _propertyName, _oldValue).AsInverseFor(this);

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