using System.Threading.Tasks;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core.Batch;

namespace PKSim.BatchTool.Presenters
{
   public interface ISnapshotRunPresenter : IBatchPresenter
   {
      void SingleFolderSelected();
      void FolderListSelected();
   }

   public class SnapshotRunPresenter : BatchPresenter<ISnapshotRunView, ISnapshotRunPresenter, SnapshotRunner, SnapshotRunOptions>, ISnapshotRunPresenter
   {
      private readonly ISingleFolderSnapshotPresenter _singleFolderSnapshotPresenter;
      private readonly IFolderListSnapshotPresenter _folderListSnapshotPresenter;
      private ISnapshotPresenter _selectedPresenter;

      public SnapshotRunPresenter(ISnapshotRunView view,
         SnapshotRunner batchRunner,
         IDialogCreator dialogCreator,
         ILogPresenter logPresenter,
         ILogger batchLogger,
         ISingleFolderSnapshotPresenter singleFolderSnapshotPresenter,
         IFolderListSnapshotPresenter folderListSnapshotPresenter) : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         _singleFolderSnapshotPresenter = singleFolderSnapshotPresenter;
         _folderListSnapshotPresenter = folderListSnapshotPresenter;
         AddSubPresenters(_singleFolderSnapshotPresenter, _folderListSnapshotPresenter);
         _view.AddSingleFolderView(_singleFolderSnapshotPresenter.View);
         _view.AddFolderListView(_folderListSnapshotPresenter.View);
         selectSnapshotPresenter(_singleFolderSnapshotPresenter);
         _singleFolderSnapshotPresenter.StatusChanged += (o, e) => updateView();
         _folderListSnapshotPresenter.StatusChanged += (o, e) => updateView();
      }

      private void selectSnapshotPresenter(ISnapshotPresenter snapshotPresenter)
      {
         _selectedPresenter = snapshotPresenter;
         _selectedPresenter.AdjustViewHeight();
         _view.SelectView(_selectedPresenter.BaseView);
         updateView();
      }

      public override Task RunBatch()
      {
         _runOptionsDTO = _selectedPresenter.RunOptions;
         return base.RunBatch();
      }

      public void SingleFolderSelected() => selectSnapshotPresenter(_singleFolderSnapshotPresenter);

      public void FolderListSelected() => selectSnapshotPresenter(_folderListSnapshotPresenter);

      public override bool CanClose => _selectedPresenter?.CanClose ?? false;

      private void updateView() => _view.CalculateEnabled = CanClose && !_isRunning;
   }
}