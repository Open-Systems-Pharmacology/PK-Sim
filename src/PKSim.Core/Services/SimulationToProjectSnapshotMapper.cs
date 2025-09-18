using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using System.Linq;
using ExpressionProfile = PKSim.Core.Model.ExpressionProfile;
using Individual = PKSim.Core.Model.Individual;
using ModelSimulation = PKSim.Core.Model.Simulation;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface ISimulationToProjectSnapshotMapper : IMapper<ModelSimulation, string>, IMapper<Individual, string>, IMapper<ExpressionProfile, string>;

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
      switch (pkSimBuildingBlock)
      {
         case Individual individualBuildingBlock:
            individualBuildingBlock.AllExpressionProfiles().Each(x => addExpression(project, x));
            project.AddBuildingBlock(individualBuildingBlock);
            return;
         case ExpressionProfile expressionProfile:
            addExpression(project, expressionProfile);
            return;
         default:
            project.AddBuildingBlock(pkSimBuildingBlock);
            return;
      }
   }

   private static void addExpression(PKSimProject project, ExpressionProfile expressionProfile)
   {
      if (!project.All<ExpressionProfile>().ExistsByName(expressionProfile.Name))
         project.AddBuildingBlock(expressionProfile);
   }

   public string MapFrom(Individual individual)
   {
      var snapshot = snapshotMapper.MapToSnapshot(individual).Result as Snapshots.Individual;
      snapshot.ExpressionProfiles = null;
      return jsonSerializer.SerializeToBase64String(snapshot);
   }

   public string MapFrom(ExpressionProfile expressionProfile)
   {
      var snapshot = snapshotMapper.MapToSnapshot(expressionProfile).Result as Snapshots.ExpressionProfile;
      return jsonSerializer.SerializeToBase64String(snapshot);
   }
}