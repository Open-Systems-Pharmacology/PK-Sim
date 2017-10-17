using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Core;

namespace PKSim.CLI
{
   public abstract class concern_for_SnapshotRunner : ContextSpecificationAsync<SnapshotRunner>
   {
      protected IWorkspace _workspace;
      protected ISnapshotTask _snapshotTask;
      protected IWorkspacePersistor _workspacePersistor;
      protected ILogger _logger;
      protected SnapshotRunOptions _runOptions;
      protected string _directoryCreated;
      private Func<string, string> _oldCreateDirectory;
      protected readonly string _inputFolder = @"C:\Input\";
      protected readonly string _outputFolder = @"C:\Output\";

      public override async Task GlobalContext()
      {
         await base.GlobalContext();
         _oldCreateDirectory = DirectoryHelper.CreateDirectory;
         DirectoryHelper.CreateDirectory = s => _directoryCreated = s;
      }

      protected override Task Context()
      {
         _workspace = A.Fake<IWorkspace>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _workspacePersistor = A.Fake<IWorkspacePersistor>();
         _logger = A.Fake<ILogger>();
         sut = new SnapshotRunner(_workspace, _snapshotTask, _workspacePersistor, _logger);

         _runOptions = new SnapshotRunOptions();
         return _completed;
      }

      public override async Task GlobalCleanup()
      {
         await base.GlobalCleanup();
         DirectoryHelper.CreateDirectory = _oldCreateDirectory;
      }
   }

   public class When_running_the_snapshot_runner_for_a_single_folder_option_generating_project : concern_for_SnapshotRunner
   {
      private readonly string _fileName = "snapshotFile";
      private string _snapshotFile;
      private string _projectFile;

      protected override async Task Context()
      {
         await base.Context();
         _runOptions.ExportMode = SnapshotExportMode.Project;
         _runOptions.InputFolder = _inputFolder;
         _runOptions.OutputFolder = _outputFolder;
         _snapshotFile = Path.Combine(_inputFolder, $"{_fileName}{Constants.Filter.JSON_EXTENSION}");
         _projectFile = Path.Combine(_outputFolder, $"{_fileName}{CoreConstants.Filter.PROJECT_EXTENSION}");
         sut.AllFilesFrom = (folder, filter) => new[] {new FileInfo(_snapshotFile) };
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_load_the_snapshot_from_file()
      {
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshot(_snapshotFile)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_project_from_snapshot()
      {
         A.CallTo(() => _workspacePersistor.SaveSession(_workspace, _projectFile)).MustHaveHappened();  
      }

      [Observation]
      public void should_generate_the_output_folder_in_case_in_does_not_exist()
      {
         _directoryCreated.ShouldBeEqualTo(_outputFolder);
      }
   }

   public class When_running_the_snapshot_runner_for_a_single_folder_option_generating_snapshot : concern_for_SnapshotRunner
   {
      private readonly string _fileName = "project";
      private string _snapshotFile;
      private string _projectFile;

      protected override async Task Context()
      {
         await base.Context();
         _runOptions.ExportMode = SnapshotExportMode.Snapshot;
         _runOptions.InputFolder = _inputFolder;
         _runOptions.OutputFolder = _outputFolder;
         _snapshotFile = Path.Combine(_outputFolder, $"{_fileName}{Constants.Filter.JSON_EXTENSION}");
         _projectFile = Path.Combine(_inputFolder, $"{_fileName}{CoreConstants.Filter.PROJECT_EXTENSION}");

         sut.AllFilesFrom = (folder, filter) => new[] { new FileInfo(_projectFile) };
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_load_the_project_into_workspace()
      {
         A.CallTo(() => _workspacePersistor.LoadSession(_workspace, _projectFile)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_project_to_snapshot()
      {
         A.CallTo(() => _snapshotTask.ExportModelToSnapshot(A<PKSimProject>._, _snapshotFile)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_output_folder_in_case_in_does_not_exist()
      {
         _directoryCreated.ShouldBeEqualTo(_outputFolder);
      }
   }

   public class When_running_the_snapshot_runner_for_a_multi_folder_option_generating_project : concern_for_SnapshotRunner
   {
      private readonly string _fileName = "snapshotFile";
      private string _snapshotFile;
      private string _projectFile;

      protected override async Task Context()
      {
         await base.Context();
         _runOptions.ExportMode = SnapshotExportMode.Project;
         _runOptions.Folders = new[] {_inputFolder};
         _snapshotFile = Path.Combine(_inputFolder, $"{_fileName}{Constants.Filter.JSON_EXTENSION}");
         _projectFile = Path.Combine(_inputFolder, $"{_fileName}{CoreConstants.Filter.PROJECT_EXTENSION}");
         sut.AllFilesFrom = (folder, filter) => new[] { new FileInfo(_snapshotFile) };
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_load_the_snapshot_from_file()
      {
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshot(_snapshotFile)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_project_from_snapshot()
      {
         A.CallTo(() => _workspacePersistor.SaveSession(_workspace, _projectFile)).MustHaveHappened();
      }
    
   }
}