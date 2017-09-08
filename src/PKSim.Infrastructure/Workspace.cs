using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Events;
using OSPSuite.Core.Serialization;
using OSPSuite.Infrastructure;
using OSPSuite.Infrastructure.Journal;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.FileLocker;

namespace PKSim.Infrastructure
{
   public class Workspace : Workspace<PKSimProject>, IWorkspace
   {
      private readonly IEventPublisher _eventPublisher;
      private readonly IRegistrationTask _registrationTask;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IMRUProvider _mruProvider;
      private readonly IHistoryManagerFactory _historyManagerFactory;
      private readonly IProjectClassifiableUpdaterAfterDeserialization _projectClassifiableUpdaterAfterDeserialization;

      public IHistoryManager HistoryManager { get; set; }
      public IWorkspaceLayout WorkspaceLayout { get; set; }

      public Workspace(IEventPublisher eventPublisher, IRegistrationTask registrationTask,
         IWorkspacePersistor workspacePersistor, IMRUProvider mruProvider, IHistoryManagerFactory historyManagerFactory,
         IFileLocker fileLocker, IProjectClassifiableUpdaterAfterDeserialization projectClassifiableUpdaterAfterDeserialization,
         IJournalSession journalSession) : base(eventPublisher, journalSession, fileLocker)
      {
         _eventPublisher = eventPublisher;
         _registrationTask = registrationTask;
         _workspacePersistor = workspacePersistor;
         _mruProvider = mruProvider;
         _historyManagerFactory = historyManagerFactory;
         _projectClassifiableUpdaterAfterDeserialization = projectClassifiableUpdaterAfterDeserialization;
      }

      public void CloseProject()
      {
         if (_project == null) return;

         _eventPublisher.PublishEvent(new ProjectClosingEvent());
         _registrationTask.UnregisterProject(_project);
         HistoryManager = null;
         _workspacePersistor.CloseSession();
         Clear();
         _eventPublisher.PublishEvent(new ProjectClosedEvent());
      }

      public void SaveProject(string fileFullPath)
      {
         //try to lock the file if it exisits or is not lock already
         LockFile(fileFullPath);

         //notify the action project saving
         _eventPublisher.PublishEvent(new ProjectSavingEvent(_project));

         _workspacePersistor.SaveSession(this, fileFullPath);
         updateProjectPropertiesFrom(fileFullPath);

         _project.HasChanged = false;

         //notify event project saved
         _eventPublisher.PublishEvent(new ProjectSavedEvent(_project));

         //once the project was saved, we should be able to access the file we just saved
         AccessFile(fileFullPath);

         //we just save the project. It is not readonly per construction
         ProjectIsReadOnly = false;

         ReleaseMemory();
      }

      public void OpenProject(string fileFullPath)
      {
         try
         {
            //notify the action loading project
            _eventPublisher.PublishEvent(new ProjectLoadingEvent());

            //retrieve project from file
            _workspacePersistor.LoadSession(this, fileFullPath);

            if (_project == null) return;

            updateProjectPropertiesFrom(fileFullPath);

            //notify event project loaded with the project
            _eventPublisher.PublishEvent(new ProjectLoadedEvent(_project));
         }
         catch (Exception)
         {
            //exception while opening the file
            ReleaseLock();
            throw;
         }
         finally
         {
            if (_project == null)
               ReleaseLock();
         }
      }
    
      public bool ProjectLoaded => Project != null;

      public bool ProjectHasChanged => Project != null && Project.HasChanged && !ProjectIsReadOnly;

      public PKSimProject Project
      {
         get => _project;
         set
         {
            //unregister first 
            _registrationTask.UnregisterProject(_project);
            //save the new project and register it
            _project = value;
            _registrationTask.RegisterProject(_project);
            _projectClassifiableUpdaterAfterDeserialization.Update(_project);

            if (HistoryManager == null)
               HistoryManager = _historyManagerFactory.Create();

            _eventPublisher.PublishEvent(new ProjectCreatedEvent(_project));
         }
      }

      private void updateProjectPropertiesFrom(string fileFullPath)
      {
         _project.FilePath = fileFullPath;
         _mruProvider.Add(fileFullPath);
      }

      public void AddCommand(ICommand command)
      {
         //History manager can happen if some events were not released properly.
         //Project was closed however so change should be take into consideration
         HistoryManager?.AddToHistory(command);
      }

      public IEnumerable<ICommand> All()
      {
         return HistoryManager.History.Select(x => x.Command);
      }
   }
}