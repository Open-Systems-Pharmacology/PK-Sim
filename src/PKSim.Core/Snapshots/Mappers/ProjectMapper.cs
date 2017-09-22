﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotProject = PKSim.Core.Snapshots.Project;
using ModelProject = PKSim.Core.Model.PKSimProject;
using ModelObservedDataClassification = OSPSuite.Core.Domain.Classification;
using SnapshotObservedDataClassifiable = PKSim.Core.Snapshots.ObservedDataClassifiable;
using ModelObservedDataClassifiable = OSPSuite.Core.Domain.ClassifiableObservedData;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectMapper : ObjectBaseSnapshotMapperBase<ModelProject, SnapshotProject>
   {
      private readonly IExecutionContext _executionContext;
      private readonly SimulationMapper _simulationMapper;
      private readonly Lazy<ISnapshotMapper> _snapshotMapper;
      private readonly ObservedDataClassificationMapper _observedDataClassificationMapper;

      public ProjectMapper(IExecutionContext executionContext, SimulationMapper simulationMapper, ObservedDataClassificationMapper observedDataClassificationMapper)
      {
         _executionContext = executionContext;
         _simulationMapper = simulationMapper;
         _observedDataClassificationMapper = observedDataClassificationMapper;
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
         snapshot.ObservedDataClassifiables = await mapObservedDataClassifiablesToSnapshots(observedDataClassifiablesFrom<ModelObservedDataClassifiable>(project.AllClassifiables));
         snapshot.ObservedDataClassifications = await mapObservedDataClassificationsToSnapshots(
            observedDataClassifiablesFrom<ModelObservedDataClassification>(project.AllClassifications), 
            observedDataClassifiablesFrom<ModelObservedDataClassifiable>(project.AllClassifiables));

         return snapshot;
      }

      private Task<SnapshotObservedDataClassifiable[]> mapObservedDataClassifiablesToSnapshots(IReadOnlyCollection<ModelObservedDataClassifiable> observedDataClassifiables)
      {
         return mapModelsToSnapshot<ModelObservedDataClassifiable, SnapshotObservedDataClassifiable>(findRoots(observedDataClassifiables), snapshotMapper.MapToSnapshot);
      }

      private IEnumerable<T> findRoots<T>(IEnumerable<T> classifications) where T : IClassifiable
      {
         return classifications.Where(x => x.Parent == null);
      }

      private IReadOnlyList<T> observedDataClassifiablesFrom<T>(IEnumerable<IClassifiable> classifications) where T: IClassifiable
      {
         return classifications.Where(x => x.ClassificationType == ClassificationType.ObservedData).OfType<T>().ToList();
      }

      private Task<ObservedDataClassification[]> mapObservedDataClassificationsToSnapshots(IReadOnlyList<ModelObservedDataClassification> allClassifications, IReadOnlyList<ModelObservedDataClassifiable> allClassifiables)
      {
         var observedDataClassificationsContext = new ObservedDataClassificationContext
         {
            Classifications = allClassifications,
            Classifiables = allClassifiables
         };

         var rootClassifications = findRoots(observedDataClassificationsContext.Classifications);

         var tasks = rootClassifications.Select(x => _observedDataClassificationMapper.MapToSnapshot(x, observedDataClassificationsContext));

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

      public override async Task<ModelProject> MapToModel(SnapshotProject snapshot)
      {
         var project = new ModelProject();

         var buildingBlocks = await allBuidingBlocksFrom(snapshot);
         buildingBlocks.Each(project.AddBuildingBlock);

         var allSimulations = await allSmulationsFrom(snapshot.Simulations, project);
         allSimulations.Each(simulation => addSimulationToProject(project, simulation));

         return project;
      }

      private void addSimulationToProject(ModelProject project, Model.Simulation simulation)
      {
         project.AddBuildingBlock(simulation);
         project.GetOrCreateClassifiableFor<ClassifiableSimulation, Model.Simulation>(simulation);
      }

      private async Task<IEnumerable<Model.Simulation>> allSmulationsFrom(Simulation[] snapshotSimulations, PKSimProject project)
      {
         if (snapshotSimulations == null)
            return Enumerable.Empty<Model.Simulation>();

         var tasks = snapshotSimulations.Select(x => _simulationMapper.MapToModel(x, project));
         return await Task.WhenAll(tasks);
      }

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