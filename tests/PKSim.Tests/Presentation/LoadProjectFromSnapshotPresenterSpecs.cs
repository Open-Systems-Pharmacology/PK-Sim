using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
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
      protected IOSPSuiteLogger _logger;
      protected IEventPublisher _eventPublisher;
      protected ILogPresenter _logPresenter;
      protected IRegistrationTask _registrationTask;
      protected IQualiticationPlanRunner _qualificationPlanRunner;
      private IStartOptions _startOptions;

      protected override void Context()
      {
         _view = A.Fake<ILoadFromSnapshotView>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _logPresenter = A.Fake<ILogPresenter>();
         _registrationTask = A.Fake<IRegistrationTask>();
         _qualificationPlanRunner = A.Fake<IQualiticationPlanRunner>();
         _startOptions= A.Fake<IStartOptions>();   

         sut = new LoadProjectFromSnapshotPresenter(
            _view, 
            _logPresenter,
            _snapshotTask,
            _dialogCreator,
            _objectTypeResolver,
            _logger,
            _eventPublisher,
            _qualificationPlanRunner,
            _registrationTask, 
            _startOptions);
      }
   }

   public class When_loading_a_project_from_snapshot : concern_for_LoadProjectFromSnapshotPresenter
   {
      private PKSimProject _project;
      private PKSimProject _newProject;
      private QualificationPlan _qualificationPlan;
      private readonly string _fileName = "PROJECT.json";

      protected override void Context()
      {
         base.Context();
         _newProject = new PKSimProject();
         _qualificationPlan = new QualificationPlan();
         _newProject.AddQualificationPlan(_qualificationPlan);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileName);
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshotFileAsync(_fileName, true)).Returns(_newProject);
         A.CallTo(() => _view.Display())
            .Invokes(x => sut.StartAsync().Wait());
      }

      protected override void Because()
      {
         _project = sut.LoadProject();
      }

      [Observation]
      public void should_start_the_file_selection_immediately()
      {
         A.CallTo(() => _dialogCreator.AskForFileToOpen(A<string>._, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, null, null)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_project_loaded_from_snapshot()
      {
         _project.ShouldBeEqualTo(_newProject);
      }

      [Observation]
      public void should_have_registered_the_project_then_start_the_qualification_then_unregistered_the_project()
      {
         A.CallTo(() => _registrationTask.RegisterProject(_newProject)).MustHaveHappened()
            .Then(A.CallTo(() => _qualificationPlanRunner.RunAsync(_qualificationPlan)).MustHaveHappened())
            .Then(A.CallTo(() => _registrationTask.UnregisterProject(_newProject)).MustHaveHappened());
      }
   }

   public class When_loading_a_project_from_snapshot_and_the_user_cancels_file_selection : concern_for_LoadProjectFromSnapshotPresenter
   {
      private PKSimProject _project;
      private PKSimProject _newProject;
      private QualificationPlan _qualificationPlan;

      protected override void Context()
      {
         base.Context();
         _newProject = new PKSimProject();
         _qualificationPlan = new QualificationPlan();
         _newProject.AddQualificationPlan(_qualificationPlan);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(null);
      }

      protected override void Because()
      {
         _project = sut.LoadProject();
      }

      [Observation]
      public void should_return_null()
      {
         _project.ShouldBeNull();
      }

      [Observation]
      public void should_not_display_the_view()
      {
         A.CallTo(() => _view.Display()).MustNotHaveHappened();
      }
   }
}