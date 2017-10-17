using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Events;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Snapshots;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Services
{
   public interface IProjectTask : OSPSuite.Presentation.Services.IProjectTask
   {
      void NewProject();
      bool CloseCurrentProject();
      bool SaveCurrentProject();
      bool SaveCurrentProjectAs();
      void OpenProject();
      void Run(StartOptions startOptions);
      void LoadProjectFromSnapshot();
      Task ExportCurrentProjectToSnapshot();
   }

   public class ProjectTask : IProjectTask
   {
      private readonly IWorkspace _workspace;
      private readonly IApplicationController _applicationController;
      private readonly IDialogCreator _dialogCreator;
      private readonly IExecutionContext _executionContext;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly IWorkspaceLayoutUpdater _workspaceLayoutUpdater;
      private readonly IUserSettings _userSettings;
      private readonly IJournalTask _journalTask;
      private readonly IJournalRetriever _journalRetriever;
      private readonly ISnapshotTask _snapshotTask;

      public ProjectTask(IWorkspace workspace,
         IApplicationController applicationController,
         IDialogCreator dialogCreator,
         IExecutionContext executionContext,
         IHeavyWorkManager heavyWorkManager,
         IWorkspaceLayoutUpdater workspaceLayoutUpdater,
         IUserSettings userSettings,
         IJournalTask journalTask,
         IJournalRetriever journalRetriever,
         ISnapshotTask snapshotTask)
      {
         _workspace = workspace;
         _applicationController = applicationController;
         _dialogCreator = dialogCreator;
         _executionContext = executionContext;
         _heavyWorkManager = heavyWorkManager;
         _workspaceLayoutUpdater = workspaceLayoutUpdater;
         _userSettings = userSettings;
         _journalTask = journalTask;
         _journalRetriever = journalRetriever;
         _snapshotTask = snapshotTask;
      }

      public void NewProject()
      {
         if (!shouldCloseProject()) return;
         closeProject();
         createNewProject();
      }

      private void createNewProject()
      {
         _workspace.AddCommand(new CreateProjectCommand(_workspace, new PKSimProject()).Run(_executionContext));
         _workspace.Project.HasChanged = false;
      }

      public bool CloseCurrentProject()
      {
         if (!shouldCloseProject()) return false;
         closeProject();
         createNewProject();
         return true;
      }

      private void closeProject()
      {
         _workspace.CloseProject();
         _applicationController.CloseAll();
      }

      private bool shouldCloseProject()
      {
         if (!_workspace.ProjectHasChanged) return true;

         var viewResult = _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.SaveProjectChanges);
         switch (viewResult)
         {
            case ViewResult.Yes:
               return SaveCurrentProject();
            case ViewResult.Cancel:
               return false;
            default:
               return true;
         }
      }

      public bool SaveCurrentProject()
      {
         if (string.IsNullOrEmpty(_workspace.Project.FilePath))
            return saveCurrentProjectAs(_workspace.Project.Name);

         var fileInfo = new FileInfo(_workspace.Project.FilePath);
         if (fileInfo.Exists && fileInfo.IsReadOnly)
            return saveCurrentProjectAs(_workspace.Project.Name);

         return saveProjectToFile(_workspace.Project.FilePath);
      }

      public bool SaveCurrentProjectAs() => saveCurrentProjectAs(_workspace.Project.Name);

      private bool saveCurrentProjectAs(string defaultName)
      {
         var defaultNameIsUndefined = string.Equals(CoreConstants.PROJECT_UNDEFINED, defaultName);
         var defaultFileName = defaultNameIsUndefined ? string.Empty : defaultName;
         var fileName = _dialogCreator.AskForFileToSave(PKSimConstants.UI.SaveProjectTitle, CoreConstants.Filter.SAVE_PROJECT_FILTER, Constants.DirectoryKey.PROJECT, defaultFileName);
         if (string.IsNullOrEmpty(fileName)) return false;

         var previousName = _workspace.Project.Name;
         var previousPath = _workspace.Project.FilePath;

         if (!saveProjectToFile(fileName))
            return false;

         if (shouldShowWarningForSharedJournal(previousPath))
         {
            var journalName = FileHelper.FileNameFromFileFullPath(_journalRetriever.JournalFullPath);
            _dialogCreator.MessageBoxInfo(Captions.Journal.JournalWillBeSharedBetweenProjectInfo(journalName));
         }

         if (string.Equals(previousName, _workspace.Project.Name))
            return true;

         //Project was renamed
         var command = new OSPSuiteInfoCommand
         {
            Description = PKSimConstants.Command.ProjectRenamedDescription(previousName, _workspace.Project.Name),
            ObjectType = PKSimConstants.ObjectTypes.Project
         };
         _workspace.AddCommand(command);

         return true;
      }

      private bool shouldShowWarningForSharedJournal(string previousPath)
      {
         if (string.IsNullOrEmpty(previousPath))
            return false;

         if (string.Equals(_workspace.Project.FilePath, previousPath))
            return false;

         return !string.IsNullOrEmpty(_journalRetriever.JournalFullPath);
      }

      private bool saveProjectToFile(string fileName)
      {
         _workspaceLayoutUpdater.SaveCurrentLayout();
         return _heavyWorkManager.Start(() => _workspace.SaveProject(fileName), PKSimConstants.UI.SavingProject);
      }

      public void OpenProject()
      {
         if (!shouldCloseProject()) return;

         var fileName = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.OpenProjectTitle, CoreConstants.Filter.LOAD_PROJECT_FILTER, Constants.DirectoryKey.PROJECT);
         if (string.IsNullOrEmpty(fileName)) return;

         openProjectFromFile(fileName);
      }

      public void OpenProjectFrom(string projectFile)
      {
         if (!shouldCloseProject()) return;
         openProjectFromFile(projectFile);
      }

      public void Run(StartOptions startOptions)
      {
         if (startOptions == null || !startOptions.IsValid())
         {
            NewProject();
            return;
         }

         switch (startOptions.StartOptionMode)
         {
            case StartOptionMode.Project:
               openProjectFromFile(startOptions.FileToLoad, shouldStartWorker: false);
               break;
            case StartOptionMode.Population:
               openSimulationForPopulationSimulation(startOptions.FileToLoad);
               break;
            case StartOptionMode.Journal:
               newProjectWithWorkingJournal(startOptions.FileToLoad);
               break;
            default:
               return;
         }
      }

      public void LoadProjectFromSnapshot()
      {
         if (!shouldCloseProject()) return;

         closeProject();

         using (var presenter = _applicationController.Start<ILoadProjectFromSnapshotPresenter>())
         {
            _workspace.LoadProject(presenter.LoadProject());
         }        
      }

      public Task ExportCurrentProjectToSnapshot() => _snapshotTask.ExportModelToSnapshot(_workspace.Project);

      public  Task<PKSimProject> LoadProjectFromSnapshotFile(string snapshotFileFullPath) => _snapshotTask.LoadProjectFromSnapshot(snapshotFileFullPath);

      private void openSimulationForPopulationSimulation(string simulationFile)
      {
         checkSimulationFileExtension(simulationFile);
         try
         {
            closeProject();
            createNewProject();
            _executionContext.Resolve<NewImportPopulationSimulationCommand>().Execute(simulationFile);
         }
         catch (Exception)
         {
            createNewProject();
            throw;
         }
         finally
         {
            _executionContext.PublishEvent(new EnableUIEvent(_workspace.Project, _workspace.ProjectLoaded));
         }
      }

      private void checkSimulationFileExtension(string fileName)
      {
         var extension = new FileInfo(fileName).Extension;
         if (!extension.IsOneOf(Constants.Filter.PKML_EXTENSION))
            throw new PKSimException(PKSimConstants.Error.FileIsNotASimulationFile(fileName, CoreConstants.PRODUCT_NAME_WITH_TRADEMARK));
      }

      private void newProjectWithWorkingJournal(string journalFileFullPath)
      {
         if (!FileHelper.FileExists(journalFileFullPath))
            return;

         var extension = new FileInfo(journalFileFullPath).Extension;
         if (!string.Equals(extension, Constants.Filter.JOURNAL_EXTENSION))
            return;

         NewProject();
         _journalTask.LoadJournal(journalFileFullPath, showJournal: true);
      }

      private void openProjectFromFile(string projectFile, bool shouldStartWorker = true)
      {
         if (!FileHelper.FileExists(projectFile))
            throw new PKSimException(PKSimConstants.Error.ProjectFileDoesNotExist(projectFile));

         checkFileExtension(projectFile);

         closeProject();

         if (projectNeedsToBeConvertedFromVersion4(projectFile))
            throw new PKSimException(PKSimConstants.Error.ProjectFileVersion4IsNotSupportedAnymore(projectFile));

         try
         {
            if (!tryLockFile(projectFile))
               return;

            _executionContext.PublishEvent(new DisableUIEvent());
            if (shouldStartWorker)
            {
               var success = _heavyWorkManager.Start(() => _workspace.OpenProject(projectFile), PKSimConstants.UI.LoadingProject);
               if (!success)
                  createNewProject();
            }
            else
               _workspace.OpenProject(projectFile);

            if (_userSettings.ShouldRestoreWorkspaceLayout)
               _workspaceLayoutUpdater.RestoreLayout();
         }
         catch (Exception)
         {
            createNewProject();
            throw;
         }
         finally
         {
            _executionContext.PublishEvent(new EnableUIEvent(_workspace.Project, _workspace.ProjectLoaded));
         }
      }

      private void checkFileExtension(string projectFile)
      {
         var extension = new FileInfo(projectFile).Extension;
         if (!extension.IsOneOf(CoreConstants.Filter.PROJECT_EXTENSION, CoreConstants.Filter.PROJECT_OLD_EXTENSION))
            throw new PKSimException(PKSimConstants.Error.FileIsNotAPKSimFile(projectFile, CoreConstants.PRODUCT_NAME_WITH_TRADEMARK));
      }

      private bool tryLockFile(string projectFile)
      {
         try
         {
            _workspace.LockFile(projectFile);
            return true;
         }
         catch (CannotLockFileException e)
         {
            var ans = _dialogCreator.MessageBoxYesNo(PKSimConstants.Error.ProjectWillBeOpenedAsReadOnly(e.Message), PKSimConstants.UI.OpenAnyway, PKSimConstants.UI.CancelButton);
            var shouldOpenInReadOnly = (ans == ViewResult.Yes);
            if (!shouldOpenInReadOnly) return false;
            _workspace.ProjectIsReadOnly = true;
            return true;
         }
      }

      private bool projectNeedsToBeConvertedFromVersion4(string projectFile)
      {
         var toto = new FileInfo(projectFile);
         return string.Equals(toto.Extension, CoreConstants.Filter.PROJECT_OLD_EXTENSION);
      }
   }
}