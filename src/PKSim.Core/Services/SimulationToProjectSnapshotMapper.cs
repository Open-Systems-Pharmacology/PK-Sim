using System;
using System.Linq;
using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using ModelSimulation = PKSim.Core.Model.Simulation;

namespace PKSim.Core.Services;

public interface ISimulationToProjectSnapshotMapper : IMapper<ModelSimulation, string>;

public class SimulationToProjectSnapshotSnapshotMapper(
   IJsonSerializer jsonSerializer,
   ISnapshotMapper snapshotMapper,
   IPKSimProjectRetriever pkSimProjectRetriever) : ISimulationToProjectSnapshotMapper
{
   public string MapFrom(ModelSimulation simulation)
   {
      var pkSimProject = createProjectFrom(simulation);
      var projectSnapshot = snapshotMapper.MapToSnapshot(pkSimProject).Result;
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonSerializer.Serialize(projectSnapshot)));
   }

   private PKSimProject createProjectFrom(ModelSimulation simulation)
   {
      var project = new PKSimProject().WithName(simulation.Name);

      simulation.UsedBuildingBlocks.Select(x => pkSimProjectRetriever.Current.BuildingBlockById(x.TemplateId)).Each(project.AddBuildingBlock);
      simulation.UsedObservedData.Select(pkSimProjectRetriever.Current.ObservedDataBy).Each(project.AddObservedData);

      project.AddBuildingBlock(simulation);

      return project;
   }
}