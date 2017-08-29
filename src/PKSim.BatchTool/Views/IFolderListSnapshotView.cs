using OSPSuite.Presentation.Views;
using PKSim.BatchTool.DTO;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool.Views
{
   public interface IFolderListSnapshotView : IView<IFolderListSnapshotPresenter>,  IResizableView
   {
      void BindTo(SnapshotFolderListDTO snapshotFolderListDTO);
   }
}