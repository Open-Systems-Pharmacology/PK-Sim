using System;
using System.Linq;
using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.R.Domain;
using CoreSnapshotTask = PKSim.Core.Snapshots.Services.ISnapshotTask;
using CoreSimulation = PKSim.Core.Model.Simulation;

namespace PKSim.R.Services
{
   public interface ISnapshotTask
   {
      /// <summary>
      ///    Loads the simulations stored in the snapshot file at <paramref name="snapshotFile" />. If
      ///    <paramref name="simulationNames" /> is provided, only the simulations whose names match are returned.
      ///    Matching is ordinal and case-sensitive, consistent with the rest of the OSP suite's name lookups.
      ///    When no name is supplied, every simulation contained in the snapshot is returned (equivalent to opening the
      ///    snapshot in PK-Sim).
      /// </summary>
      Simulation[] LoadSimulationsFromSnapshot(string snapshotFile, params string[] simulationNames);

      /// <summary>
      ///    Runs the snapshot workflow for the given <paramref name="inputFolder" /> and <paramref name="outputFolder" />
      ///    using sensible defaults for the remaining options. Optional <paramref name="folders" /> can be provided to
      ///    process each folder pair instead of the single input/output pair.
      /// </summary>
      void RunSnapshot(string inputFolder, string outputFolder, bool runSimulations = true,
         SnapshotExportMode exportMode = SnapshotExportMode.Snapshot, params string[] folders);
   }

   public class SnapshotTask : ISnapshotTask
   {
      private readonly CoreSnapshotTask _snapshotTask;
      private readonly IBatchRunner<SnapshotRunOptions> _snapshotRunner;

      public SnapshotTask(CoreSnapshotTask snapshotTask, IBatchRunner<SnapshotRunOptions> snapshotRunner)
      {
         _snapshotTask = snapshotTask;
         _snapshotRunner = snapshotRunner;
      }

      public Simulation[] LoadSimulationsFromSnapshot(string snapshotFile, params string[] simulationNames)
      {
         var project = _snapshotTask.LoadProjectFromSnapshotFileAsync(snapshotFile, runSimulations: false).GetAwaiter().GetResult();
         if (project == null)
            return Array.Empty<Simulation>();

         var allSimulations = project.All<CoreSimulation>();

         var matched = simulationNames.Length == 0
            ? allSimulations
            : allSimulations.Where(x => simulationNames.Contains(x.Name));

         return matched.Select(x => new Simulation(x)).ToArray();
      }

      public void RunSnapshot(string inputFolder, string outputFolder, bool runSimulations = true,
         SnapshotExportMode exportMode = SnapshotExportMode.Snapshot, params string[] folders)
      {
         var runOptions = new SnapshotRunOptions
         {
            InputFolder = inputFolder,
            OutputFolder = outputFolder,
            RunSimulations = runSimulations,
            ExportMode = exportMode,
            Folders = folders
         };

         _snapshotRunner.RunBatchAsync(runOptions).GetAwaiter().GetResult();
      }
   }
}
