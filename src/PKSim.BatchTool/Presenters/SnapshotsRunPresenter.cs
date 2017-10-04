using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface ISnapshotsRunPresenter : IBatchPresenter<SnapshotRunOptions>
   {
   }

   public class SnapshotsRunPresenter : InputAndOutputBatchPresenter<SnapshotsRunner, SnapshotRunOptions>, ISnapshotsRunPresenter
   {
      public SnapshotsRunPresenter(IInputAndOutputBatchView<SnapshotRunOptions> view, SnapshotsRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger) : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim Snapshots: Create brand new project from snapshot";
      }

      public override bool SelectOutputFolder()
      {
         if (!base.SelectOutputFolder())
            return false;

         _startOptions.LogFileFullPath = CoreConstants.DefaultBatchLogFullPath(_startOptions.OutputFolder);
         return true;
      }
   }
}