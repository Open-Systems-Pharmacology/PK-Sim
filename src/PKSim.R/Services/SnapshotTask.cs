using System;
using System.Linq;
using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.R.Domain;
using PKSim.Core.Services;
using PKSim.R.Mappers;
using CoreSnapshotTask = PKSim.Core.Snapshots.Services.ISnapshotTask;
using CoreSimulation = PKSim.Core.Model.Simulation;
using CoreAgingData = PKSim.Core.Model.AgingData;
using PopulationSimulation = PKSim.Core.Model.PopulationSimulation;

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
      private readonly ISimulationConfigurationTask _simulationConfigurationTask;
      private readonly ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private readonly IPopulationSimulationToIndividualValuesCacheMapper _populationCacheMapper;
      private readonly IAgingDataMapper _agingDataMapper;

      public SnapshotTask(CoreSnapshotTask snapshotTask,
         IBatchRunner<SnapshotRunOptions> snapshotRunner,
         ISimulationConfigurationTask simulationConfigurationTask,
         ISimulationToModelCoreSimulationMapper modelCoreSimulationMapper,
         IPopulationSimulationToIndividualValuesCacheMapper populationCacheMapper,
         IAgingDataMapper agingDataMapper)
      {
         _snapshotTask = snapshotTask;
         _snapshotRunner = snapshotRunner;
         _simulationConfigurationTask = simulationConfigurationTask;
         _modelCoreSimulationMapper = modelCoreSimulationMapper;
         _populationCacheMapper = populationCacheMapper;
         _agingDataMapper = agingDataMapper;
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

         return matched.Select(simulationFrom).ToArray();
      }

      private Simulation simulationFrom(CoreSimulation simulation)
      {
         var rSimulation = new Simulation(coreSimulationFrom(simulation));

         if (simulation is PopulationSimulation populationSimulation)
         {
            rSimulation.IndividualValuesCache = _populationCacheMapper.MapFrom(populationSimulation);

            if (shouldExportAgingData(populationSimulation.AgingData))
               rSimulation.AgingData = _agingDataMapper.MapFrom(populationSimulation.AgingData);
         }

         return rSimulation;
      }

      private IModelCoreSimulation coreSimulationFrom(CoreSimulation simulation)
      {
         var simulationConfiguration = _simulationConfigurationTask.CreateFor(simulation, shouldValidate: true, createAgingDataInSimulation: false);
         return _modelCoreSimulationMapper.MapFrom(simulation, simulationConfiguration, shouldCloneModel: false);
      }

      private static bool shouldExportAgingData(CoreAgingData agingData) =>
         agingData != null && agingData.AllParameterData.Any(x => x.IndividualIndexes.Count > 0);

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
