using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
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
      protected ILogger _logger;
      protected IEventPublisher _eventPublisher;
      protected ILogPresenter _logPresenter;
      protected IRegistrationTask _registrationTask;
      protected IQualiticationPlanRunner _qualificationPlanRunner;

      protected override void Context()
      {
         _view = A.Fake<ILoadFromSnapshotView>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _logger = A.Fake<ILogger>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _logPresenter = A.Fake<ILogPresenter>();
         _registrationTask = A.Fake<IRegistrationTask>();
         _qualificationPlanRunner = A.Fake<IQualiticationPlanRunner>();
         sut = new LoadProjectFromSnapshotPresenter(_view, _logPresenter, _snapshotTask, _dialogCreator, _objectTypeResolver, _logger, _eventPublisher, _qualificationPlanRunner, _registrationTask);
      }
   }

   public class When_loading_a_project_from_snapshot : concern_for_LoadProjectFromSnapshotPresenter
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

      [Observation]
      public void should_have_registered_the_project_then_start_the_qualification_then_unregistered_the_project()
      {
         A.CallTo(() => _registrationTask.RegisterProject(_newProject)).MustHaveHappened()
            .Then(A.CallTo(() => _qualificationPlanRunner.RunAsync(_qualificationPlan)).MustHaveHappened())
            .Then(A.CallTo(() => _registrationTask.UnregisterProject(_newProject)).MustHaveHappened());
      }
   }
}