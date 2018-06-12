using OSPSuite.Presentation.Views;
using PKSim.BatchTool.DTO;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool.Views
{
   public interface ISingleFolderSnapshotView : IView<ISingleFolderSnapshotPresenter>, IResizableView
   {
      void BindTo(SnapshotSingleFolderDTO snapshotSingleFolderDTO);
   }
}