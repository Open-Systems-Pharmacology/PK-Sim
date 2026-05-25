using NHibernate;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Infrastructure.Serialization.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Services;

namespace PKSim.Presentation.Infrastructure.Serialization
{
   public class WorkspacePersistor : CoreWorkspacePersistor
   {
      private readonly IWorkspaceLayoutPersistor _workspaceLayoutPersistor;

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
         : base(projectPersistor, historyManagerPersistor, sessionManager, progressManager,
            projectFileCompressor, databaseSchemaMigrator, journalLoader, projectClassifiableUpdaterAfterDeserialization)
      {
         _workspaceLayoutPersistor = workspaceLayoutPersistor;
      }

      protected override void SaveLayoutFor(ICoreWorkspace workspace, ISession session)
      {
         if (workspace is IWithWorkspaceLayout withWorkspaceLayout)
            _workspaceLayoutPersistor.Save(withWorkspaceLayout.WorkspaceLayout, session);
      }

      protected override void LoadLayoutFor(ICoreWorkspace workspace, ISession session)
      {
         if (workspace is not IWithWorkspaceLayout withWorkspaceLayout)
            return;

         var workspaceLayout = _workspaceLayoutPersistor.Load(session);
         // The workspace layout may be null if the workspace was created via CLI. In that case,
         // we simply initialize the workspace layout.
         withWorkspaceLayout.WorkspaceLayout = workspaceLayout ?? new WorkspaceLayout();
      }
   }
}