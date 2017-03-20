using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetProteinMembraneLocationCommand : BuildingBlockStructureChangeCommand
   {
      private readonly MembraneLocation _newMembraneLocation;
      private readonly string _proteinId;
      private MembraneLocation _oldMembraneLocation;
      private PKSim.Core.Model.IndividualProtein _protein;

      public SetProteinMembraneLocationCommand(PKSim.Core.Model.IndividualProtein protein, MembraneLocation membraneLocation, IExecutionContext context)
      {
         _protein = protein;
         BuildingBlockId = context.BuildingBlockIdContaining(protein);
         _newMembraneLocation = membraneLocation;
         ObjectType = PKSimConstants.ObjectTypes.Protein;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _proteinId = _protein.Id;
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(_protein));
      }

      protected override void ClearReferences()
      {
         _protein = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _protein = context.Get<PKSim.Core.Model.IndividualProtein>(_proteinId);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetProteinMembraneLocationCommand(_protein, _oldMembraneLocation, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldMembraneLocation = _protein.MembraneLocation;
         _protein.MembraneLocation = _newMembraneLocation;
         Description = PKSimConstants.Command.SetProteinMembraneLocationDescription(_oldMembraneLocation.ToString(), _newMembraneLocation.ToString());
      }
   }
}