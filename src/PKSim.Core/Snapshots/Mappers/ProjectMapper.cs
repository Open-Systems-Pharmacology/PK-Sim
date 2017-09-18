using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotProject = PKSim.Core.Snapshots.Project;
using ModelProject = PKSim.Core.Model.PKSimProject;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ProjectMapper : ObjectBaseSnapshotMapperBase<ModelProject, SnapshotProject>
   {
      private readonly IExecutionContext _executionContext;

      public ProjectMapper(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
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
         snapshot.ObservedData = await mapObservedDataToSnapshots(project.AllObservedData);

         return snapshot;
      }

      private async Task<DataRepository[]> mapObservedDataToSnapshots(IReadOnlyCollection<OSPSuite.Core.Domain.Data.DataRepository> allObservedData)
      {
         var snapshotMapper = _executionContext.Resolve<ObservedDataMapper>();
         var tasks = allObservedData.Select(datarepository => snapshotMapper.MapToSnapshot(datarepository));
         return await Task.WhenAll(tasks);
      }

      private async Task<T[]> mapBuildingBlocksToSnapshots<T>(IReadOnlyCollection<IPKSimBuildingBlock> buildingBlocks)
      {
         //required to load the snapshot mapper via execution context to avoid circular references
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         var tasks = buildingBlocks.Select(bb => mapBuildingBlockToSnapshot(bb, snapshotMapper));
         var snapshots = await Task.WhenAll(tasks);
         return snapshots.OfType<T>().ToArray();
      }

      private Task<object> mapBuildingBlockToSnapshot(IPKSimBuildingBlock buildingBlock, ISnapshotMapper snapshotMapper)
      {
         _executionContext.Load(buildingBlock);
         return snapshotMapper.MapToSnapshot(buildingBlock);
      }

      public override async Task<ModelProject> MapToModel(SnapshotProject snapshot)
      {
         var project = new ModelProject();
         var buildingBlocks = await allBuidingBlocksFrom(snapshot);
         buildingBlocks.Each(project.AddBuildingBlock);

         var observedData = await observedDataFrom(snapshot);
         observedData.Each(repository => addObservedDataToProject(project, repository));

         return project;
      }

      private static void addObservedDataToProject(ModelProject project, OSPSuite.Core.Domain.Data.DataRepository repository)
      {
         project.AddObservedData(repository);
         project.GetOrCreateClassifiableFor<ClassifiableObservedData, OSPSuite.Core.Domain.Data.DataRepository>(repository);
      }

      private async Task<IEnumerable<OSPSuite.Core.Domain.Data.DataRepository>> observedDataFrom(SnapshotProject snapshot)
      {
         var dataRepositories = await Task.WhenAll(mapSnapshotsToModels(snapshot.ObservedData));
         return dataRepositories.Cast<OSPSuite.Core.Domain.Data.DataRepository>();
      }

      private async Task<IEnumerable<IPKSimBuildingBlock>> allBuidingBlocksFrom(SnapshotProject snapshot)
      {
         var tasks = new List<Task<object>>();
         tasks.AddRange(mapSnapshotsToModels(snapshot.Individuals));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Compounds));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Events));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Formulations));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Protocols));
         tasks.AddRange(mapSnapshotsToModels(snapshot.Populations));

         var buildingBlocks = await Task.WhenAll(tasks);
         return buildingBlocks.Cast<IPKSimBuildingBlock>();
      }

      private IEnumerable<Task<object>> mapSnapshotsToModels(IEnumerable<object> snapshots)
      {
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         return snapshots.Select(snapshotMapper.MapToModel);
      }
   }
}