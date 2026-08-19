using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

public class UpdateValueOriginInOverwriteSetCommand : BuildingBlockChangeCommand<Compound>
{
   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;
   private readonly string _parameterPath;
   private ValueOrigin _valueOrigin;
   private ValueOrigin _oldValueOrigin;

   public UpdateValueOriginInOverwriteSetCommand(
      OverwriteParameterSet overwriteParameterSet,
      Compound compound,
      string parameterPath,
      ValueOrigin valueOrigin)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      _parameterPath = parameterPath;
      _valueOrigin = valueOrigin;
      CommandType = PKSimConstants.Command.CommandTypeEdit;
      Description = PKSimConstants.Command.UpdateValueOriginInOverwriteParameterSet(parameterPath, overwriteParameterSet.Name, compound.Name);
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);
      var parameterValue = _overwriteParameterSet.ParameterValueByPath(_parameterPath);
      _oldValueOrigin = parameterValue.ValueOrigin.Clone();
      parameterValue.UpdateValueOriginFrom(_valueOrigin);
      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
      new UpdateValueOriginInOverwriteSetCommand(_overwriteParameterSet, _buildingBlock, _parameterPath, _oldValueOrigin).AsInverseFor(this);

   protected override void ClearReferences()
   {
      base.ClearReferences();
      _overwriteParameterSet = null;
      _valueOrigin = null;
   }

   public override void RestoreExecutionData(IExecutionContext context)
   {
      base.RestoreExecutionData(context);
      _overwriteParameterSet = context.Get<OverwriteParameterSet>(_overwriteParameterSetId);
   }
}
