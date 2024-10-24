﻿using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.CLI
{
   public abstract class concern_for_ExportSimulationRunner : ContextSpecificationAsync<ExportSimulationRunner>
   {
      protected IOSPSuiteLogger _logger;
      protected IWorkspacePersistor _workspacePersistor;
      protected ICoreWorkspace _workspace;
      protected ISimulationExporter _simulationExporter;
      protected ILazyLoadTask _lazyLoadTask;

      protected ExportRunOptions _exportRunOptions = new ExportRunOptions {ExportMode = SimulationExportMode.Json | SimulationExportMode.Xml};

      private Func<string, bool> _oldFileExists;
      private Func<string, string> _oldCreateDirectory;

      protected bool _outputFolderCreated;
      protected static string _projectName = "ProjectFile";
      protected string _projectFileName = $"c:\\{_projectName}.pksim";
      protected static string _outputFolder = "OutputFolder";
      protected PKSimProject _project;
      protected Simulation _simulation1;
      protected Simulation _simulation2;
      protected static string _simulation1Name = "S1 200mg/kg";
      protected static string _simulation2Name = "S2";
      protected string _s1OutputFolder = Path.Combine(_outputFolder, FileHelper.RemoveIllegalCharactersFrom(_simulation1Name));
      protected string _s2OutputFolder = Path.Combine(_outputFolder, FileHelper.RemoveIllegalCharactersFrom(_simulation2Name));
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
         _logger = A.Fake<IOSPSuiteLogger>();
         _workspacePersistor = A.Fake<IWorkspacePersistor>();
         _workspace = A.Fake<ICoreWorkspace>();
         _simulationExporter = A.Fake<ISimulationExporter>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         sut = new ExportSimulationRunner(_logger, _workspacePersistor, _workspace, _simulationExporter, _lazyLoadTask);

         _project = new PKSimProject {Name = _projectName};
         _simulation1 = createSimulationWithResults(_simulation1Name);
         _simulation2 = createSimulationWithResults(_simulation2Name);

         A.CallTo(() => _workspacePersistor.LoadSession(_workspace, _projectFileName)).Invokes(x => { _workspace.Project = _project; });


         return _completed;
      }

      private Simulation createSimulationWithResults(string simulationName)
      {
         var simulation = new IndividualSimulation
         {
            Name = simulationName,
            Settings = new SimulationSettings(),
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

   public class When_exporting_the_simulation_result_for_a_simulation_in_a_project : concern_for_ExportSimulationRunner
   {
      private SimulationMapping _export;

      protected override async Task Because()
      {
         _export = await sut.ExportSimulation(_simulation1, _exportRunOptions, _project);
      }

      [Observation]
      public void should_ensure_that_simulation_export_contains_the_name_of_the_simulation()
      {
         _export.Simulation.ShouldBeEqualTo(_simulation1Name);
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
         A.CallTo(() => _simulationExporter.Export(_simulation1, A<SimulationExportOptions>.That.Matches(x => x.OutputFolder == _s1OutputFolder))).MustHaveHappened();
         A.CallTo(() => _simulationExporter.Export(_simulation2, A<SimulationExportOptions>.That.Matches(x => x.OutputFolder == _s2OutputFolder))).MustHaveHappened();
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
         A.CallTo(() => _simulationExporter.Export(_simulation1, A<SimulationExportOptions>.That.Matches(x => x.OutputFolder == _s1OutputFolder))).MustHaveHappened();
      }

      [Observation]
      public void should_not_run_the_export_for_the_simulation_excluded_from_the_run()
      {
         A.CallTo(() => _simulationExporter.Export(_simulation2, A<SimulationExportOptions>.That.Matches(x => x.OutputFolder == _s2OutputFolder))).MustNotHaveHappened();
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

   public class When_running_the_export_simulation_runner_for_an_empty_list_of_simulations_in_an_existing_project_with_export_all_flag_false : concern_for_ExportSimulationRunner
   {
      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _exportRunOptions.Simulations = Array.Empty<string>();
         _exportRunOptions.ExportAllSimulationsIfListIsEmpty = false;
         _exportRunOptions.RunSimulation = false;
         _project.AddBuildingBlock(_simulation1);
         _project.AddBuildingBlock(_simulation2);
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_not_run_the_export_for_the_selected_simulations_defined_in_the_project()
      {
         A.CallTo(() => _simulationExporter.Export(A<Simulation>._, A<SimulationExportOptions>._)).MustNotHaveHappened();
      }
   }

   public class When_running_the_export_simulation_runner_for_simulations_of_an_existing_project_and_simulation_should_be_calculated : concern_for_ExportSimulationRunner
   {
      private SimulationExportOptions _simulationExportOptions;

      protected override async Task Context()
      {
         await base.Context();
         _exportRunOptions.ProjectFile = _projectFileName;
         _exportRunOptions.RunSimulation = true;
         _project.AddBuildingBlock(_simulation1);
         A.CallTo(() => _simulationExporter.RunAndExport(_simulation1, A<SimulationRunOptions>._, A<SimulationExportOptions>._))
            .Invokes(x => _simulationExportOptions = x.GetArgument<SimulationExportOptions>(2));
      }

      protected override Task Because()
      {
         return sut.RunBatchAsync(_exportRunOptions);
      }

      [Observation]
      public void should_run_the_export_for_the_simulations_defined_in_the_project()
      {
         _simulationExportOptions.PrependProjectName.ShouldBeFalse();
         _simulationExportOptions.ProjectName.ShouldBeEqualTo(_projectName);
         _simulationExportOptions.LogCategory.ShouldBeEqualTo(_projectName);
         _simulationExportOptions.OutputFolder.ShouldBeEqualTo(_s1OutputFolder);
         _simulationExportOptions.ExportMode.ShouldBeEqualTo(_exportRunOptions.ExportMode);
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
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains(_simulation2Name), LogLevel.Warning, A<string>._)).MustHaveHappened();
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
         The.Action(() => sut.RunBatchAsync(_exportRunOptions)).ShouldThrowAn<OSPSuiteException>();
      }
   }
}