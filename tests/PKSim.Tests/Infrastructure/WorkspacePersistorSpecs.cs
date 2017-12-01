using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using FakeItEasy;
using NHibernate;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Infrastructure.Serialization;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_WorkspacePersistor : ContextSpecification<IWorkspacePersistor>
   {
      protected IProjectPersistor _projectPersistor;
      protected IHistoryManagerPersistor _historyManagerPersistor;
      protected IWorkspace _workspace;
      protected ISessionManager _sessionManager;
      protected ISession _session;
      protected ITransaction _transaction;
      protected string _fileName;
      protected IProgressManager _progressManager;
      protected IWorkspaceLayoutPersistor _workspaceLayoutPersistor;
      protected IHistoryManager _historyManager;
      protected IPKSimCommand _command1;
      protected IPKSimCommand _command2;
      protected IProjectFileCompressor _projectFileCompressor;
      protected IDatabaseSchemaMigrator _databaseSchemaMigrator;
      private IJournalLoader _journalLoader;
      protected IProjectClassifiableUpdaterAfterDeserialization _projectClassifiableUpdaterAfterDeserialization;

      protected override void Context()
      {
         _projectPersistor = A.Fake<IProjectPersistor>();
         _historyManagerPersistor = A.Fake<IHistoryManagerPersistor>();
         _progressManager = A.Fake<IProgressManager>();
         _workspaceLayoutPersistor = A.Fake<IWorkspaceLayoutPersistor>();
         _historyManager = A.Fake<IHistoryManager>();
         _projectFileCompressor = A.Fake<IProjectFileCompressor>();
         _databaseSchemaMigrator= A.Fake<IDatabaseSchemaMigrator>();
         _journalLoader= A.Fake<IJournalLoader>();
         _projectClassifiableUpdaterAfterDeserialization= A.Fake<IProjectClassifiableUpdaterAfterDeserialization>();
         _command1 = A.Fake<IPKSimCommand>();
         _command2 = A.Fake<IPKSimCommand>();
         var history1 = A.Fake<IHistoryItem>();

         A.CallTo(() => history1.Command).Returns(_command1);
         var history2 = A.Fake<IHistoryItem>();
         A.CallTo(() => history2.Command).Returns(_command2);

         A.CallTo(() => _progressManager.Create()).Returns(A.Fake<IProgressUpdater>());
         A.CallTo(() => _historyManager.History).Returns(new[] {history1, history2});
         _workspace = A.Fake<IWorkspace>();
         _workspace.HistoryManager = _historyManager;
         _session = A.Fake<ISession>();
         _transaction = A.Fake<ITransaction>();
         A.CallTo(() => _session.BeginTransaction()).Returns(_transaction);
         _sessionManager = A.Fake<ISessionManager>();
         A.CallTo(() => _sessionManager.OpenSession()).Returns(_session);
         _fileName = "c:\\toto.txt";
         sut = new WorkspacePersistor(_projectPersistor, _historyManagerPersistor, _workspaceLayoutPersistor, _sessionManager, _progressManager,
            _projectFileCompressor,_databaseSchemaMigrator,_journalLoader, _projectClassifiableUpdaterAfterDeserialization);
      }
   }

   public class When_saving_a_workspace : concern_for_WorkspacePersistor
   {
      protected override void Because()
      {
         sut.SaveSession(_workspace, _fileName);
      }

      [Observation]
      public void should_leverage_the_session_manager_to_retrieve_a_project_session()
      {
         A.CallTo(() => _sessionManager.OpenSession()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_project()
      {
         A.CallTo(() => _projectPersistor.Save(_workspace.Project, _session)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_history()
      {
         A.CallTo(() => _historyManagerPersistor.Save(_workspace.HistoryManager, _session)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_workspace_layout()
      {
         A.CallTo(() => _workspaceLayoutPersistor.Save(_workspace.WorkspaceLayout, _session)).MustHaveHappened();
      }

      [Observation]
      public void should_create_a_new_factory_for_the_given_file()
      {
         A.CallTo(() => _sessionManager.CreateFactoryFor(_fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_commit_the_transaction()
      {
         A.CallTo(() => _transaction.Commit()).MustHaveHappened();
      }

      [Observation]
      public void should_dispose_the_session()
      {
         A.CallTo(() => _session.Dispose()).MustHaveHappened();
      }

      [Observation]
      public void should_compress_the_project()
      {
         A.CallTo(() => _projectFileCompressor.Compress(_fileName)).MustHaveHappened();
      }
  }

   public class When_loading_a_workspace : concern_for_WorkspacePersistor
   {
      private PKSimProject _project;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<PKSimProject>();
         A.CallTo(() => _projectPersistor.Load(_session)).Returns(_project);
      }

      protected override void Because()
      {
         sut.LoadSession(_workspace, _fileName);
      }

      [Observation]
      public void should_migrate_the_database_schema()
      {
         A.CallTo(() => _databaseSchemaMigrator.MigrateSchema(_fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_session_manager_to_retrieve_a_project_session()
      {
         A.CallTo(() => _sessionManager.OpenSession()).MustHaveHappened();
      }

      [Observation]
      public void should_open_a_new_session_for_the_given_file()
      {
         A.CallTo(() => _sessionManager.OpenFactoryFor(_fileName)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_project()
      {
         A.CallTo(() => _projectPersistor.Load(_session)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_project_in_the_workspace()
      {
         _workspace.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_load_the_history()
      {
         A.CallTo(() => _historyManagerPersistor.Load(_session)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_workspace_layout()
      {
         A.CallTo(() => _workspaceLayoutPersistor.Load(_session)).MustHaveHappened();
      }

      [Observation]
      public void should_unbind_the_session()
      {
         A.CallTo(() => _session.Dispose()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_project_after_deserialization()
      {
         A.CallTo(() => _projectClassifiableUpdaterAfterDeserialization.Update(_project)).MustHaveHappened();
      }
   }

   public class When_loading_a_workspace_from_a_corrupted_file : concern_for_WorkspacePersistor
   {
      private PKSimProject _project;

      protected override void Context()
      {
         base.Context();
         _project = null;
         A.CallTo(() => _projectPersistor.Load(_session)).Returns(_project);
      }

      protected override void Because()
      {
         sut.LoadSession(_workspace, _fileName);
      }

      [Observation]
      public void should_not_set_the_project_in_the_workspace()
      {
         _workspace.Project.ShouldNotBeEqualTo(_project);
      }

      [Observation]
      public void should_reset_the_history()
      {
         _workspace.HistoryManager.ShouldBeNull();
      }
   }

   public class When_told_to_close_the_session : concern_for_WorkspacePersistor
   {
      protected override void Because()
      {
         sut.CloseSession();
      }

      [Observation]
      public void should_leverage_the_session_manager_and_release_any_open_session()
      {
         A.CallTo(() => _sessionManager.CloseFactory()).MustHaveHappened();
      }
   }
}