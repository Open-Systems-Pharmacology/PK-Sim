using System.Linq;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Serialization.Exchange;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Model;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface IProjectSnapshotToSimulationTransferMapper : IMapper<string, SimulationTransfer>;

public class ProjectSnapshotToSimulationTransferMapper(
   IJsonSerializer jsonSerializer,
   ISnapshotMapper snapshotMapper,
   ISimulationConfigurationTask simulationConfigurationTask,
   ISimulationToModelCoreSimulationMapper simulationMapper) : IProjectSnapshotToSimulationTransferMapper
{
   public SimulationTransfer MapFrom(string snapshotString)
   {
      var snapshot = jsonSerializer.DeserializeFromBase64String<Project>(snapshotString).Result;
      var project = snapshotMapper.MapToModel(snapshot, new ProjectContext(new PKSimProject(), runSimulations: false)).Result as PKSimProject;

      if (project.All<Simulation>().Count != 1)
         throw new OSPSuiteException(PKSimConstants.Error.AProjectSnapshotShouldOnlyContainOneSimuilationWhenUsedToRebuildAModule);
      var simulation = project.All<Simulation>().First();

      var configuration = simulationConfigurationTask.CreateFor(simulation, shouldValidate: true, createAgingDataInSimulation: false);
      var modelCoreSimulation = simulationMapper.MapFrom(simulation, configuration, shouldCloneModel: true);

      // The module should contain the snapshot from which it was created
      modelCoreSimulation.Configuration.ModuleConfigurations.First().Module.Snapshot = snapshotString;

      return new SimulationTransfer
      {
         Simulation = modelCoreSimulation
      };
   }
}