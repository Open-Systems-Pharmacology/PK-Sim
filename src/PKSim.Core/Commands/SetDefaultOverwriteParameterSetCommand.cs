using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

public class SetDefaultOverwriteParameterSetCommand : BuildingBlockChangeCommand<Compound>
{
   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;
   private string _previousDefaultId;
   private OverwriteParameterSet _previousDefault;

   public SetDefaultOverwriteParameterSetCommand(OverwriteParameterSet overwriteParameterSet, Compound compound)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      CommandType = PKSimConstants.Command.CommandTypeEdit;
      Description = PKSimConstants.Command.SetDefaultOverwriteParameterSetInCompound(overwriteParameterSet.Name, compound.Name);
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);

      _previousDefault = _buildingBlock.OverwriteParameterSets.FirstOrDefault(x => x.IsDefault && !ReferenceEquals(x, _overwriteParameterSet));
      _previousDefaultId = _previousDefault?.Id;

      _buildingBlock.OverwriteParameterSets.Each(x => x.IsDefault = false);
      _overwriteParameterSet.IsDefault = true;

      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
   {
      if (_previousDefault != null)
         return new SetDefaultOverwriteParameterSetCommand(_previousDefault, _buildingBlock).AsInverseFor(this);

      return new ClearDefaultOverwriteParameterSetCommand(_overwriteParameterSet, _buildingBlock).AsInverseFor(this);
   }

   protected override void ClearReferences()
   {
      base.ClearReferences();
      _overwriteParameterSet = null;
      _previousDefault = null;
   }

   public override void RestoreExecutionData(IExecutionContext context)
   {
      base.RestoreExecutionData(context);
      _overwriteParameterSet = context.Get<OverwriteParameterSet>(_overwriteParameterSetId);
      if (_previousDefaultId != null)
         _previousDefault = context.Get<OverwriteParameterSet>(_previousDefaultId);
   }
}