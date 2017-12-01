using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Snapshots;
using PKSim.Presentation.Presenters.Snapshots;

namespace PKSim.Presentation.Views.Snapshots
{
   public interface ILoadFromSnapshotView : IModalView<ILoadFromSnapshotPresenter>
   {
      void AddLogView(IView view);
      void BindTo(LoadFromSnapshotDTO loadFromSnapshotDTO);
      void EnableButtons(bool cancelEnabled, bool okEnabled = false, bool startEnabled=false);
   }
}