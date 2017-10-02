using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotProject = PKSim.Core.Snapshots.Project;
using ModelProject = PKSim.Core.Model.PKSimProject;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using SnapshotDataRepository = PKSim.Core.Snapshots.DataRepository;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectMapper : ObjectBaseSnapshotMapperBase<ModelProject, SnapshotProject>
   {
      private readonly IExecutionContext _executionContext;
      private readonly SimulationMapper _simulationMapper;
      private readonly SimulationComparisonMapper _simulationComparisonMapper;
      private readonly IClassificationSnapshotTask _classificationSnapshotTask;
      private readonly Lazy<ISnapshotMapper> _snapshotMapper;

      public ProjectMapper(SimulationMapper simulationMapper, SimulationComparisonMapper simulationComparisonMapper, IExecutionContext executionContext, IClassificationSnapshotTask classificationSnapshotTask)
      {
         _executionContext = executionContext;
         _simulationMapper = simulationMapper;
         _simulationComparisonMapper = simulationComparisonMapper;
         _classificationSnapshotTask = classificationSnapshotTask;
         //required to load the snapshot mapper via execution context to avoid circular references
         _snapshotMapper = new Lazy<ISnapshotMapper>(() => _executionContext.Resolve<ISnapshotMapper>());
      }

      public override async Task<SnapshotProject> MapToSnapshot(ModelProject project)
      {
         var snapshot = await SnapshotFrom(project);
         snapshot.Individuals = await mapBuildingBlocksToSnapshots<Individual>(project.All<Model.Individual>());
         snapshot.Compounds = await mapBuildingBlocksToSnapshots<Compound>(project.All<Model.Compound>());
         snapshot.Events = await mapBuildingBlocksToSnapshots<Event>(project.All<PKSimEvent>());
         snapshot.Formulations = await mapBuildingBlocksToSnapshots<Formulation>(project.All<Model.Formulation>());
         snapshot.Protocols = await mapBuildingBlocksToSnapshots<Protocol>(project.All<Model.Protocol>());
         snapshot.Populations = await mapBuildingBlocksToSnapshots<Population>(project.All<Model.Population>());
         snapshot.Simulations = await mapSimulationsToSnapshots(project.All<Model.Simulation>(), project);
         snapshot.ObservedData = await mapObservedDataToSnapshots(project.AllObservedData);
         snapshot.SimulationComparisons = await mapSimulationComparisonsToSnapshots(project.AllSimulationComparisons);

         snapshot.ObservedDataClassifications = await mapObservedDataClassificationsToSnapshots(project);
         snapshot.SimulationComparisonClassifications = await mapComparisonClassificationsToSnapshots(project);
         snapshot.SimulationClassifications = await mapSimulationClassificationToSnapshots(project);
         return snapshot;
      }

      public override async Task<ModelProject> MapToModel(SnapshotProject snapshot)
      {
         var project = new ModelProject();

         var buildingBlocks = await allBuidingBlocksFrom(snapshot);
         buildingBlocks.Each(project.AddBuildingBlock);

         var observedData = await observedDataFrom(snapshot.ObservedData);
         observedData.Each(repository => addObservedDataToProject(project, repository));

         var allSimulations = await allSmulationsFrom(snapshot.Simulations, project);
         allSimulations.Each(simulation => addSimulationToProject(project, simulation));

         var allSimulationComparisons = await allSimulationComparisonsFrom(snapshot.SimulationComparisons, project);
         allSimulationComparisons.Each(comparison => addComparisonToProject(project, comparison));

         //Map all classifications once project have been loaded
         await updateProjectClassification(snapshot, project);

         return project;
      }

      private Task<Classification[]> mapSimulationClassificationToSnapshots(ModelProject project)
      {
         return _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableSimulation>(project);
      }

      private Task<Classification[]> mapObservedDataClassificationsToSnapshots(ModelProject project)
      {
         return _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableObservedData>(project);
      }

      private Task<Classification[]> mapComparisonClassificationsToSnapshots(ModelProject project)
      {
         return _classificationSnapshotTask.MapClassificationsToSnapshots<ClassifiableComparison>(project);
      }

      private Task<SnapshotDataRepository[]> mapObservedDataToSnapshots(IReadOnlyCollection<ModelDataRepository> allObservedData)
      {
         return mapModelsToSnapshot<ModelDataRepository, SnapshotDataRepository>(allObservedData, snapshotMapper.MapToSnapshot);
      }

      private Task<IEnumerable<ModelDataRepository>> observedDataFrom(SnapshotDataRepository[] snapshotRepositories)
      {
         return awaitAs<ModelDataRepository>(mapSnapshotsToModels(snapshotRepositories));
      }

      private Task<ISimulationComparison[]> allSimulationComparisonsFrom(SimulationComparison[] snapshotSimulationComparisons, ModelProject project) => _simulationComparisonMapper.MapToModels(snapshotSimulationComparisons, project);

      private Task<SimulationComparison[]> mapSimulationComparisonsToSnapshots(IReadOnlyCollection<ISimulationComparison> allSimulationComparisons)
      {
         var tasks = allSimulationComparisons.Select(mapSimulationComparisonToSnapshot);
         return Task.WhenAll(tasks);
      }

      private Task<Simulation[]> mapSimulationsToSnapshots(IReadOnlyCollection<Model.Simulation> allSimulations, ModelProject project)
      {
         var tasks = allSimulations.Select(x => mapSimulationToSnapshot(x, project));
         return Task.WhenAll(tasks);
      }

      private Task<Simulation> mapSimulationToSnapshot(Model.Simulation simulation, ModelProject project)
      {
         _executionContext.Load(simulation);
         return _simulationMapper.MapToSnapshot(simulation, project);
      }

      private Task<SimulationComparison> mapSimulationComparisonToSnapshot(ISimulationComparison simulationComparison)
      {
         _executionContext.Load(simulationComparison);
         return _simulationComparisonMapper.MapToSnapshot(simulationComparison);
      }

      private Task<T[]> mapBuildingBlocksToSnapshots<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
      {
         return mapModelsToSnapshot<IPKSimBuildingBlock, T>(buildingBlocks, mapBuildingBlockToSnapshot);
      }

      private Task<object> mapBuildingBlockToSnapshot(IPKSimBuildingBlock buildingBlock)
      {
         _executionContext.Load(buildingBlock);
         return snapshotMapper.MapToSnapshot(buildingBlock);
      }

      private async Task<TSnapshot[]> mapModelsToSnapshot<TModel, TSnapshot>(IEnumerable<TModel> models, Func<TModel, Task<object>> mapFunc)
      {
         var tasks = models.Select(mapFunc);
         var snapshots = await awaitAs<TSnapshot>(tasks);
         return snapshots.ToArray();
      }

      private Task updateProjectClassification(SnapshotProject snapshot, ModelProject project)
      {
         var tasks = new[]
         {
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableObservedData, ModelDataRepository>(snapshot.ObservedDataClassifications, project, project.AllObservedData),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableSimulation, Model.Simulation>(snapshot.SimulationClassifications, project, project.All<Model.Simulation>()),
            _classificationSnapshotTask.UpdateProjectClassifications<ClassifiableComparison, ISimulationComparison>(snapshot.SimulationComparisonClassifications, project, project.AllSimulationComparisons),
         };

         return Task.WhenAll(tasks);
      }

      private void addSimulationToProject(ModelProject project, Model.Simulation simulation)
      {
         addClassifiableToProject<ClassifiableSimulation, Model.Simulation>(project, simulation, project.AddBuildingBlock);
      }

      private void addObservedDataToProject(ModelProject project, ModelDataRepository repository)
      {
         addClassifiableToProject<ClassifiableObservedData, ModelDataRepository>(project, repository, project.AddObservedData);
      }

      private void addComparisonToProject(ModelProject project, ISimulationComparison simulationComparison)
      {
         addClassifiableToProject<ClassifiableComparison, ISimulationComparison>(project, simulationComparison, project.AddSimulationComparison);
      }

      private void addClassifiableToProject<TClassifiableWrapper, TSubject>(ModelProject project, TSubject subject, Action<TSubject> addToProjectAction) where TClassifiableWrapper : Classifiable<TSubject>, new() where TSubject : IWithId, IWithName
      {
         addToProjectAction(subject);
         project.GetOrCreateClassifiableFor<TClassifiableWrapper, TSubject>(subject);
      }

      private Task<Model.Simulation[]> allSmulationsFrom(Simulation[] snapshotSimulations, ModelProject project) => _simulationMapper.MapToModels(snapshotSimulations, project);

      private Task<IEnumerable<IPKSimBuildingBlock>> allBuidingBlocksFrom(SnapshotProject snapshot)
      {
         var tasks = new List<Task<object>>();
         tasks.AddRange(mapSnapshotsToModels(snapshot.Individuals));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Compounds));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Events));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Formulations));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Protocols));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Populations));
         return awaitAs<IPKSimBuildingBlock>(tasks);
      }

      private async Task<IEnumerable<T>> awaitAs<T>(IEnumerable<Task<object>> tasks)
      {
         var models = await Task.WhenAll(tasks);
         return models.Cast<T>();
      }

      private IEnumerable<Task<object>> mapSnapshotsToModels(IEnumerable<object> snapshots)
      {
         if (snapshots == null)
            return Enumerable.Empty<Task<object>>();

         return snapshots.Select(snapshotMapper.MapToModel);
      }

      private ISnapshotMapper snapshotMapper => _snapshotMapper.Value;
   }
}