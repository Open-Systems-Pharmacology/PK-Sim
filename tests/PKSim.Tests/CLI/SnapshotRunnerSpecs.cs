using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.CLI
{
   public abstract class concern_for_SnapshotRunner : ContextSpecificationAsync<SnapshotRunner>
   {
      protected ICoreWorkspace _workspace;
      protected ISnapshotTask _snapshotTask;
      protected IWorkspacePersistor _workspacePersistor;
      protected IOSPSuiteLogger _logger;
      protected IMissingBuildingBlockContentRecoveryTask _missingBuildingBlockContentRecoveryTask;
      protected SnapshotRunOptions _runOptions;
      protected string _createdDirectory;
      private Func<string, string> _oldCreateDirectory;
      protected readonly string _inputFolder = @"C:\Input\";
      protected readonly string _outputFolder = @"C:\Output\";

      public override async Task GlobalContext()
      {
         await base.GlobalContext();
         _oldCreateDirectory = DirectoryHelper.CreateDirectory;
         DirectoryHelper.CreateDirectory = s => _createdDirectory = s;
      }

      protected override Task Context()
      {
         _workspace = A.Fake<ICoreWorkspace>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _workspacePersistor = A.Fake<IWorkspacePersistor>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _missingBuildingBlockContentRecoveryTask = A.Fake<IMissingBuildingBlockContentRecoveryTask>();
         A.CallTo(() => _missingBuildingBlockContentRecoveryTask.Recover(A<PKSimProject>._)).Returns(MissingBuildingBlockContentRecoveryResult.Empty);
         sut = new SnapshotRunner(_workspace, _snapshotTask, _workspacePersistor, _logger, _missingBuildingBlockContentRecoveryTask);

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
         sut.AllFilesFrom = (folder, filter) => new[] { new FileInfo(_snapshotFile) };
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_load_the_snapshot_from_file()
      {
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshotFileAsync(_snapshotFile, true)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_project_from_snapshot()
      {
         A.CallTo(() => _workspacePersistor.SaveSession(_workspace, _projectFile)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_output_folder_in_case_in_does_not_exist()
      {
         _createdDirectory.ShouldBeEqualTo(_outputFolder);
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
         A.CallTo(() => _snapshotTask.ExportModelToSnapshotAsync(A<PKSimProject>._, _snapshotFile)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_output_folder_in_case_in_does_not_exist()
      {
         _createdDirectory.ShouldBeEqualTo(_outputFolder);
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
         _runOptions.Folders = new[] { _inputFolder };
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
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshotFileAsync(_snapshotFile, true)).MustHaveHappened();
      }

      [Observation]
      public void should_generate_the_project_from_snapshot()
      {
         A.CallTo(() => _workspacePersistor.SaveSession(_workspace, _projectFile)).MustHaveHappened();
      }
   }

   public class When_running_the_snapshot_runner_for_a_project_file_in_which_building_blocks_without_content_were_recovered : concern_for_SnapshotRunner
   {
      private readonly string _fileName = "projectFile";
      private string _snapshotFile;
      private string _projectFile;
      private PKSimProject _project;
      private MissingBuildingBlockContentRecoveryResult _recoveryResult;

      protected override async Task Context()
      {
         await base.Context();
         _runOptions.ExportMode = SnapshotExportMode.Snapshot;
         _runOptions.InputFolder = _inputFolder;
         _runOptions.OutputFolder = _outputFolder;
         _snapshotFile = Path.Combine(_outputFolder, $"{_fileName}{Constants.Filter.JSON_EXTENSION}");
         _projectFile = Path.Combine(_inputFolder, $"{_fileName}{CoreConstants.Filter.PROJECT_EXTENSION}");
         sut.AllFilesFrom = (folder, filter) => new[] {new FileInfo(_projectFile)};

         _project = new PKSimProject();
         A.CallTo(() => _workspacePersistor.LoadSession(_workspace, _projectFile)).Invokes(x => _workspace.Project = _project);
         var removedBuildingBlock = new BuildingBlockWithMissingContent {Id = "EP", Name = "CYP3A4|Human|Pop", BuildingBlockType = PKSimBuildingBlockType.ExpressionProfile};
         _recoveryResult = new MissingBuildingBlockContentRecoveryResult(new List<IPKSimBuildingBlock>(), new[] {removedBuildingBlock}, "recovery message");
         A.CallTo(() => _missingBuildingBlockContentRecoveryTask.Recover(_project)).Returns(_recoveryResult);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_recover_the_building_blocks_without_content_before_exporting_the_snapshot()
      {
         A.CallTo(() => _missingBuildingBlockContentRecoveryTask.Recover(_project)).MustHaveHappened()
            .Then(A.CallTo(() => _snapshotTask.ExportModelToSnapshotAsync(_project, _snapshotFile)).MustHaveHappened());
      }

      [Observation]
      public void should_log_the_recovery_as_a_warning()
      {
         A.CallTo(() => _logger.AddToLog(_recoveryResult.Message, LogLevel.Warning, A<string>._)).MustHaveHappened();
      }
   }
}