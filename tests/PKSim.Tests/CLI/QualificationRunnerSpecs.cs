using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Core;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Individual = PKSim.Core.Snapshots.Individual;
using Simulation = PKSim.Core.Snapshots.Simulation;
using SnapshotProject = PKSim.Core.Snapshots.Project;

namespace PKSim.CLI
{
   public abstract class concern_for_QualificationRunner : ContextSpecificationAsync<QualificationRunner>
   {
      protected ISnapshotTask _snapshotTask;
      protected IJsonSerializer _jsonSerializer;
      protected ICoreWorkspace _workspace;
      protected IWorkspacePersistor _workspacePersistor;
      protected IExportSimulationRunner _exportSimulationRunner;
      protected IOSPSuiteLogger _logger;
      protected QualificationRunOptions _runOptions;
      protected QualifcationConfiguration _qualificationConfiguration;
      private Func<string, string> _oldCreateDirectory;
      protected List<string> _createdDirectories = new List<string>();
      private Func<string, bool> _oldFileExists;
      private Func<string, bool> _oldDirectoryExists;
      private Action<string, bool> _oldDeleteDirectory;
      protected IDataRepositoryExportTask _dataRepositoryTask;
      protected IMarkdownReporterTask _markdownReporterTask;

      public override async Task GlobalContext()
      {
         await base.GlobalContext();
         _oldCreateDirectory = DirectoryHelper.CreateDirectory;
         _oldDirectoryExists = DirectoryHelper.DirectoryExists;
         _oldDeleteDirectory = DirectoryHelper.DeleteDirectory;
         _oldFileExists = FileHelper.FileExists;
         DirectoryHelper.CreateDirectory = s =>
         {
            _createdDirectories.Add(s);
            return s;
         };
      }

      protected override Task Context()
      {
         _snapshotTask = A.Fake<ISnapshotTask>();
         _jsonSerializer = A.Fake<IJsonSerializer>();
         _workspace = A.Fake<ICoreWorkspace>();
         _workspacePersistor = A.Fake<IWorkspacePersistor>();
         _exportSimulationRunner = A.Fake<IExportSimulationRunner>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _dataRepositoryTask = A.Fake<IDataRepositoryExportTask>();
         _markdownReporterTask = A.Fake<IMarkdownReporterTask>();

         sut = new QualificationRunner(_snapshotTask, _jsonSerializer, _workspace, _workspacePersistor, _exportSimulationRunner, _dataRepositoryTask, _markdownReporterTask, _logger);

         _runOptions = new QualificationRunOptions();
         _qualificationConfiguration = new QualifcationConfiguration();
         return _completed;
      }

      public override async Task GlobalCleanup()
      {
         await base.GlobalCleanup();
         DirectoryHelper.CreateDirectory = _oldCreateDirectory;
         FileHelper.FileExists = _oldFileExists;
         DirectoryHelper.DirectoryExists = _oldDirectoryExists;
         DirectoryHelper.DeleteDirectory = _oldDeleteDirectory;
      }
   }

