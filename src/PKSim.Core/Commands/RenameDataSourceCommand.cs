using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RenameDataSourceCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newDataSource;
      private CompoundProcess _compoundProcess;
      private readonly string _processId;
      private string _oldDataSource;

      public RenameDataSourceCommand(CompoundProcess compoundProcess, string newDataSource, IExecutionContext context)
      {
         _processId = compoundProcess.Id;
         BuildingBlockId = context.BuildingBlockIdContaining(compoundProcess);
         _newDataSource = newDataSource;
         _compoundProcess = compoundProcess;
         ObjectType = context.TypeFor(compoundProcess);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(compoundProcess));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldDataSource = _compoundProcess.DataSource;
         _compoundProcess.DataSource = _newDataSource;
         _compoundProcess.RefreshName();
         Description = PKSimConstants.Command.RenameEntityCommandDescripiton(PKSimConstants.UI.DataSource, _oldDataSource, _newDataSource);
      }

      protected override void ClearReferences()
      {
         _compoundProcess = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _compoundProcess = context.Get<CompoundProcess>(_processId);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RenameDataSourceCommand(_compoundProcess, _oldDataSource, context).AsInverseFor(this);
      }
   }
}