using System;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Presentation.Core;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Events;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization;
using OSPSuite.Infrastructure.Journal;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.FileLocker;

namespace PKSim.Core
{
   public abstract class concern_for_Workspace : ContextSpecification<IWorkspace>
   {
      protected IEventPublisher _eventPublisher;
      protected IHistoryManager _historyManager;
      protected IWorkspacePersistor _workspacePersisor;
      protected IRegistrationTask _registrationTask;
      protected IMRUProvider _mruProvider;
      private IHistoryManagerFactory _historyManagerFactory;
      protected IFileLocker _fileLocker;
      private Func<string, bool> _oldFileExitst;
      protected IJournalSession _journalSession;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileExitst = FileHelper.FileExists;
      }

      protected override void Context()
      {
         _eventPublisher = A.Fake<IEventPublisher>();
         _mruProvider = A.Fake<IMRUProvider>();
         _workspacePersisor = A.Fake<IWorkspacePersistor>();
         _registrationTask = A.Fake<IRegistrationTask>();
         _historyManagerFactory = A.Fake<IHistoryManagerFactory>();
         _historyManager = A.Fake<IHistoryManager>();
         _fileLocker = A.Fake<IFileLocker>();
         _journalSession = A.Fake<IJournalSession>();
         A.CallTo(() => _historyManagerFactory.Create()).Returns(_historyManager);
         sut = new Workspace(_eventPublisher,  _journalSession, _fileLocker, _registrationTask, _workspacePersisor, _mruProvider, _historyManagerFactory);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExitst;
      }
   }

   public class When_creating_a_new_workspace : concern_for_Workspace
   {
      [Observation]
      public void should_have_a_defined_workspace_layout()
      {
         sut.WorkspaceLayout.ShouldNotBeNull();
      }
   }

   public class When_told_to_open_a_project_from_a_project_file_that_is_not_valid : concern_for_Workspace
   {
      private string _fileName;

      protected override void Context()
      {
         base.Context();
         _fileName = "toto";
      }

      protected override void Because()
      {
         sut.OpenProject(_fileName);
      }

      [Observation]
      public void should_not_notify_the_event_project_loaded()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectLoadedEvent>.Ignored)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_update_the_list_of_most_recently_used_file()
      {
         A.CallTo(() => _mruProvider.Add(_fileName)).MustNotHaveHappened();
      }

      [Observation]
      public void should_have_released_the_lock_on_the_filename()
      {
         A.CallTo(() => _fileLocker.ReleaseFile()).MustHaveHappened();
      }
   }

   public class When_told_to_open_a_project_from_a_valid_project_file : concern_for_Workspace
   {
      private string _fileName;
      private PKSimProject _project;
      private ProjectLoadingEvent _event;
      private ProjectLoadedEvent _event2;
      private ProjectCreatedEvent _event3;

      protected override void Context()
      {
         base.Context();
         _fileName = "toto";
         _project = A.Fake<PKSimProject>();
         sut.Project = _project;

         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectLoadingEvent>._)).Invokes(
            x => _event = x.GetArgument<ProjectLoadingEvent>(0));

         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectLoadedEvent>._)).Invokes(
            x => _event2 = x.GetArgument<ProjectLoadedEvent>(0));

         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectCreatedEvent>._)).Invokes(
            x => _event3 = x.GetArgument<ProjectCreatedEvent>(0));
      }

      protected override void Because()
      {
         sut.OpenProject(_fileName);
      }

      [Observation]
      public void should_notify_the_event_project_loading()
      {
         _event.ShouldBeAnInstanceOf<ProjectLoadingEvent>();
      }

      [Observation]
      public void should_register_the_project()
      {
         A.CallTo(() => _registrationTask.RegisterProject(_project)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_the_event_project_created_with_the_project()
      {
         _event3.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_leverage_the_persistence_manager_to_load_the_project_from_file()
      {
         A.CallTo(() => _workspacePersisor.LoadSession(sut, _fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_the_event_project_loaded_with_the_loaded_project_as_parameter()
      {
         _event2.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_update_the_list_of_most_recently_used_file()
      {
         A.CallTo(() => _mruProvider.Add(_fileName)).MustHaveHappened();
      }
   }

   public class When_told_to_save_a_project : concern_for_Workspace
   {
      private string _fileName;
      private PKSimProject _project;
      private ProjectSavingEvent _savingEvent;
      private ProjectSavedEvent _savedEvent;

      protected override void Context()
      {
         base.Context();
         _fileName = "toto";
         _project = A.Fake<PKSimProject>();
         _project.HasChanged = true;
         sut.Project = _project;
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectSavingEvent>.Ignored)).Invokes(
            x => _savingEvent = x.GetArgument<ProjectSavingEvent>(0));
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectSavedEvent>.Ignored)).Invokes(
            x => _savedEvent = x.GetArgument<ProjectSavedEvent>(0));
      }

      protected override void Because()
      {
         sut.SaveProject(_fileName);
      }

      [Observation]
      public void should_notify_the_event_project_saving_with_the_project()
      {
         _savingEvent.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_notify_the_event_project_saved_with_the_project()
      {
         _savedEvent.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_leverage_the_persistence_manager_to_save_the_project_from_file()
      {
         A.CallTo(() => _workspacePersisor.SaveSession(sut, _fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_list_of_most_recently_used_file()
      {
         A.CallTo(() => _mruProvider.Add(_fileName)).MustHaveHappened();
      }

      [Observation]
      public void the_saved_project_should_not_be_marked_as_changed_anymore()
      {
         _project.HasChanged.ShouldBeFalse();
      }
   }

   public class When_updating_the_path_of_the_working_journal_to_a_path_relative_to_the_project_path : concern_for_Workspace
   {
      private string _projectFullPath;
      private string _fullJournalPath;

      protected override void Context()
      {
         base.Context();
         _projectFullPath = @"C:\A\B\C\file.pksim";
         _fullJournalPath = @"C:\A\B\C\journal.sbj";

         sut.Project = new PKSimProject();
         sut.Journal = new Journal {FullPath = _fullJournalPath};
      }

      protected override void Because()
      {
         sut.UpdateJournalPathRelativeTo(_projectFullPath);

         //calling it twice to simulate saving the project again
         sut.UpdateJournalPathRelativeTo(_projectFullPath);
      }

      [Observation]
      public void should_update_the_path_to_a_relative_path()
      {
         sut.Project.JournalPath.ShouldBeEqualTo("journal.sbj");
      }
   }

   public class When_told_to_close_a_project : concern_for_Workspace
   {
      private PKSimProject _project;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<PKSimProject>();
         sut.Project = _project;
         sut.HistoryManager = A.Fake<IHistoryManager>();
      }

      protected override void Because()
      {
         sut.CloseProject();
      }

      [Observation]
      public void closing_and_closed_events_should_be_raised_in_order()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectClosingEvent>._)).MustHaveHappened();
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectClosedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void should_unregister_the_project()
      {
         A.CallTo(() => _registrationTask.UnregisterProject(_project)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_project_to_null()
      {
         sut.Project.ShouldBeNull();
      }

      [Observation]
      public void should_set_the_history_manaager_to_null()
      {
         sut.HistoryManager.ShouldBeNull();
      }

      [Observation]
      public void should_release_the_lock_on_the_opened_file()
      {
         A.CallTo(() => _fileLocker.ReleaseFile()).MustHaveHappened();
      }

      [Observation]
      public void should_close_the_journal()
      {
         A.CallTo(() => _journalSession.Close()).MustHaveHappened();
      }

      [Observation]
      public void should_not_have_the_project_opened_as_readonly_anymore()
      {
         sut.ProjectIsReadOnly.ShouldBeFalse();
      }
   }

   public class When_told_to_close_a_project_that_was_open_as_readonly : concern_for_Workspace
   {
      private PKSimProject _project;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<PKSimProject>();
         sut.Project = _project;
         sut.HistoryManager = A.Fake<IHistoryManager>();
         sut.ProjectIsReadOnly = true;
      }

      protected override void Because()
      {
         sut.CloseProject();
      }

      [Observation]
      public void should_not_have_the_project_opened_as_readonly_anymore()
      {
         sut.ProjectIsReadOnly.ShouldBeFalse();
      }
   }

   public class When_told_to_close_a_project_when_no_project_was_open : concern_for_Workspace
   {
      [Observation]
      public void should_not_crash()
      {
         sut.CloseProject();
      }
   }

   public class When_setting_a_project : concern_for_Workspace
   {
      private PKSimProject _project;
     private ProjectCreatedEvent _event;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<PKSimProject>();
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectCreatedEvent>.Ignored)).Invokes(
            x => _event = x.GetArgument<ProjectCreatedEvent>(0));
      }

      protected override void Because()
      {
         sut.Project = _project;
      }

      [Observation]
      public void should_register_the_project()
      {
         A.CallTo(() => _registrationTask.RegisterProject(_project)).MustHaveHappened();
      }

 

      [Observation]
      public void should_update_the_history_manager()
      {
         sut.HistoryManager.ShouldBeEqualTo(_historyManager);
      }
   }

   public class When_asked_to_a_lock_an_existing_file : concern_for_Workspace
   {
      private string _fileToAccess;

      protected override void Context()
      {
         base.Context();
         _fileToAccess = "tralala";
         FileHelper.FileExists = x => string.Equals(x, _fileToAccess);
      }

      protected override void Because()
      {
         sut.LockFile(_fileToAccess);
      }

      [Observation]
      public void should_try_to_access_the_new_file()
      {
         A.CallTo(() => _fileLocker.AccessFile(_fileToAccess)).MustHaveHappened();
      }
   }

   public class When_asked_to_a_lock_file_that_does_not_exist : concern_for_Workspace
   {
      private string _fileToAccess;

      protected override void Context()
      {
         base.Context();
         _fileToAccess = "tralala";
         FileHelper.FileExists = s => false;
      }

      protected override void Because()
      {
         sut.LockFile(_fileToAccess);
      }

      [Observation]
      public void should_try_to_access_the_new_file()
      {
         A.CallTo(() => _fileLocker.AccessFile(_fileToAccess)).MustNotHaveHappened();
      }
   }
}