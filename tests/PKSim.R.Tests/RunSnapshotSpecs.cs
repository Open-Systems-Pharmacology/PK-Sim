using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Utility;

namespace PKSim.R;

internal class RunSnapshotSpecs : ContextForStaticIntegration
{
   private readonly string _inputFolder = @"C:\Input\";
   private readonly string _outputFolder = @"C:\Output\";

   protected override void Context()
   {
      base.Context();
      var runOptions = new SnapshotRunOptions
      {
         InputFolder = _inputFolder,
         OutputFolder = _outputFolder,
         ExportMode = SnapshotExportMode.Project
      };

      DirectoryHelper.CreateDirectory(_inputFolder);
      FileHelper.Copy(DomainHelperForSpecs.DataFilePathFor("Atazanavir-Model_1Sim.json"), Path.Join(_inputFolder, "Atazanavir-Model_1Sim.json"));

      Api.RunSnapshot(runOptions);
   }

   [Observation]
   public void the_output_folder_contains_one_pksim5_file()
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