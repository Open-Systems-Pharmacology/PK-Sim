using System;
using System.IO;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.R.Domain;
using OSPSuite.Utility;
using PKSim.R.Services;

namespace PKSim.R
{
   public abstract class concern_for_SnapshotTask : ContextForIntegration<ISnapshotTask>
   {
      protected string _snapshotFile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = Api.GetSnapshotTask();
      }

      protected override void Context()
      {
         base.Context();
         _snapshotFile = DomainHelperForSpecs.DataFilePathFor("Atazanavir-Model_1Sim.json");
      }
   }

   public class When_loading_all_simulations_from_a_snapshot : concern_for_SnapshotTask
   {
      private Simulation[] _simulations;

      protected override void Because()
      {
         _simulations = sut.LoadSimulationsFromSnapshot(_snapshotFile);
      }

      [Observation]
      public void should_return_every_simulation_defined_in_the_snapshot()
      {
         _simulations.ShouldNotBeNull();
         _simulations.Length.ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_return_simulations_that_carry_a_simulation_configuration()
      {
         _simulations.All(x => x.Configuration != null).ShouldBeTrue();
      }

      [Observation]
      public void should_return_simulations_that_wrap_a_model_core_simulation()
      {
         _simulations.All(x => x.CoreSimulation is ModelCoreSimulation).ShouldBeTrue();
      }
   }

   public class When_loading_simulations_filtered_by_an_existing_simulation_name : concern_for_SnapshotTask
   {
      private Simulation[] _simulations;
      private string _existingSimulationName;

      protected override void Context()
      {
         base.Context();
         _existingSimulationName = sut.LoadSimulationsFromSnapshot(_snapshotFile).First().Name;
      }

      protected override void Because()
      {
         _simulations = sut.LoadSimulationsFromSnapshot(_snapshotFile, _existingSimulationName);
      }

      [Observation]
      public void should_return_only_simulations_whose_name_matches()
      {
         _simulations.Length.ShouldBeEqualTo(1);
         _simulations[0].Name.ShouldBeEqualTo(_existingSimulationName);
      }
   }

   public class When_loading_simulations_filtered_by_a_name_that_does_not_exist : concern_for_SnapshotTask
   {
      private Simulation[] _simulations;

      protected override void Because()
      {
         _simulations = sut.LoadSimulationsFromSnapshot(_snapshotFile, "ThisSimulationDoesNotExist");
      }

      [Observation]
      public void should_return_an_empty_array()
      {
         _simulations.Length.ShouldBeEqualTo(0);
      }
   }

   public class When_running_a_snapshot_export_using_the_convenient_method : concern_for_SnapshotTask
   {
      private readonly string _inputFolder = Path.Combine(Path.GetTempPath(), $"PKSim_SnapshotTask_Input_{Guid.NewGuid():N}");
      private readonly string _outputFolder = Path.Combine(Path.GetTempPath(), $"PKSim_SnapshotTask_Output_{Guid.NewGuid():N}");

      protected override void Context()
      {
         base.Context();
         DirectoryHelper.CreateDirectory(_inputFolder);
         FileHelper.Copy(_snapshotFile, Path.Join(_inputFolder, "Atazanavir-Model_1Sim.json"));
      }

      protected override void Because()
      {
         sut.RunSnapshot(_inputFolder, _outputFolder, exportMode: SnapshotExportMode.Project);
      }

      [Observation]
      public void should_write_the_pksim5_project_file_to_the_output_folder()
      {
         Directory.Exists(_outputFolder).ShouldBeTrue();
         File.Exists(Path.Join(_outputFolder, "Atazanavir-Model_1Sim.pksim5")).ShouldBeTrue();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         DirectoryHelper.DeleteDirectory(_outputFolder, true);
         DirectoryHelper.DeleteDirectory(_inputFolder, true);
      }
   }
}
