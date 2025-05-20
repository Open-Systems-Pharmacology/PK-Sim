using System;
using System.Linq;
using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface IProjectSnapshotToSimulationMapper : IMapper<string, IModelCoreSimulation>;

public class ProjectSnapshotToSimulationMapper(
   IJsonSerializer jsonSerializer,
   ISnapshotMapper snapshotMapper,
   ISimulationConfigurationTask simulationConfigurationTask,
   ISimulationToModelCoreSimulationMapper simulationMapper) : IProjectSnapshotToSimulationMapper
{
   public IModelCoreSimulation MapFrom(string snapshotString)
   {
      var snapshot = jsonSerializer.DeserializeFromString(Encoding.UTF8.GetString(Convert.FromBase64String(snapshotString)), typeof(Project)).Result as Project;
      var project = snapshotMapper.MapToModel(snapshot, new ProjectContext(runSimulations: false)).Result as PKSimProject;

      var simulation = project.All<Simulation>().First();

      var configuration = simulationConfigurationTask.CreateFor(simulation, shouldValidate: true, createAgingDataInSimulation: false);
      return simulationMapper.MapFrom(simulation, configuration, shouldCloneModel: true);
   }
}