using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Model;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface IProjectSnapshotToModuleMapper : IMapper<string, (Module module, PKSimProject project)>;

public class ProjectSnapshotToModuleMapper(
   IJsonSerializer jsonSerializer,
   ISnapshotMapper snapshotMapper,
   ISimulationConfigurationTask simulationConfigurationTask,
   ISimulationToModelCoreSimulationMapper simulationMapper)
   : SnapshotExchangeMapper<(Module module, PKSimProject project), Project, PKSimProject>(jsonSerializer, snapshotMapper), IProjectSnapshotToModuleMapper
{
   public override (Module module, PKSimProject project) MapFrom(string snapshotString)
   {
      var project = PKSimModelFor(snapshotString);

      if (project.All<Simulation>().Count != 1)
         throw new OSPSuiteException(PKSimConstants.Error.AProjectSnapshotShouldOnlyContainOneSimuilationWhenUsedToRebuildAModule);
      var simulation = project.All<Simulation>().First();

      var configuration = simulationConfigurationTask.CreateFor(simulation, shouldValidate: true, createAgingDataInSimulation: false);
      var modelCoreSimulation = simulationMapper.MapFrom(simulation, configuration, shouldCloneModel: true);

      // The module should contain the snapshot from which it was created
      var module = modelCoreSimulation.Configuration.ModuleConfigurations.First().Module;
      module.Snapshot = snapshotString;

      return (module, project);
   }
}