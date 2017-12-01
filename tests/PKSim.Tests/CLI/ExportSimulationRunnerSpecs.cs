using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI
{
   public abstract class concern_for_ExportSimulationRunner : ContextSpecificationAsync<ExportSimulationRunner>
   {
      protected ILogger _logger;
      protected IWorkspacePersistor _workspacePersitor;
      protected IWorkspace _workspace;
      protected ISimulationExporter _simulationExporter;
      protected ILazyLoadTask _lazyLoadTask;
      protected ExportRunOptions _exportRunOptions = new ExportRunOptions {ExportMode = SimulationExportMode.Json | SimulationExportMode.Xml};

      private Func<string, bool> _oldFileExists;
      private Func<string, string> _oldCreateDirectory;

      protected bool _outputFolderCreated;
      protected string _projectFileName = "ProjectFile";
      protected static string _outputFolder = "OutputFolder";
      protected PKSimProject _project;
      protected Simulation _simulation1;
      protected Simulation _simulation2;
      protected static string _simulation1Name = "S1";
      protected static string _simulation2Name = "S2";
      protected string _s1OutputFolder = Path.Combine(_outputFolder, _simulation1Name);
      protected string _s2OutputFolder = Path.Combine(_outputFolder, _simulation2Name);
      protected bool _s1OutputFolderCreated;
      protected bool _s2OutputFolderCreated;

      public override async Task GlobalContext()
      {
         await base.GlobalContext();
         _oldFileExists = FileHelper.FileExists;
         _oldCreateDirectory = DirectoryHelper.CreateDirectory;

         FileHelper.FileExists = s => string.Equals(s, _projectFileName);
         DirectoryHelper.CreateDirectory = s =>
         {
            if (string.Equals(s, _outputFolder))
               _outputFolderCreated = true;

            if (string.Equals(s, _s1OutputFolder))
               _s1OutputFolderCreated = true;

            if (string.Equals(s, _s2OutputFolder))
               _s2OutputFolderCreated = true;

            return s;
         };

         _exportRunOptions.OutputFolder = _outputFolder;
      }

      protected override Task Context()
      {
         _logger = A.Fake<ILogger>();
         _workspacePersitor = A.Fake<IWorkspacePersistor>();
         _workspace = A.Fake<IWorkspace>();
         _simulationExporter = A.Fake<ISimulationExporter>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         sut = new ExportSimulationRunner(_logger, _workspacePersitor, _workspace, _simulationExporter, _lazyLoadTask);

         _project = new PKSimProject();
         _simulation1 = createSimulationWithResults(_simulation1Name);
         _simulation2 = createSimulationWithResults(_simulation2Name);

         A.CallTo(() => _workspacePersitor.LoadSession(_workspace, _projectFileName)).Invokes(x => { _workspace.Project = _project; });


         return _completed;
      }

      private Simulation createSimulationWithResults(string simulationName)
      {
         var simulation = new IndividualSimulation
         {
            Name = simulationName,
            SimulationSettings = new SimulationSettings(),
            DataRepository = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor(simulationName)
         };

         simulation.OutputSelections.AddOutput(new QuantitySelection("C", QuantityType.Drug));
         return simulation;
      }

      public override async Task GlobalCleanup()
      {
         await base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExists;
         DirectoryHelper.CreateDirectory = _oldCreateDirectory;
      }
   }

   public class When_running_the_export_simulation_runner_for_all_simulations_of_an_existing_project : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _exportRunOptions.RunSimulation = false;
         _project.AddBuildingBlock(_simulation1);
         _project.AddBuildingBlock(_simulation2);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_run_the_export_for_all_simulations_defined_in_the_project()
      {
         A.CallTo(() => _simulationExporter.Export(_simulation1, _s1OutputFolder, _exportRunOptions.ExportMode)).MustHaveHappened();
         A.CallTo(() => _simulationExporter.Export(_simulation2, _s2OutputFolder, _exportRunOptions.ExportMode)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_project_file()
      {
         _workspace.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_create_the_output_folder()
      {
         _outputFolderCreated.ShouldBeTrue();
      }

      [Observation]
      public void should_create_one_folder_for_each_exported_simulation()
      {
         _s1OutputFolderCreated.ShouldBeTrue();
         _s2OutputFolderCreated.ShouldBeTrue();
      }

      [Observation]
      public void should_lazy_load_all_simulations_to_export_as_well_as_their_results()
      {
         A.CallTo(() => _lazyLoadTask.Load(_simulation1)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.LoadResults(_simulation1)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load(_simulation2)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.LoadResults(_simulation2)).MustHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_selected_simulations_of_an_existing_project : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _exportRunOptions.Simulations = new[] {_simulation1.Name};
         _exportRunOptions.RunSimulation = false;
         _project.AddBuildingBlock(_simulation1);
         _project.AddBuildingBlock(_simulation2);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_run_the_export_for_the_selected_simulations_defined_in_the_project()
      {
         A.CallTo(() => _simulationExporter.Export(_simulation1, _s1OutputFolder,  _exportRunOptions.ExportMode)).MustHaveHappened();
      }

      [Observation]
      public void should_not_run_the_export_for_the_simulation_excluded_from_the_run()
      {
         A.CallTo(() => _simulationExporter.Export(_simulation2, _s2OutputFolder,  _exportRunOptions.ExportMode)).MustNotHaveHappened();
      }

      [Observation]
      public void should_load_the_project_file()
      {
         _workspace.Project.ShouldBeEqualTo(_project);
      }

      [Observation]
      public void should_create_the_output_folder()
      {
         _outputFolderCreated.ShouldBeTrue();
      }

      [Observation]
      public void should_create_one_folder_for_each_exported_simulation()
      {
         _s1OutputFolderCreated.ShouldBeTrue();
         _s2OutputFolderCreated.ShouldBeFalse();
      }

      [Observation]
      public void should_lazy_load_all_simulations_to_export_as_well_as_their_results()
      {
         A.CallTo(() => _lazyLoadTask.Load(_simulation1)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.LoadResults(_simulation1)).MustHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_simulations_of_an_existing_project_and_simulation_should_be_calculated : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _exportRunOptions.RunSimulation = true;
         _project.AddBuildingBlock(_simulation1);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_run_the_export_for_the_simulations_defined_in_the_project()
      {
         A.CallTo(() => _simulationExporter.RunAndExport(_simulation1, _s1OutputFolder,  A<SimulationRunOptions>._, _exportRunOptions.ExportMode)).MustHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_a_simulation_that_does_not_have_selected_output : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _project.AddBuildingBlock(_simulation1);
         _simulation1.OutputSelections = new OutputSelections();
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_warn_the_user_that_the_simulation_to_export_does_not_have_any_output()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains(_simulation1Name), NotificationType.Warning)).MustHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_a_simulation_that_does_not_have_any_results : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _project.AddBuildingBlock(_simulation1);
         _simulation1.DowncastTo<IndividualSimulation>().DataRepository = null;
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_warn_the_user_that_the_simulation_to_export_does_not_have_any_results()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains(_simulation1Name), NotificationType.Warning)).MustHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_a_simulation_that_does_not_exist : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _exportRunOptions.Simulations = new[] {_simulation2Name};
         _project.AddBuildingBlock(_simulation1);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_warn_the_user_that_the_simulation_to_export_does_not_have_any_output()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains(_simulation2Name), NotificationType.Warning)).MustHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_a_project_that_does_not_exist : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = "A_file_that_does_not_exist";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.RunBatchAsync(_exportRunOptions)).ShouldThrowAnAsync<OSPSuiteException>();
      }
   }
}