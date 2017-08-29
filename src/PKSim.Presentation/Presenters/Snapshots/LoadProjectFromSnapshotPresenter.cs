using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public interface ILoadProjectFromSnapshotPresenter : ILoadFromSnapshotPresenter<PKSimProject>
   {
      PKSimProject LoadProject();
   }

   public class LoadProjectFromSnapshotPresenter : LoadFromSnapshotPresenter<PKSimProject>, ILoadProjectFromSnapshotPresenter
   {
      public LoadProjectFromSnapshotPresenter(ILoadFromSnapshotView view, ILogPresenter logPresenter, ISnapshotTask snapshotTask, IDialogCreator dialogCreator, IObjectTypeResolver objectTypeResolver, ILogger logger, IEventPublisher eventPublisher) : base(view, logPresenter, snapshotTask, dialogCreator, objectTypeResolver, logger, eventPublisher)
      {
      }

      public PKSimProject LoadProject()
      {
         var models = LoadModelFromSnapshot();
         return models?.FirstOrDefault();
      }

      protected override async Task<IEnumerable<PKSimProject>> LoadModelAsync(string snapshotFile)
      {
         var project = await _snapshotTask.LoadProjectFromSnapshot(snapshotFile);
         return new[] {project};
      }
   }
}