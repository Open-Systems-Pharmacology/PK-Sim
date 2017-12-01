using System;
using System.IO;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization
{
   public class WorkspacePersistor : IWorkspacePersistor
   {
      private readonly IProjectPersistor _projectPersistor;
      private readonly IHistoryManagerPersistor _historyManagerPersistor;
      private readonly IWorkspaceLayoutPersistor _workspaceLayoutPersistor;
      private readonly ISessionManager _sessionManager;
      private readonly IProgressManager _progressManager;
      private readonly IProjectFileCompressor _projectFileCompressor;
      private readonly IDatabaseSchemaMigrator _databaseSchemaMigrator;
      private readonly IJournalLoader _journalLoader;
      private readonly IProjectClassifiableUpdaterAfterDeserialization _projectClassifiableUpdaterAfterDeserialization;

      public WorkspacePersistor(
         IProjectPersistor projectPersistor, 
         IHistoryManagerPersistor historyManagerPersistor, 
         IWorkspaceLayoutPersistor workspaceLayoutPersistor, 
         ISessionManager sessionManager, 
         IProgressManager progressManager, 
         IProjectFileCompressor projectFileCompressor, 
         IDatabaseSchemaMigrator databaseSchemaMigrator, 
         IJournalLoader journalLoader, 
         IProjectClassifiableUpdaterAfterDeserialization projectClassifiableUpdaterAfterDeserialization)
      {
         _projectPersistor = projectPersistor;
         _historyManagerPersistor = historyManagerPersistor;
         _workspaceLayoutPersistor = workspaceLayoutPersistor;
         _sessionManager = sessionManager;
         _progressManager = progressManager;
         _projectFileCompressor = projectFileCompressor;
         _databaseSchemaMigrator = databaseSchemaMigrator;
         _journalLoader = journalLoader;
         _projectClassifiableUpdaterAfterDeserialization = projectClassifiableUpdaterAfterDeserialization;

      }

      public void SaveSession(IWorkspace workspace, string fileFullPath)
      {
         using (var progress = _progressManager.Create())
         {
            progress.Initialize(5);

            progress.IncrementProgress(PKSimConstants.UI.CreatingProjectDatabase);

            _sessionManager.CreateFactoryFor(fileFullPath);

            using (var session = _sessionManager.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
               progress.IncrementProgress(PKSimConstants.UI.SavingProject);
               workspace.UpdateJournalPathRelativeTo(fileFullPath);
               _projectPersistor.Save(workspace.Project, session);

               progress.IncrementProgress(PKSimConstants.UI.SavingHistory);
               _historyManagerPersistor.Save(workspace.HistoryManager, session);

               progress.IncrementProgress(PKSimConstants.UI.SavingLayout);
               _workspaceLayoutPersistor.Save(workspace.WorkspaceLayout, session);

               transaction.Commit();
            }

            progress.IncrementProgress(PKSimConstants.UI.CompressionProject);
            _projectFileCompressor.Compress(fileFullPath);

            //once saved, we can 
            _projectPersistor.UpdateProjectAfterSave(workspace.Project);
         }


         workspace.Project.Name = FileHelper.FileNameFromFileFullPath(fileFullPath);
      }

      public void LoadSession(IWorkspace workspace, string fileFullPath)
      {
         using (var progress = _progressManager.Create())
         {
            progress.Initialize(5);
            progress.IncrementProgress(PKSimConstants.UI.OpeningProjectDatabase);

            verifyProjectNotReadOnly(fileFullPath);
            _databaseSchemaMigrator.MigrateSchema(fileFullPath);

            try
            {
               _sessionManager.OpenFactoryFor(fileFullPath);
               
               using (var session = _sessionManager.OpenSession())
               using (session.BeginTransaction())
               {
                  progress.IncrementProgress(PKSimConstants.UI.LoadingHistory);
                  var historyManager = _historyManagerPersistor.Load(session);
                  workspace.HistoryManager = historyManager;

                  progress.IncrementProgress(PKSimConstants.UI.LoadingProject);
                  var project = _projectPersistor.Load(session);
                  if (project == null)
                  {
                     //history was loaded but project load failed
                     workspace.HistoryManager = null;
                     return;
                  }

                  project.Name = FileHelper.FileNameFromFileFullPath(fileFullPath);
                  workspace.Project = project;

                  _projectClassifiableUpdaterAfterDeserialization.Update(project);

                  progress.IncrementProgress(PKSimConstants.UI.LoadingWorkingJournal);
                  var journal  = _journalLoader.Load(project.JournalPath, fileFullPath);
                  workspace.Journal = journal;

                  progress.IncrementProgress(PKSimConstants.UI.LoadingLayout);
                  var workspaceLayout = _workspaceLayoutPersistor.Load(session);
                  workspace.WorkspaceLayout = workspaceLayout;
               }
            }
            catch (Exception)
            {
               //Exeption occurs while opening the project! 
               //close the file and rethrow the exception
               _sessionManager.CloseFactory();
               throw;
            }
         }
      }

      private void verifyProjectNotReadOnly(string fileFullPath)
      {
         var fileInfo = new FileInfo(fileFullPath);
         if (!fileInfo.Exists || !fileInfo.IsReadOnly) return;

         throw new PKSimException(PKSimConstants.Error.ProjectFileIsReadOnlyAndCannotBeRead(fileFullPath));
      }

      public void CloseSession()
      {
         _sessionManager.CloseFactory();
      }
   }
}