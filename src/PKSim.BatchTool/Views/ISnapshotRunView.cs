using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.Presentation.Views;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool.Views
{
   public interface ISnapshotRunView : IBatchView<SnapshotRunOptions>, IView<ISnapshotRunPresenter>
   {
      void AddSingleFolderView(IResizableView view);
      void AddFolderListView(IResizableView view);
      void SelectView(IView view);
   }
}