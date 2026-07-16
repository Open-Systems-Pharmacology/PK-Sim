using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

public class ClearDefaultOverwriteParameterSetCommand : BuildingBlockChangeCommand<Compound>
{
   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;

   public ClearDefaultOverwriteParameterSetCommand(OverwriteParameterSet overwriteParameterSet, Compound compound)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      CommandType = PKSimConstants.Command.CommandTypeEdit;
      Description = PKSimConstants.Command.ClearDefaultOverwriteParameterSetInCompound(overwriteParameterSet.Name, compound.Name);
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);
      _overwriteParameterSet.IsDefault = false;
      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
   {
      return new SetDefaultOverwriteParameterSetCommand(_overwriteParameterSet, _buildingBlock).AsInverseFor(this);
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