using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetProteinIntracellularVascularEndoLocationCommand : BuildingBlockStructureChangeCommand
   {
      private readonly IntracellularVascularEndoLocation _newVascularEndoLocation;
      private readonly string _proteinId;
      private IntracellularVascularEndoLocation _oldVascularEndoLocation;
      private IndividualProtein _protein;

      public SetProteinIntracellularVascularEndoLocationCommand(IndividualProtein protein, IntracellularVascularEndoLocation newVascularEndoLocation, IExecutionContext context)
      {
         _protein = protein;
         _newVascularEndoLocation = newVascularEndoLocation;
         BuildingBlockId = context.BuildingBlockIdContaining(protein);
         ObjectType = PKSimConstants.ObjectTypes.Protein;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _proteinId = _protein.Id;
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(_protein));
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _protein = context.Get<IndividualProtein>(_proteinId);
      }

      protected override void ClearReferences()
      {
         _protein = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetProteinIntracellularVascularEndoLocationCommand(_protein, _oldVascularEndoLocation, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldVascularEndoLocation = _protein.IntracellularVascularEndoLocation;
         _protein.IntracellularVascularEndoLocation = _newVascularEndoLocation;
         Description = PKSimConstants.Command.SetProteinMembraneLocationDescription(_oldVascularEndoLocation.ToString(), _newVascularEndoLocation.ToString());
      }
   }
}