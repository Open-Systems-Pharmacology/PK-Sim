using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Presenters.Snapshots;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation
{
   public abstract class concern_for_LoadProjectFromSnapshotPresenter : ContextSpecification<ILoadProjectFromSnapshotPresenter>
   {
      protected ILoadFromSnapshotView _view;
      protected ISnapshotTask _snapshotTask;
      protected IDialogCreator _dialogCreator;
      protected IObjectTypeResolver _objectTypeResolver;
      protected ILogger _logger;
      protected IEventPublisher _eventPublisher;
      protected ILogPresenter _logPresenter;

      protected override void Context()
      {
         _view = A.Fake<ILoadFromSnapshotView>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _logger = A.Fake<ILogger>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _logPresenter = A.Fake<ILogPresenter>();

         sut = new LoadProjectFromSnapshotPresenter(_view, _logPresenter, _snapshotTask, _dialogCreator, _objectTypeResolver, _logger, _eventPublisher);
      }
   }

   public class When_loading_a_project_from_snapshot : concern_for_LoadProjectFromSnapshotPresenter
   {
      private PKSimProject _project;
      private PKSimProject _newProject;

      protected override void Context()
      {
         base.Context();
         _newProject = new PKSimProject();
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshot(A<string>._)).Returns(_newProject);
      }
      protected override void Because()
      {
         sut.Start().Wait();
         _project = sut.LoadProject();
      }

      [Observation]
      public void should_return_a_project_loaded_from_snapshot()
      {
         _project.ShouldBeEqualTo(_newProject);
      }
   }
}	