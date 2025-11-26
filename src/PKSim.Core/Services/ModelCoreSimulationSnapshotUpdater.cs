using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using ExpressionProfile = PKSim.Core.Model.ExpressionProfile;
using Individual = PKSim.Core.Model.Individual;
using ModelSimulation = PKSim.Core.Model.Simulation;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface IModelCoreSimulationSnapshotUpdater
{
   void AddSnapshotsToModelCoreSimulation(ModelSimulation pkSimSimulation, IModelCoreSimulation coreSimulation);
}

public class ModelCoreSimulationSnapshotUpdater(
   ISnapshotMapper snapshotMapper,
   IPKSimProjectRetriever pkSimProjectRetriever,
   IJsonSerializer jsonSerializer) : IModelCoreSimulationSnapshotUpdater
{
   public void AddSnapshotsToModelCoreSimulation(ModelSimulation pkSimSimulation, IModelCoreSimulation coreSimulation)
   {
      var pkSimProject = createProjectFrom(pkSimSimulation);
      var projectSnapshot = snapshotMapper.MapToSnapshot(pkSimProject).Result as Project;

      initializeModuleSnapshots(coreSimulation, projectSnapshot);

      initializeIndividualSnapshots(coreSimulation, projectSnapshot);

      initializeExpressionSnapshots(coreSimulation, projectSnapshot);
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
         case Population population:
            addBuildingBlockAndDependents(population.FirstIndividual, project);
            project.AddBuildingBlock(population);
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

   private void initializeExpressionSnapshots(IModelCoreSimulation coreSimulation, Project projectSnapshot) =>
      coreSimulation.Configuration.ExpressionProfiles.Each(x => x.Snapshot = jsonSerializer.SerializeToBase64String(projectSnapshot.ExpressionProfiles.Single(p => string.Equals(p.ExpressionName, x.Name))));

   private void initializeIndividualSnapshots(IModelCoreSimulation coreSimulation, Project projectSnapshot)
   {
      var individualBuildingBlock = coreSimulation.Configuration.Individual;
      if (individualBuildingBlock == null)
         return;

      var individualSnapshot = projectSnapshot.Individuals.FindByName(individualBuildingBlock.Name);
      
      // clear expression profiles for this snapshot since they are serialized separately
      individualSnapshot.ExpressionProfiles = null;
      
      individualBuildingBlock.Snapshot = jsonSerializer.SerializeToBase64String(individualSnapshot);
   }

   private void initializeModuleSnapshots(IModelCoreSimulation coreSimulation, Project projectSnapshot) =>
      coreSimulation.Configuration.ModuleConfigurations.First().Module.Snapshot = jsonSerializer.SerializeToBase64String(projectSnapshot);
}