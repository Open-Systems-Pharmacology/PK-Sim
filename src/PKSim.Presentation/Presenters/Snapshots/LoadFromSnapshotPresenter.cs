using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public class LoadFromSnapshotPresenter<T> : LoadFromSnapshotPresenter<T, PKSimProject, Project> where T : class, IObjectBase
   {
      public LoadFromSnapshotPresenter(
         ILoadFromSnapshotView view,
         ILogPresenter logPresenter,
         ISnapshotTask snapshotTask,
         IDialogCreator dialogCreator,
         IObjectTypeResolver objectTypeResolver,
         IOSPSuiteLogger logger,
         IEventPublisher eventPublisher) : base(view, logPresenter, snapshotTask, dialogCreator, objectTypeResolver, logger, eventPublisher)
      {
      }
   }
}