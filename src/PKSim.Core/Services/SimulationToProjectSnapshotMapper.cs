using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using ModelSimulation = PKSim.Core.Model.Simulation;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface ISimulationToProjectSnapshotMapper : IMapper<ModelSimulation, string>;

public class SimulationToProjectSnapshotMapper(
   ISnapshotMapper snapshotMapper,
   IPKSimProjectRetriever pkSimProjectRetriever,
   IJsonSerializer jsonSerializer) : ISimulationToProjectSnapshotMapper
{
   public string MapFrom(ModelSimulation simulation)
   {
      var pkSimProject = createProjectFrom(simulation);
      var projectSnapshot = snapshotMapper.MapToSnapshot(pkSimProject).Result as Project;
      return jsonSerializer.SerializeToBase64String(projectSnapshot);
   }

   private PKSimProject createProjectFrom(ModelSimulation simulation)
   {
      var project = new PKSimProject().WithName(simulation.Name);

      simulation.UsedBuildingBlocks.Select(x => pkSimProjectRetriever.Current.BuildingBlockById(x.TemplateId)).Each(x => addBuildingBlockAndDependents(x, project));
      simulation.UsedObservedData.Select(pkSimProjectRetriever.Current.ObservedDataBy).Each(project.AddObservedData);


      project.AddBuildingBlock(simulation);

      return project;
   }

   private void addBuildingBlockAndDependents(IPKSimBuildingBlock pkSimBuildingBlock, PKSimProject project)
   {
      project.AddBuildingBlock(pkSimBuildingBlock);

      if (pkSimBuildingBlock is Individual individualBuildingBlock)
         individualBuildingBlock.AllExpressionProfiles().Each(project.AddBuildingBlock);
   }
}