   public class When_running_the_qualification_runner_with_an_invalid_configuration : concern_for_QualificationRunner
   {
      [Observation]
      public void should_log_the_error()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public abstract class concern_for_QualificationRunnerWithValidConfiguration : concern_for_QualificationRunner
   {
      protected SnapshotProject _projectSnapshot;
      protected PKSimProject _project;
      protected const string PROJECT_NAME = "toto";
      protected const string PROJECT_SNAPSHOT_NAME = "toto_model";

      protected override async Task Context()
      {
         await base.Context();
         _runOptions.ConfigurationFile = "XXX";
         _runOptions.Run = true;
         A.CallTo(() => _jsonSerializer.Deserialize<QualifcationConfiguration>(_runOptions.ConfigurationFile)).Returns(_qualificationConfiguration);
         _qualificationConfiguration.Project = PROJECT_NAME;
         _qualificationConfiguration.OutputFolder = "c:/tests/outputs/";
         _qualificationConfiguration.InputsFolder = "c:/tests/outputs/INPUTS";
         _qualificationConfiguration.SnapshotFile = $"c:/tests/inputs/{PROJECT_SNAPSHOT_NAME}.json";
         _qualificationConfiguration.MappingFile = $"c:/tests/temp/{PROJECT_NAME}_Mapping.json";
         _qualificationConfiguration.TempFolder = $"c:/tests/temp";
         _qualificationConfiguration.ReportConfigurationFile = "c:/tests/outputs/report_config.json";
         _qualificationConfiguration.ObservedDataFolder = "c:/tests/outputs/OBS_DATA_FOLDER";

         _projectSnapshot = new SnapshotProject().WithName(PROJECT_SNAPSHOT_NAME);
         _project = new PKSimProject().WithName(PROJECT_NAME);
         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_qualificationConfiguration.SnapshotFile)).Returns(_projectSnapshot);
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshotAsync(_projectSnapshot, _runOptions.Run)).Returns(_project);
         FileHelper.FileExists = s => s.IsOneOf(_qualificationConfiguration.SnapshotFile, _runOptions.ConfigurationFile);
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_resulting_in_an_output_folder_not_defined : concern_for_QualificationRunnerWithValidConfiguration
   {
      protected override async Task Context()
      {
         await base.Context();
         _qualificationConfiguration.OutputFolder = "";
      }

      [Observation]
      public void should_log_the_error()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_resulting_in_a_mapping_file_not_defined : concern_for_QualificationRunnerWithValidConfiguration
   {
      protected override async Task Context()
      {
         await base.Context();
         _qualificationConfiguration.MappingFile = "";
      }

      [Observation]
      public void should_log_the_error()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_for_a_snapshot_file_that_does_not_exist : concern_for_QualificationRunnerWithValidConfiguration
   {
      protected override async Task Context()
      {
         await base.Context();
         FileHelper.FileExists = s => false;
      }

      [Observation]
      public void should_log_the_error()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_for_a_valid_snapshot_file : concern_for_QualificationRunnerWithValidConfiguration
   {
      private string _expectedOutputPath;
      private string _deletedDirectory;
      private ExportRunOptions _exportOptions;
      private DataRepository _observedData;
      private SimulationMapping[] _simulationExports;
      private SimulationMapping _simulationExport;
      private string _expectedSimulationPath;
      private QualificationMapping _mapping;
      private string _simulationName;
      private string _expectedObservedDataXlsFullPath;
      private string _expectedObservedDataCsvFullPath;
      private string _expectedInputFullPath;
      private Input _input;
      private Simulation _simulation;
      private IndividualSimulation _individualSimulation;

      protected override async Task Context()
      {
         await base.Context();

         _expectedOutputPath = Path.Combine(_qualificationConfiguration.OutputFolder, PROJECT_NAME);
         DirectoryHelper.DirectoryExists = s => string.Equals(s, _expectedOutputPath);
         DirectoryHelper.DeleteDirectory = (s, b) => _deletedDirectory = s;

         _simulationName = "S1";
         _simulation = new Simulation {Name = _simulationName};
         _individualSimulation = new IndividualSimulation {Name = _simulationName};
         _input = new Input {Project = PROJECT_NAME, Name = _simulationName, SectionId = 2, Type = PKSimBuildingBlockType.Simulation, SectionLevel = 5};

         _expectedSimulationPath = Path.Combine(_expectedOutputPath, _simulationName);
         _simulationExport = new SimulationMapping { Project = PROJECT_NAME, Simulation = _simulationName, Path = _expectedSimulationPath};
         _simulationExports = new[] {_simulationExport};
         A.CallTo(() => _exportSimulationRunner.ExportSimulationsIn(_project, A<ExportRunOptions>._))
            .Invokes(x => _exportOptions = x.GetArgument<ExportRunOptions>(1))
            .Returns(_simulationExports);

         _observedData = DomainHelperForSpecs.ObservedData().WithName("OBS");
         _project.AddObservedData(_observedData);
         _projectSnapshot.Simulations = new[] {_simulation};
         _expectedObservedDataXlsFullPath = Path.Combine(_qualificationConfiguration.ObservedDataFolder, $"{_observedData.Name}{Constants.Filter.XLSX_EXTENSION}");
         _expectedObservedDataCsvFullPath = Path.Combine(_qualificationConfiguration.ObservedDataFolder, $"{_observedData.Name}{Constants.Filter.CSV_EXTENSION}");

         _expectedInputFullPath = Path.Combine(_qualificationConfiguration.InputsFolder, PROJECT_NAME, PKSimBuildingBlockType.Simulation.ToString(), $"{_simulationName}{CoreConstants.Filter.MARKDOWN_EXTENSION}");

         A.CallTo(() => _jsonSerializer.Serialize(A<QualificationMapping>._, _qualificationConfiguration.MappingFile))
            .Invokes(x => _mapping = x.GetArgument<QualificationMapping>(0));

         _project.AddBuildingBlock(_individualSimulation);
         _qualificationConfiguration.Inputs = new[] {_input};
         _runOptions.Run = true;
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_delete_the_project_output_folder_under_the_output_folder_if_available()
      {
         _deletedDirectory.ShouldBeEqualTo(_expectedOutputPath);
      }

      [Observation]
      public void should_create_the_output_directory_for_the_project()
      {
         _createdDirectories.ShouldContain(_expectedOutputPath);
      }

      [Observation]
      public void should_create_the_output_directory_for_the_observed_data()
      {
         _createdDirectories.ShouldContain(_qualificationConfiguration.ObservedDataFolder);
      }

      [Observation]
      public void should_load_the_project_from_snapshot_and_export_its_simulations_to_the_output_folder()
      {
         _exportOptions.OutputFolder.ShouldBeEqualTo(_expectedOutputPath);
         _exportOptions.ExportMode.ShouldBeEqualTo(SimulationExportMode.Csv | SimulationExportMode.Xml);
      }

      [Observation]
      public void should_export_the_mapping_to_the_specified_mapping_file()
      {
         _mapping.ShouldNotBeNull();
      }

      [Observation]
      public void should_export_the_simulation_configuration_with_mapping_relative_to_the_report_output_folder()
      {
         _mapping.SimulationMappings.Length.ShouldBeEqualTo(1);
         _mapping.SimulationMappings[0].Simulation.ShouldBeEqualTo(_simulationName);
         _mapping.SimulationMappings[0].Project.ShouldBeEqualTo(PROJECT_NAME);
         _mapping.SimulationMappings[0].Path.ShouldBeEqualTo($"{PROJECT_NAME}/{_simulationName}/");
      }

      [Observation]
      public void should_export_the_observed_data_defined_in_the_project_to_excel_into_the_observed_data_folder()
      {
         A.CallTo(() => _dataRepositoryTask.ExportToExcelAsync(_observedData, _expectedObservedDataXlsFullPath, false, null)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_observed_data_defined_in_the_project_to_csv_into_the_observed_data_folder()
      {
         A.CallTo(() => _dataRepositoryTask.ExportToCsvAsync(_observedData, _expectedObservedDataCsvFullPath, null)).MustHaveHappened();
      }

      [Observation]
      public void should_export_the_observed_data_mapping_relative_to_the_report_output_folder()
      {
         _mapping.ObservedDataMappings.Length.ShouldBeEqualTo(1);
         _mapping.ObservedDataMappings[0].Id.ShouldBeEqualTo(_observedData.Name);
         _mapping.ObservedDataMappings[0].Path.ShouldBeEqualTo($"OBS_DATA_FOLDER/{_observedData.Name}.csv");
      }

      [Observation]
      public void should_export_the_inputs_mapping_relative_to_the_report_output_folder()
      {
         _mapping.Inputs.Length.ShouldBeEqualTo(1);
         _mapping.Inputs[0].SectionId.ShouldBeEqualTo(_input.SectionId);
         _mapping.Inputs[0].Path.ShouldBeEqualTo($"INPUTS/{PROJECT_NAME}/Simulation/{_input.Name}.md");
         A.CallTo(() => _markdownReporterTask.ExportToMarkdown(_individualSimulation, _expectedInputFullPath, _input.SectionLevel)).MustHaveHappened();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_for_a_valid_snapshot_file_in_validation_mode : concern_for_QualificationRunnerWithValidConfiguration
   {
      private string _expectedOutputPath;
      private string _expectedObsDataPath;
      private string _deletedDirectory;
      private DataRepository _observedData;

      protected override async Task Context()
      {
         await base.Context();
         _runOptions.Validate = true;

         _expectedOutputPath = Path.Combine(_qualificationConfiguration.OutputFolder, PROJECT_NAME);
         _expectedObsDataPath = _qualificationConfiguration.ObservedDataFolder;
         DirectoryHelper.DirectoryExists = s => string.Equals(s, _expectedOutputPath);
         DirectoryHelper.DeleteDirectory = (s, b) => _deletedDirectory = s;


         _observedData = DomainHelperForSpecs.ObservedData().WithName("OBS");
         _project.AddObservedData(_observedData);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_not_delete_the_project_output_folder_under_the_output_folder_if_available()
      {
         string.IsNullOrEmpty(_deletedDirectory).ShouldBeTrue();
      }

      [Observation]
      public void should_not_create_the_output_directory_for_the_project()
      {
         _createdDirectories.ShouldNotContain(_expectedOutputPath);
      }

      [Observation]
      public void should_not_create_the_output_directory_for_the_observed_data()
      {
         _createdDirectories.ShouldNotContain(_expectedObsDataPath);
      }

      [Observation]
      public void should_not_export_the_project()
      {
         A.CallTo(() => _exportSimulationRunner.ExportSimulationsIn(_project, A<ExportRunOptions>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_export_the_observed_data_defined_in_the_project_into_the_observed_data_folder()
      {
         A.CallTo(_dataRepositoryTask).MustNotHaveHappened();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_swappable_building_blocks_based_on_an_invalid_file : concern_for_QualificationRunnerWithValidConfiguration
   {
      private BuildingBlockSwap _buildingBlockSwap;

      protected override async Task Context()
      {
         await base.Context();
         _buildingBlockSwap = new BuildingBlockSwap
         {
            Name = "Ind",
            Type = PKSimBuildingBlockType.Individual,
            SnapshotFile = "RefSnapshotPathDoesNotExist"
         };

         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_buildingBlockSwap.SnapshotFile)).Returns((SnapshotProject) null);
         _qualificationConfiguration.BuildingBlocks = new[] {_buildingBlockSwap};
      }

      [Observation]
      public void should_log_the_error_that_the_snapshot_was_not_found()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_swappable_building_blocks_based_on_a_valid_file_referencing_an_invalid_building_block_in_ref_project : concern_for_QualificationRunnerWithValidConfiguration
   {
      private BuildingBlockSwap _buildingBlockSwap;
      private SnapshotProject _refSnapshotProject;

      protected override async Task Context()
      {
         await base.Context();
         _buildingBlockSwap = new BuildingBlockSwap
         {
            Name = "Ind",
            Type = PKSimBuildingBlockType.Individual,
            SnapshotFile = "RefSnapshotPath.json"
         };

         _qualificationConfiguration.BuildingBlocks = new[] {_buildingBlockSwap};
         _refSnapshotProject = new SnapshotProject();
         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_buildingBlockSwap.SnapshotFile)).Returns(_refSnapshotProject);
      }

      [Observation]
      public void should_log_the_error_that_the_snapshot_was_not_found()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_swappable_building_blocks_based_on_a_valid_file_referencing_an_invalid_building_block_in_base_project : concern_for_QualificationRunnerWithValidConfiguration
   {
      private BuildingBlockSwap _buildingBlockSwap;
      private SnapshotProject _refSnapshotProject;
      private Individual _refIndividual;

      protected override async Task Context()
      {
         await base.Context();

         _buildingBlockSwap = new BuildingBlockSwap
         {
            Name = "Ind",
            Type = PKSimBuildingBlockType.Individual,
            SnapshotFile = "RefSnapshotPath.json"
         };

         _qualificationConfiguration.BuildingBlocks = new[] {_buildingBlockSwap};
         _refIndividual = new Individual().WithName(_buildingBlockSwap.Name);
         _refSnapshotProject = new SnapshotProject {Individuals = new[] {_refIndividual}};
         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_buildingBlockSwap.SnapshotFile)).Returns(_refSnapshotProject);
      }

      [Observation]
      public void should_log_the_error_that_the_snapshot_was_not_found_in_the_project()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_invalid_input_references : concern_for_QualificationRunnerWithValidConfiguration
   {
      protected override async Task Context()
      {
         await base.Context();

         var input = new Input
         {
            Type = PKSimBuildingBlockType.Compound,
            Name = "NOT THERE"
         };
         _qualificationConfiguration.Inputs = new[] {input};
      }

      [Observation]
      public void should_log_the_error_that_the_snapshot_was_not_found_in_the_project()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_wit_swappable_building_blocks_based_on_a_valid_file_referencing_a_valid_building_blocks : concern_for_QualificationRunnerWithValidConfiguration
   {
      private BuildingBlockSwap _buildingBlockSwap;
      private SnapshotProject _refSnapshotProject;
      private Individual _refIndividual;

      protected override async Task Context()
      {
         await base.Context();
         _buildingBlockSwap = new BuildingBlockSwap
         {
            Name = "Ind",
            Type = PKSimBuildingBlockType.Individual,
            SnapshotFile = "RefSnapshotPath"
         };

         _qualificationConfiguration.BuildingBlocks = new[] {_buildingBlockSwap};
         _originalIndividual = new Individual().WithName(_buildingBlockSwap.Name);
         _projectSnapshot.Individuals = new[] {_originalIndividual,};

         _refIndividual = new Individual().WithName(_buildingBlockSwap.Name);
         _refSnapshotProject = new SnapshotProject {Individuals = new[] {_refIndividual}};

         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_buildingBlockSwap.SnapshotFile)).Returns(_refSnapshotProject);
      }

      private Individual _originalIndividual;

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_swap_the_individual_and_use_the_referenced_individual()
      {
         _projectSnapshot.Individuals.ShouldNotContain(_originalIndividual);
         _projectSnapshot.Individuals.ShouldContain(_refIndividual);
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_swappable_simulation_parameters_based_on_a_valid_file_referencing_a_valid_simulation_parameters : concern_for_QualificationRunnerWithValidConfiguration
   {
      private SimulationParameterSwap _simulationParameterSwap;
      private SnapshotProject _refSnapshotProject;
      private Simulation _refSimulation;
      private Simulation _originalSimulation;
      private LocalizedParameter _refLocalizedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _originalSimulation = new Simulation().WithName("TARGET_SIM");
         _refSimulation = new Simulation().WithName("REF_SIM");
         _refLocalizedParameter = new LocalizedParameter {Path = "Organism|P", Value = 10};

         _simulationParameterSwap = new SimulationParameterSwap
         {
            Simulation = _refSimulation.Name,
            Path = _refLocalizedParameter.Path,
            TargetSimulations = new[] {_originalSimulation.Name},
            SnapshotFile = "RefSnapshotPath"
         };

         _refSimulation.Parameters = new[] {_refLocalizedParameter};
         _qualificationConfiguration.SimulationParameters = new[] {_simulationParameterSwap};

         _refSnapshotProject = new SnapshotProject {Simulations = new[] {_refSimulation}};

         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_simulationParameterSwap.SnapshotFile)).Returns(_refSnapshotProject);
         _projectSnapshot.Simulations = new[] {_originalSimulation};
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_add_the_simulation_parameter_from_the_reference_simulation_into_the_original_simulation()
      {
         var newParameter = _originalSimulation.Parameters.Find(x => string.Equals(x.Path, _refLocalizedParameter.Path));
         newParameter.Value.ShouldBeEqualTo(_refLocalizedParameter.Value);
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_swappable_simulation_parameters_based_on_a_target_simulation_that_does_not_exist : concern_for_QualificationRunnerWithValidConfiguration
   {
      private SimulationParameterSwap _simulationParameterSwap;
      private SnapshotProject _refSnapshotProject;
      private Simulation _refSimulation;
      private LocalizedParameter _refLocalizedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _refSimulation = new Simulation().WithName("REF_SIM");
         _refLocalizedParameter = new LocalizedParameter {Path = "Organism|P", Value = 10};

         _simulationParameterSwap = new SimulationParameterSwap
         {
            Simulation = _refSimulation.Name,
            Path = _refLocalizedParameter.Path,
            TargetSimulations = new[] {"DOES NOT EXIST"},
            SnapshotFile = "RefSnapshotPath"
         };

         _refSimulation.Parameters = new[] {_refLocalizedParameter};
         _qualificationConfiguration.SimulationParameters = new[] {_simulationParameterSwap};

         _refSnapshotProject = new SnapshotProject {Simulations = new[] {_refSimulation}};

         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_simulationParameterSwap.SnapshotFile)).Returns(_refSnapshotProject);
      }

      [Observation]
      public void should_log_the_error_that_the_simulation_was_not_found_in_the_project()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_with_swappable_simulation_parameters_based_on_a_ref_simulation_that_does_not_exist : concern_for_QualificationRunnerWithValidConfiguration
   {
      private SimulationParameterSwap _simulationParameterSwap;
      private SnapshotProject _refSnapshotProject;

      protected override async Task Context()
      {
         await base.Context();

         _simulationParameterSwap = new SimulationParameterSwap
         {
            Simulation = "DOES_NOT_EXIST",
            TargetSimulations = new[] {"DOES NOT EXIST"},
            SnapshotFile = "RefSnapshotPath"
         };

         _qualificationConfiguration.SimulationParameters = new[] {_simulationParameterSwap};

         _refSnapshotProject = new SnapshotProject();

         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_simulationParameterSwap.SnapshotFile)).Returns(_refSnapshotProject);
      }

      [Observation]
      public void should_log_the_error_that_the_simulation_was_not_found_in_the_project()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_valid_configuration_wit_swappable_simulation_parameters_based_on_a_ref_parameter_that_does_not_exist : concern_for_QualificationRunnerWithValidConfiguration
   {
      private SimulationParameterSwap _simulationParameterSwap;
      private SnapshotProject _refSnapshotProject;
      private Simulation _refSimulation;

      protected override async Task Context()
      {
         await base.Context();
         _refSimulation = new Simulation().WithName("REF_SIM");

         _simulationParameterSwap = new SimulationParameterSwap
         {
            Simulation = _refSimulation.Name,
            Path = "DOES NOT EXIST",
            SnapshotFile = "RefSnapshotPath"
         };

         _qualificationConfiguration.SimulationParameters = new[] {_simulationParameterSwap};

         _refSnapshotProject = new SnapshotProject {Simulations = new[] {_refSimulation}};

         A.CallTo(() => _snapshotTask.LoadSnapshotFromFileAsync<SnapshotProject>(_simulationParameterSwap.SnapshotFile)).Returns(_refSnapshotProject);
      }

      [Observation]
      public void should_log_the_error_that_the_simulation_was_not_found_in_the_project()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_configuration_defining_charts_for_a_simulation_that_does_not_exist : concern_for_QualificationRunnerWithValidConfiguration
   {
      protected override async Task Context()
      {
         await base.Context();
         var simulationPlot = new SimulationPlot
         {
            SectionId = 2,
            Simulation = "SimDoesNotExist"
         };
         _qualificationConfiguration.SimulationPlots = new[] {simulationPlot};
      }

      [Observation]
      public void should_log_an_error()
      {
         The.Action(() => sut.RunBatchAsync(_runOptions)).ShouldThrowAn<QualificationRunException>();
      }
   }

   public class When_running_the_qualification_runner_with_a_configuration_defining_charts_for_a_simulation_that_does_exist : concern_for_QualificationRunnerWithValidConfiguration
   {
      private QualificationMapping _mapping;
      private SimulationPlot _simulationPlot;
      private CurveChart _curveChart;

      protected override async Task Context()
      {
         await base.Context();
         var simulation = new Simulation().WithName("Sim");


         _curveChart = new CurveChart();
         simulation.IndividualAnalyses = new[] {_curveChart};
         _simulationPlot = new SimulationPlot
         {
            SectionId = 2,
            Simulation = simulation.Name
         };
         _projectSnapshot.Simulations = new[] {simulation};
         _qualificationConfiguration.SimulationPlots = new[] {_simulationPlot};

         A.CallTo(() => _jsonSerializer.Serialize(A<QualificationMapping>._, _qualificationConfiguration.MappingFile))
            .Invokes(x => _mapping = x.GetArgument<QualificationMapping>(0));
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_export_the_charts_corresponding_to_the_selected_simulation_at_the_expected_section()
      {
         _mapping.Plots.Length.ShouldBeEqualTo(1);
         _mapping.Plots[0].SectionId.ShouldBeEqualTo(_simulationPlot.SectionId);
         _mapping.Plots[0].Simulation.ShouldBeEqualTo(_simulationPlot.Simulation);
         _mapping.Plots[0].Project.ShouldBeEqualTo(_projectSnapshot.Name);
         _mapping.Plots[0].Plot.ShouldBeEqualTo(_curveChart);
      }

      [Observation]
      public void should_create_an_empty_array_for_observed_data_if_the_project_does_not_have_any_observed_data()
      {
         _mapping.ObservedDataMappings.ShouldNotBeNull();
         _mapping.ObservedDataMappings.ShouldBeEmpty();
      }

      [Observation]
      public void should_create_an_empty_array_for_inputs_if_no_input_were_exported()
      {
         _mapping.Inputs.ShouldNotBeNull();
         _mapping.Inputs.ShouldBeEmpty();
      }
   }

   public class When_running_the_qualification_runner_with_a_configuration_without_any_mapping_created : concern_for_QualificationRunnerWithValidConfiguration
   {
      private QualificationMapping _mapping;

      protected override async Task Context()
      {
         await base.Context();

         A.CallTo(() => _jsonSerializer.Serialize(A<QualificationMapping>._, _qualificationConfiguration.MappingFile))
            .Invokes(x => _mapping = x.GetArgument<QualificationMapping>(0));
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void should_create_an_empty_array_for_simulation_mapping()
      {
         _mapping.Plots.ShouldNotBeNull();
         _mapping.Plots.ShouldBeEmpty();
      }

      [Observation]
      public void should_create_an_empty_array_for_charts()
      {
         _mapping.Plots.ShouldNotBeNull();
         _mapping.Plots.ShouldBeEmpty();
      }

      [Observation]
      public void should_create_an_empty_array_for_observed_data_mappings()
      {
         _mapping.ObservedDataMappings.ShouldNotBeNull();
         _mapping.ObservedDataMappings.ShouldBeEmpty();
      }

      [Observation]
      public void should_create_an_empty_array_for_inputs()
      {
         _mapping.Inputs.ShouldNotBeNull();
         _mapping.Inputs.ShouldBeEmpty();
      }
   }
}