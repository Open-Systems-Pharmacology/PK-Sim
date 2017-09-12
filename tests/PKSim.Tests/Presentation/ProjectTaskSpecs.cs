using System;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using IProjectTask = PKSim.Presentation.Services.IProjectTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProjectTask : ContextSpecification<IProjectTask>
   {
      protected IDialogCreator _dialogCreator;
      protected IWorkspace _workspace;
      protected PKSimProject _project;
      protected IApplicationController _applicationController;
      protected IExecutionContext _executionContext;
      protected IHeavyWorkManager _heavyWorkManager;
      protected IWorkspaceLayoutUpdater _workspaceLayoutUpdater;
      private Func<string, bool> _oldFileExitst;
      protected IUserSettings _userSettings;
      protected IJournalTask _journalTask;
      protected IJournalRetriever _journalRetriever;
      protected ISnapshotTask _snapshotTask;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _project = new PKSimProject();
         _dialogCreator = A.Fake<IDialogCreator>();
         _workspace = A.Fake<IWorkspace>();
         _executionContext = A.Fake<IExecutionContext>();
         _applicationController = A.Fake<IApplicationController>();
         _workspaceLayoutUpdater = A.Fake<IWorkspaceLayoutUpdater>();
         _userSettings = A.Fake<IUserSettings>();
         _journalTask = A.Fake<IJournalTask>();
         _journalRetriever = A.Fake<IJournalRetriever>();
         _snapshotTask = A.Fake<ISnapshotTask>();

         _workspace.Project = _project;
         _workspace.WorkspaceLayout = new WorkspaceLayout();
         _heavyWorkManager = new HeavyWorkManagerForSpecs();

         sut = new ProjectTask(_workspace, _applicationController, _dialogCreator,
            _executionContext, _heavyWorkManager, _workspaceLayoutUpdater, _userSettings,
            _journalTask, _journalRetriever, _snapshotTask);

         _oldFileExitst = FileHelper.FileExists;
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExitst;
      }
   }

   public class When_runnung_pk_sim_with_a_pop_simulation_file : concern_for_ProjectTask
   {
      private readonly string _simFile = "simFile.pkml";
      private NewImportPopulationSimulationCommand _importPopulationSimlationCommand;
      private StartOptions _startOptions;

      protected override void Context()
      {
         base.Context();
         _importPopulationSimlationCommand = A.Fake<NewImportPopulationSimulationCommand>();
         FileHelper.FileExists = x => string.Equals(x, _simFile);
         A.CallTo(() => _executionContext.Resolve<NewImportPopulationSimulationCommand>()).Returns(_importPopulationSimlationCommand);
         _startOptions = new StartOptions();
         _startOptions.InitializeFrom(new[] {"/pop", _simFile});
      }

      protected override void Because()
      {
         sut.Run(_startOptions);
      }

      [Observation]
      public void should_start_population_simulation_with_simulation_file()
      {
         A.CallTo(() => _importPopulationSimlationCommand.Execute(_simFile)).MustHaveHappened();
      }
   }

   public class When_runnung_pk_sim_with_some_undefined_options : concern_for_ProjectTask
   {
      protected override void Because()
      {
         sut.Run(null);
      }

      [Observation]
      public void should_create_a_new_project()
      {
         _workspace.Project.ShouldNotBeEqualTo(_project);
      }
   }

   public class When_runnung_pk_sim_with_some_invalid_options : concern_for_ProjectTask
   {
      private StartOptions _startOptions;

      protected override void Context()
      {
         base.Context();
         _startOptions = A.Fake<StartOptions>();
         A.CallTo(() => _startOptions.IsValid()).Returns(false);
      }

      protected override void Because()
      {
         sut.Run(_startOptions);
      }

      [Observation]
      public void should_create_a_new_project()
      {
         _workspace.Project.ShouldNotBeEqualTo(_project);
      }
   }

   public class When_asked_to_create_a_new_project_when_the_current_project_does_not_exist : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = false;
      }

      protected override void Because()
      {
         sut.NewProject();
      }

      [Observation]
      public void should_set_the_newly_created_project_as_active_project_in_the_workspace()
      {
         _workspace.Project.ShouldNotBeEqualTo(_project);
      }
   }

   public class When_asked_to_create_a_new_project_with_an_existing_current_project_that_has_changed_and_the_user_canceling_the_action : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Cancel);
      }

      protected override void Because()
      {
         sut.NewProject();
      }

      [Observation]
      public void should_not_erase_the_current_active_project()
      {
         _workspace.Project.ShouldBeEqualTo(_project);
      }
   }

   public class When_asked_to_close_a_project_that_has_not_changed : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = false;
      }

      protected override void Because()
      {
         sut.CloseCurrentProject();
      }

      [Observation]
      public void should_delete_the_project_workspace()
      {
         A.CallTo(() => _workspace.CloseProject()).MustHaveHappened();
      }

      [Observation]
      public void should_close_all_opened_views()
      {
         A.CallTo(() => _applicationController.CloseAll()).MustHaveHappened();
      }
   }

   public class When_asked_to_close_a_project_that_has_changed : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
      }

      protected override void Because()
      {
         sut.CloseCurrentProject();
      }

      [Observation]
      public void should_ask_the_user_if_he_wants_to_save_the_project()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).MustHaveHappened();
      }
   }

   public class When_the_user_decides_to_cancel_the_action_of_closing_a_project : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Cancel);
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
      }

      protected override void Because()
      {
         sut.CloseCurrentProject();
      }

      [Observation]
      public void should_not_close_the_project()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_close_all_opened_views()
      {
         A.CallTo(() => _applicationController.CloseAll()).MustNotHaveHappened();
      }
   }

   public class When_asked_to_close_a_project_that_has_changed_and_the_save_action_is_not_successfull : concern_for_ProjectTask
   {
      protected override void Context()
      {
         sut = new ProjectTask(_workspace, _applicationController, _dialogCreator,
            _executionContext, new HeavyWorkManagerFailingForSpecs(), _workspaceLayoutUpdater, _userSettings, _journalTask, _journalRetriever, _snapshotTask);

         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         _project.FilePath = FileHelper.GenerateTemporaryFileName();
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.CloseCurrentProject();
      }

      [Observation]
      public void should_not_close_the_current_project()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }
   }

   public class When_the_user_decides_to_save_a_project_that_was_never_saved_before_closing_it : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Yes);
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         _project.FilePath = string.Empty;
      }

      protected override void Because()
      {
         sut.CloseCurrentProject();
      }

      [Observation]
      public void should_retrieve_a_file_where_to_save_the_project()
      {
         A.CallTo(() => _dialogCreator.AskForFileToSave(A<string>.Ignored, A<string>.Ignored, Constants.DirectoryKey.PROJECT, string.Empty, null)).MustHaveHappened();
      }
   }

   public class When_the_user_decides_to_save_a_project_that_was_already_saved_before_closing_it : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Yes);
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         _project.FilePath = "tralala";
         _project.Name = "toto";
      }

      protected override void Because()
      {
         sut.CloseCurrentProject();
      }

      [Observation]
      public void should_not_retrieve_a_file_where_to_save_the_project()
      {
         A.CallTo(() => _dialogCreator.AskForFileToSave(PKSimConstants.UI.SaveProjectTitle, CoreConstants.Filter.SAVE_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, _project.Name, null)).MustNotHaveHappened();
      }

      [Observation]
      public void should_ask_the_workspace_to_save_the_project()
      {
         A.CallTo(() => _workspace.SaveProject(_project.FilePath)).MustHaveHappened();
      }
   }

   public class When_saving_a_project_that_was_already_saved : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = true;
         _project.FilePath = "tralala";
      }

      protected override void Because()
      {
         sut.SaveCurrentProject();
      }

      [Observation]
      public void should_ask_the_workspace_to_save_the_project()
      {
         A.CallTo(() => _workspace.SaveProject(_project.FilePath)).MustHaveHappened();
      }
   }

   public class When_saving_a_project_that_has_not_changed_and_that_was_already_saved : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = false;
         _project.FilePath = "tralala";
      }

      protected override void Because()
      {
         sut.SaveCurrentProject();
      }

      [Observation]
      public void should_ask_the_workspace_to_save_the_project()
      {
         A.CallTo(() => _workspace.SaveProject(_project.FilePath)).MustHaveHappened();
      }
   }

   public class When_saving_a_project_that_was_not_already_saved : concern_for_ProjectTask
   {
      private bool _result;

      protected override void Context()
      {
         _project.HasChanged = true;
         _project.FilePath = string.Empty;
         _project.Name = "tralal";
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("tralaal");
      }

      protected override void Because()
      {
         _result = sut.SaveCurrentProject();
      }

      [Observation]
      public void should_retrieve_a_file_where_to_save_the_project()
      {
         A.CallTo(() => _dialogCreator.AskForFileToSave(PKSimConstants.UI.SaveProjectTitle, CoreConstants.Filter.SAVE_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, _project.Name, null)).MustHaveHappened();
      }

      [Observation]
      public void should_return_true()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_saving_a_project_under_another_path_and_a_journal_was_defined_for_the_project : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = true;
         _project.FilePath = "OldPath";
         _project.Name = "C";
         A.CallTo(() => _journalRetriever.JournalFullPath).Returns(@"A\B\Journal.xyz");
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("NewPath");
         A.CallTo(() => _workspace.SaveProject("NewPath"))
            .Invokes(x => _project.FilePath = "NewPath");
      }

      protected override void Because()
      {
         sut.SaveCurrentProjectAs();
      }

      [Observation]
      public void should_warn_the_user_that_the_journal_will_be_shared()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustHaveHappened();
      }
   }

   public class When_saving_a_project_under_another_path_and_a_journal_was_not_defined_for_the_project : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = true;
         _project.FilePath = "OldPath";
         _project.Name = "C";
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("NewPath");
         A.CallTo(() => _workspace.SaveProject("NewPath"))
            .Invokes(x => _project.FilePath = "NewPath");
      }

      protected override void Because()
      {
         sut.SaveCurrentProjectAs();
      }

      [Observation]
      public void should_not_warn_the_user_that_the_journal_will_be_shared()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_saving_a_project_under_the_same_path_and_a_journal_was_defined_for_the_project : concern_for_ProjectTask
   {
      protected override void Context()
      {
         _project.HasChanged = true;
         _project.FilePath = "OldPath";
         _project.Name = "C";
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("NewPath");
         A.CallTo(() => _workspace.SaveProject("NewPath"))
            .Invokes(x => _project.FilePath = "NewPath");
      }

      protected override void Because()
      {
         sut.SaveCurrentProjectAs();
      }

      [Observation]
      public void should_not_warn_the_user_that_the_journal_will_be_shared()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_saving_a_project_that_was_not_already_saved_while_the_user_cancel_the_action : concern_for_ProjectTask
   {
      private bool _result;

      protected override void Context()
      {
         _project.HasChanged = true;
         _project.FilePath = string.Empty;
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(string.Empty);
      }

      protected override void Because()
      {
         _result = sut.SaveCurrentProject();
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_asked_to_open_a_project_while_a_current_project_does_not_exist_and_the_user_select_a_valid_file : concern_for_ProjectTask
   {
      private string _fileToOpen;

      protected override void Context()
      {
         _fileToOpen = "toto.pksim5";
         FileHelper.FileExists = x => string.Equals(x, _fileToOpen);
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).Returns(_fileToOpen);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_retrieve_a_file_to_open()
      {
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).MustHaveHappened();
      }

      [Observation]
      public void should_delegate_to_the_workspace_to_open_the_file()
      {
         A.CallTo(() => _workspace.OpenProject(_fileToOpen)).MustHaveHappened();
      }
   }

   public class When_opening_a_project_and_the_user_cancel_the_action : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_not_asked_the_workspace_to_open_the_project()
      {
         A.CallTo(() => _workspace.OpenProject(A<string>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_opening_a_project_while_a_current_project_that_has_changed_is_opened : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_asked_the_user_to_save_the_project()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).MustHaveHappened();
      }
   }

   public class When_opening_a_project_while_a_current_project_that_has_changed_is_opened_and_the_user_cancel_the_save_action : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Cancel);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_not_close_the_current_project()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }
   }

   public class When_opening_a_project_while_a_current_project_that_has_changed_is_opened_and_the_saving_action_is_not_sucessful : concern_for_ProjectTask
   {
      protected override void Context()
      {
         sut = new ProjectTask(_workspace, _applicationController, _dialogCreator,
            _executionContext, new HeavyWorkManagerFailingForSpecs(), _workspaceLayoutUpdater, _userSettings, _journalTask, _journalRetriever, _snapshotTask);

         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         _project.FilePath = FileHelper.GenerateTemporaryFileName();
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_not_close_the_current_project()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }
   }

   public class When_opening_a_project_with_a_current_project_that_the_user_does_not_want_to_save_but_the_action_of_opening_is_canceled : concern_for_ProjectTask
   {
      protected override void Context()
      {
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.No);
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_not_close_the_current_projectn()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }
   }

   public class When_opening_a_project_with_a_current_project_that_the_user_does_not_want_to_save : concern_for_ProjectTask
   {
      private string _fileToOpen;

      protected override void Context()
      {
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         _fileToOpen = "toto.pksim5";
         FileHelper.FileExists = x => string.Equals(x, _fileToOpen);
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.No);
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).Returns(_fileToOpen);
         A.CallTo(() => _userSettings.ShouldRestoreWorkspaceLayout).Returns(true);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_close_the_current_project()
      {
         A.CallTo(() => _workspace.CloseProject()).MustHaveHappened();
      }

      [Observation]
      public void should_open_the_new_project()
      {
         A.CallTo(() => _workspace.OpenProject(_fileToOpen)).MustHaveHappened();
      }

      [Observation]
      public void should_restore_the_project_layout_if_set_in_the_user_settings()
      {
         A.CallTo(() => _workspaceLayoutUpdater.RestoreLayout()).MustHaveHappened();
      }
   }

   public class When_opening_a_project_file_that_does_not_exist : concern_for_ProjectTask
   {
      private string _fileToOpen;

      protected override void Context()
      {
         base.Context();
         _fileToOpen = "Tralala";
         FileHelper.FileExists = x => false;
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.OpenProjectFrom(_fileToOpen)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_opening_a_file_that_is_already_open_by_another_user : concern_for_ProjectTask
   {
      private string _fileToOpen;
      private string _message;

      protected override void Context()
      {
         base.Context();
         _fileToOpen = "toto.pksim5";
         FileHelper.FileExists = x => string.Equals(x, _fileToOpen);
         var cannotLockFileException = new CannotLockFileException(new Exception());
         A.CallTo(() => _workspace.LockFile(_fileToOpen)).Throws(cannotLockFileException);
         _message = PKSimConstants.Error.ProjectWillBeOpenedAsReadOnly(cannotLockFileException.Message);
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).Returns(_fileToOpen);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_notify_that_the_project_is_already_open()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(_message, PKSimConstants.UI.OpenAnyway, PKSimConstants.UI.CancelButton)).MustHaveHappened();
      }
   }

   public class When_opening_a_file_that_is_already_open_by_another_user_and_the_active_user_decides_to_open_the_project_anyway : concern_for_ProjectTask
   {
      private string _fileToOpen;

      protected override void Context()
      {
         base.Context();
         _fileToOpen = "toto.pksim5";
         FileHelper.FileExists = x => string.Equals(x, _fileToOpen);
         var cannotLockFileException = new CannotLockFileException(new Exception());
         A.CallTo(() => _workspace.LockFile(_fileToOpen)).Throws(cannotLockFileException);
         var message = PKSimConstants.Error.ProjectWillBeOpenedAsReadOnly(cannotLockFileException.Message);
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).Returns(_fileToOpen);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(message, PKSimConstants.UI.OpenAnyway, PKSimConstants.UI.CancelButton)).Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_open_the_project_as_readonly()
      {
         _workspace.ProjectIsReadOnly.ShouldBeTrue();
      }

      [Observation]
      public void should_delegate_to_the_workspace_to_open_the_file()
      {
         A.CallTo(() => _workspace.OpenProject(_fileToOpen)).MustHaveHappened();
      }
   }

   public class When_opening_a_file_that_is_already_open_by_another_user_and_the_active_user_decides_to_cancel_the_open_action : concern_for_ProjectTask
   {
      private string _fileToOpen;

      protected override void Context()
      {
         base.Context();
         _fileToOpen = "toto.pksim5";
         FileHelper.FileExists = x => string.Equals(x, _fileToOpen);
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, null, null)).Returns(_fileToOpen);
         var cannotLockFileException = new CannotLockFileException(new Exception());
         A.CallTo(() => _workspace.LockFile(_fileToOpen)).Throws(cannotLockFileException);
         var message = PKSimConstants.Error.ProjectWillBeOpenedAsReadOnly(cannotLockFileException.Message);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(message, PKSimConstants.UI.OKButton, PKSimConstants.UI.CancelButton)).Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.OpenProject();
      }

      [Observation]
      public void should_not_open_the_project_as_readonly()
      {
         _workspace.ProjectIsReadOnly.ShouldBeFalse();
      }

      [Observation]
      public void should_not_delegate_to_the_workspace_to_open_the_file()
      {
         A.CallTo(() => _workspace.OpenProject(_fileToOpen)).MustNotHaveHappened();
      }
   }

   public class When_opening_a_file_with_the_wrong_extension : concern_for_ProjectTask
   {
      private string _wrongFileName;

      protected override void Context()
      {
         base.Context();
         FileHelper.FileExists = x => string.Equals(x, _wrongFileName);
         _wrongFileName = "toto.xml";
      }

      [Observation]
      public void should_throw_an_error_that_the_file_is_not_a_project_file()
      {
         The.Action(() => sut.OpenProjectFrom(_wrongFileName)).ShouldThrowAn<Exception>();
      }
   }

   public class When_opening_a_project_file_version_4_2 : concern_for_ProjectTask
   {
      private string _oldFileName;

      protected override void Context()
      {
         base.Context();
         FileHelper.FileExists = x => string.Equals(x, _oldFileName);
         _oldFileName = "oldProject.pkprj";
      }

      [Observation]
      public void should_warn_the_user_that_support_for_older_file_has_ended()
      {
         The.Action(() => sut.OpenProjectFrom(_oldFileName)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_exporting_the_current_project_to_snapshot : concern_for_ProjectTask
   {
      protected override void Because()
      {
         sut.ExportCurrentProjectToSnapshot();
      }

      [Observation]
      public void should_export_the_current_project_to_a_snapshot()
      {
         A.CallTo(() => _snapshotTask.ExportSnapshot(_project)).MustHaveHappened();
      }
   }

   public class When_exporting_a_project_to_snapshot : concern_for_ProjectTask
   {
      private PKSimProject _projectToExport;

      protected override void Context()
      {
         base.Context();
         _projectToExport = new PKSimProject();
      }

      protected override void Because()
      {
         sut.ExportProjectToSnapshot(_projectToExport);
      }

      [Observation]
      public void should_export_the_project_to_snapshot()
      {
         A.CallTo(() => _snapshotTask.ExportSnapshot(_projectToExport)).MustHaveHappened();
      }
   }

   public class When_laoding_a_snapshot_into_the_current_project_with_a_project_already_open_and_the_user_cancels_the_action_of_loading_the_snapshot : concern_for_ProjectTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.No);
         A.CallTo(() => _snapshotTask.LoadFromSnapshot<PKSimProject>()).Returns(Enumerable.Empty<PKSimProject>());
      }

      protected override void Because()
      {
         sut.LoadProjectFromSnapshot();
      }

      [Observation]
      public void should_not_close_the_current_projectn()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }
   }

   public class When_laoding_a_snapshot_into_the_current_project_with_a_project_already_open_and_the_user_cancels_the_action : concern_for_ProjectTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _workspace.ProjectHasChanged).Returns(true);
         A.CallTo(() => _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges)).Returns(ViewResult.Cancel);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.LoadProjectFromSnapshot();
      }

      [Observation]
      public void should_not_close_the_current_projectn()
      {
         A.CallTo(() => _workspace.CloseProject()).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_load_the_project_from_snapshot()
      {
         A.CallTo(() => _snapshotTask.LoadFromSnapshot<PKSimProject>()).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_overwrite_the_current_project()
      {
         _workspace.Project.ShouldBeEqualTo(_project);
      }
   }

   public class When_laoding_a_snapshot_into_the_current_project_with_a_project_already_open_and_the_user_loads_a_real_snapshot_file : concern_for_ProjectTask
   {
      private PKSimProject _newProject;
      private Action _loadAction;
      private string _filename;

      protected override void Context()
      {
         base.Context();
         _newProject = new PKSimProject();
         _filename = @"C:\test\SuperProject.json";
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_filename);

         A.CallTo(() => _snapshotTask.LoadFromSnapshot<PKSimProject>(_filename)).Returns(new[] {_newProject});
         A.CallTo(() => _workspace.LoadProject(A<Action>._)).Invokes(x => _loadAction = x.GetArgument<Action>(0));
      }

      protected override void Because()
      {
         sut.LoadProjectFromSnapshot();
         _loadAction();
      }

      [Observation]
      public void should_close_the_current_project()
      {
         A.CallTo(() => _workspace.CloseProject()).MustHaveHappened();
      }

      [Observation]
      public void should_close_all_presenters_effectively_open()
      {
         A.CallTo(() => _applicationController.CloseAll()).MustHaveHappened();
      }

      [Observation]
      public void should_overwrite_the_current_project()
      {
         _workspace.Project.ShouldBeEqualTo(_newProject);
      }

      [Observation]
      public void should_set_the_properties_as_expected()
      {
         _newProject.Name.ShouldBeEqualTo("SuperProject");
         _newProject.HasChanged.ShouldBeTrue();
      }
   }
}