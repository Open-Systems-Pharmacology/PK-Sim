using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class ChangeEnzymaticProcessMetaboliteNameCommand : BuildingBlockStructureChangeCommand
   {
      private EnzymaticProcess _process;
      private readonly string _newMetabolite;
      private readonly string _processId;
      private string _oldMetabolite;

      public ChangeEnzymaticProcessMetaboliteNameCommand(EnzymaticProcess process, string newMetabolite, IExecutionContext context)
      {
         _process = process;
         BuildingBlockId = context.BuildingBlockIdContaining(_process);
         _processId = process.Id;
         _newMetabolite = newMetabolite;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SetMetaboliteForProcess(_newMetabolite, process.Name, process.MetaboliteName);
         ObjectType = PKSimConstants.ObjectTypes.MetabolizingEnzyme;

         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(_process));
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _process = context.Get<EnzymaticProcess>(_processId);
      }

      protected override void ClearReferences()
      {
         _process = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new ChangeEnzymaticProcessMetaboliteNameCommand(_process, _oldMetabolite, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldMetabolite = _process.MetaboliteName;
         _process.MetaboliteName = _newMetabolite;
      }
   }
}