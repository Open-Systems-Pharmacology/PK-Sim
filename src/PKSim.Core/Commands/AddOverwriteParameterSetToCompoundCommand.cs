using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddOverwriteParameterSetToCompoundCommand : BuildingBlockChangeCommand<Compound>
   {
      private OverwriteParameterSet _overwriteParameterSet;
      private readonly string _overwriteParameterSetId;

      public AddOverwriteParameterSetToCompoundCommand(OverwriteParameterSet overwriteParameterSet, Compound compound)
         : base(compound)
      {
         _overwriteParameterSet = overwriteParameterSet;
         _overwriteParameterSetId = overwriteParameterSet.Id;
         CommandType = PKSimConstants.Command.CommandTypeAdd;
         Description = PKSimConstants.Command.AddEntityToContainer(PKSimConstants.ObjectTypes.OverwriteParameterSet, overwriteParameterSet.Name, PKSimConstants.ObjectTypes.Compound, compound.Name);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         _buildingBlock.AddOverwriteParameterSet(_overwriteParameterSet);
         context.Register(_overwriteParameterSet);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveOverwriteParameterSetFromCompoundCommand(_overwriteParameterSet, _buildingBlock).AsInverseFor(this);
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
}