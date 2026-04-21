using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands;

public class RemoveParameterValueFromOverwriteSetCommand : BuildingBlockChangeCommand<Compound>
{
   private readonly string _overwriteParameterSetId;
   private OverwriteParameterSet _overwriteParameterSet;
   private readonly string _parameterPath;
   private ParameterValue _removedParameterValue;

   public RemoveParameterValueFromOverwriteSetCommand(
      OverwriteParameterSet overwriteParameterSet,
      Compound compound,
      string parameterPath)
      : base(compound)
   {
      _overwriteParameterSet = overwriteParameterSet;
      _overwriteParameterSetId = overwriteParameterSet.Id;
      _parameterPath = parameterPath;
      CommandType = PKSimConstants.Command.CommandTypeDelete;
      Description = PKSimConstants.Command.RemoveParameterValueFromOverwriteParameterSet(parameterPath, overwriteParameterSet.Name, compound.Name);
   }

   protected override void PerformExecuteWith(IExecutionContext context)
   {
      base.PerformExecuteWith(context);
      _removedParameterValue = _overwriteParameterSet.ParameterValueByPath(_parameterPath);
      _overwriteParameterSet.RemoveByPath(_parameterPath);
      context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
      new AddParameterValueToOverwriteSetCommand(_overwriteParameterSet, _buildingBlock, _removedParameterValue).AsInverseFor(this);

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

   /// <summary>
   ///    Internal inverse of <see cref="RemoveParameterValueFromOverwriteSetCommand" />.
   ///    Not public: new parameter values are only added to an <see cref="OverwriteParameterSet" />
   ///    via commit from a simulation, never directly from the compound tab.
   /// </summary>
   private class AddParameterValueToOverwriteSetCommand : BuildingBlockChangeCommand<Compound>
   {
      private readonly string _overwriteParameterSetId;
      private OverwriteParameterSet _overwriteParameterSet;
      private readonly ParameterValue _parameterValue;

      public AddParameterValueToOverwriteSetCommand(OverwriteParameterSet overwriteParameterSet, Compound compound, ParameterValue parameterValue)
         : base(compound)
      {
         _overwriteParameterSet = overwriteParameterSet;
         _overwriteParameterSetId = overwriteParameterSet.Id;
         _parameterValue = parameterValue;
         CommandType = PKSimConstants.Command.CommandTypeAdd;
         Description = PKSimConstants.Command.AddParameterValueToOverwriteParameterSet(parameterValue.Path.PathAsString, overwriteParameterSet.Name, compound.Name);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         _overwriteParameterSet.Add(_parameterValue);
         context.PublishEvent(new OverwriteParameterSetChangedEvent(_buildingBlock, _overwriteParameterSet));
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
         new RemoveParameterValueFromOverwriteSetCommand(_overwriteParameterSet, _buildingBlock, _parameterValue.Path.PathAsString).AsInverseFor(this);

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
}
