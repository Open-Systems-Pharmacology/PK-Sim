using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class ChangeCompoundProcessInternalNameCommand : BuildingBlockChangeCommand
   {
      private readonly string _internalName;
      private readonly string _oldInternalName;
      private readonly string _processId;
      private CompoundProcess _compoundProcess;

      public ChangeCompoundProcessInternalNameCommand(CompoundProcess compoundProcess, string internalName, IExecutionContext context)
      {
         _compoundProcess = compoundProcess;
         _internalName = internalName;
         _processId = compoundProcess.Id;
         _oldInternalName = _compoundProcess.InternalName;
         BuildingBlockId = context.BuildingBlockIdContaining(_compoundProcess);
         //This command is necessary to insure consitency but does not need to be seen
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(_compoundProcess));
         Visible = false;
      }

      protected override void ClearReferences()
      {
         _compoundProcess = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _compoundProcess = context.Get<PartialProcess>(_processId);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new ChangeCompoundProcessInternalNameCommand(_compoundProcess, _oldInternalName, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _compoundProcess.InternalName = _internalName;
      }
   }
}