using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using ExpressionProfile = PKSim.Core.Model.ExpressionProfile;
using Individual = PKSim.Core.Model.Individual;
using ModelSimulation = PKSim.Core.Model.Simulation;
using SnapshotExpressionProfile = PKSim.Core.Snapshots.ExpressionProfile;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using SnapshotProject = PKSim.Core.Snapshots.Project;

namespace PKSim.Core.Services;

public interface ISnapshotUpdater
{
   void AddSnapshotTo(IndividualBuildingBlock individualBuildingBlock, Individual individual);
   void AddSnapshotTo(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, ExpressionProfile expressionProfile);
   void UpdateSnapshotFromQuery(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, QueryExpressionResults queryResults);

   /// <summary>
   ///    Embeds the snapshots describing <paramref name="pkSimSimulation" /> into <paramref name="coreSimulation" />: a
   ///    project snapshot reduced to the simulation and the building blocks it uses on the module, and the individual and
   ///    expression profile snapshots on their respective building blocks. <paramref name="project" /> is the project the
   ///    simulation and its template building blocks belong to.
   /// </summary>
   void AddSnapshotsToModelCoreSimulation(ModelSimulation pkSimSimulation, IModelCoreSimulation coreSimulation, PKSimProject project);
}

public class SnapshotUpdater(
   ISnapshotMapper snapshotMapper,
   IJsonSerializer jsonSerializer,
   IExpressionProfileUpdater expressionProfileUpdater) : ISnapshotUpdater
{
   public void AddSnapshotTo(IndividualBuildingBlock individualBuildingBlock, Individual individual)
   {
      var snapshot = snapshotFor<SnapshotIndividual>(individual);
      snapshot.ExpressionProfiles = null;
      individualBuildingBlock.Snapshot = jsonSerializer.SerializeToBase64String(snapshot);
   }

   public void AddSnapshotTo(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, ExpressionProfile expressionProfile) =>
      expressionProfileBuildingBlock.Snapshot = jsonSerializer.SerializeToBase64String(snapshotFor<SnapshotExpressionProfile>(expressionProfile));

   public void UpdateSnapshotFromQuery(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, QueryExpressionResults queryResults)
   {
      if (!expressionProfileBuildingBlock.HasSnapshot)
         return;

      var expressionProfile = expressionProfileFrom(expressionProfileBuildingBlock.Snapshot);
      expressionProfileUpdater.UpdateExpressionFromQuery(expressionProfile, queryResults);
      AddSnapshotTo(expressionProfileBuildingBlock, expressionProfile);
   }

   public void AddSnapshotsToModelCoreSimulation(ModelSimulation pkSimSimulation, IModelCoreSimulation coreSimulation, PKSimProject project)
   {
      var projectSnapshot = snapshotFor<SnapshotProject>(createProjectFrom(pkSimSimulation, project));

      coreSimulation.Configuration.ModuleConfigurations.First().Module.Snapshot = jsonSerializer.SerializeToBase64String(projectSnapshot);

      initializeIndividualSnapshot(coreSimulation, projectSnapshot);

      initializeExpressionProfileSnapshots(coreSimulation, projectSnapshot);
   }

   private static PKSimProject createProjectFrom(ModelSimulation simulation, PKSimProject project)
   {
      var reducedProject = new PKSimProject().WithName(simulation.Name);

      simulation.UsedBuildingBlocks.Select(x => project.BuildingBlockById(x.TemplateId)).Each(x => addBuildingBlockAndDependents(x, reducedProject));
      simulation.UsedObservedData.Select(project.ObservedDataBy).Each(reducedProject.AddObservedData);

      reducedProject.AddBuildingBlock(simulation);

      return reducedProject;
   }

   private static void addBuildingBlockAndDependents(IPKSimBuildingBlock pkSimBuildingBlock, PKSimProject project)
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

   private void initializeIndividualSnapshot(IModelCoreSimulation coreSimulation, SnapshotProject projectSnapshot)
   {
      var individualBuildingBlock = coreSimulation.Configuration.Individual;
      if (individualBuildingBlock == null)
         return;

      var individualSnapshot = projectSnapshot.Individuals.FindByName(individualBuildingBlock.Name);

      // clear expression profiles for this snapshot since they are serialized separately
      individualSnapshot.ExpressionProfiles = null;

      individualBuildingBlock.Snapshot = jsonSerializer.SerializeToBase64String(individualSnapshot);
   }

   private void initializeExpressionProfileSnapshots(IModelCoreSimulation coreSimulation, SnapshotProject projectSnapshot) =>
      coreSimulation.Configuration.ExpressionProfiles.Each(x => x.Snapshot = jsonSerializer.SerializeToBase64String(projectSnapshot.ExpressionProfiles.Single(p => string.Equals(p.ExpressionName, x.Name))));

   private TSnapshot snapshotFor<TSnapshot>(object model) where TSnapshot : class => snapshotMapper.MapToSnapshot(model).Result.DowncastTo<TSnapshot>();

   private ExpressionProfile expressionProfileFrom(string base64Snapshot)
   {
      var snapshot = jsonSerializer.DeserializeFromBase64String<SnapshotExpressionProfile>(base64Snapshot).Result;
      return snapshotMapper.MapToModel(snapshot, new ProjectContext(new PKSimProject(), runSimulations: false)).Result.DowncastTo<ExpressionProfile>();
   }
}
