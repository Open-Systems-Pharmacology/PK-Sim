using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
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

      _oldValue = currentValue();

      if (string.IsNullOrEmpty(_newValue))
      {
         if (_overwriteParameterSet.ExtendedProperties.Contains(_propertyName))
            _overwriteParameterSet.ExtendedProperties.Remove(_propertyName);
      }
      else if (_overwriteParameterSet.ExtendedProperties.Contains(_propertyName))
      {
         _overwriteParameterSet.ExtendedProperties[_propertyName].ValueAsObject = _newValue;
      }
      else
      {
         _overwriteParameterSet.ExtendedProperties.Add(new ExtendedProperty<string> { Name = _propertyName, Value = _newValue });
      }

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

   private string currentValue()
   {
      if (!_overwriteParameterSet.ExtendedProperties.Contains(_propertyName))
         return string.Empty;

      return _overwriteParameterSet.ExtendedProperties[_propertyName].ValueAsObject?.ToString() ?? string.Empty;
   }
}
