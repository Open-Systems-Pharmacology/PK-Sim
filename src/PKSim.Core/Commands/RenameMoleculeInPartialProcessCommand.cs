using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RenameMoleculeInPartialProcessCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newMoleculeName;
      private readonly string _partialPocessId;
      private string _oldMoleculeName;
      private PKSim.Core.Model.PartialProcess _partialProcess;

      public RenameMoleculeInPartialProcessCommand(PKSim.Core.Model.PartialProcess partialProcess, string newMoleculeName, IExecutionContext context)
      {
         _partialPocessId = partialProcess.Id;
         BuildingBlockId = context.BuildingBlockIdContaining(partialProcess);
         _newMoleculeName = newMoleculeName;
         _partialProcess = partialProcess;
         ObjectType = PKSimConstants.ObjectTypes.PartialProcess;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(partialProcess));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldMoleculeName = _partialProcess.MoleculeName;
         _partialProcess.MoleculeName = _newMoleculeName;
         _partialProcess.Name = CoreConstants.ContainerName.PartialProcessName(_partialProcess.MoleculeName, _partialProcess.DataSource);
         Description = PKSimConstants.Command.RenameEntityCommandDescripiton(PKSimConstants.UI.Molecule, _oldMoleculeName, _newMoleculeName);
         context.PublishEvent(new MoleculeRenamedInCompound(context.BuildingBlockContaining(_partialProcess).DowncastTo<Compound>()));
      }

      protected override void ClearReferences()
      {
         _partialProcess = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _partialProcess = context.Get<PKSim.Core.Model.PartialProcess>(_partialPocessId);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RenameMoleculeInPartialProcessCommand(_partialProcess, _oldMoleculeName, context).AsInverseFor(this);
      }
   }
}