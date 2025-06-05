using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Core.Snapshots.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using SnapshotContext = PKSim.Core.Snapshots.Mappers.SnapshotContext;

namespace PKSim.Core.Snapshots.Services;

public interface ISnapshotTask : ISnapshotTask<PKSimProject, Project>
{
   /// <summary>
   ///    Returns <c>true</c> if <paramref name="objectToExport" /> was created with a version of PK-Sim fully supporting
   ///    snapshot (7.3 and higher) otherwise <c>false</c>
   /// </summary>
   bool IsVersionCompatibleWithSnapshotExport<T>(T objectToExport) where T : class, IWithCreationMetaData;

   Task<T> LoadModelFromProjectFileAsync<T>(string fileName, PKSimBuildingBlockType buildingBlockType, string buildingBlockName);
}

public class SnapshotTask : SnapshotTask<PKSimProject, Project>, ISnapshotTask
{
   private readonly IPKSimProjectRetriever _projectRetriever;
   private readonly ProjectMapper _projectMapper;

   public SnapshotTask(
      IDialogCreator dialogCreator,
      IJsonSerializer jsonSerializer,
      ISnapshotMapper snapshotMapper,
      IExecutionContext executionContext,
      IObjectTypeResolver objectTypeResolver,
      IPKSimProjectRetriever projectRetriever,
      ProjectMapper projectMapper) : base(jsonSerializer, snapshotMapper, dialogCreator, objectTypeResolver, executionContext)
   {
      _projectRetriever = projectRetriever;
      _projectMapper = projectMapper;
   }

   public bool IsVersionCompatibleWithSnapshotExport<T>(T objectToExport) where T : class, IWithCreationMetaData
   {
      var projectCreationVersion = objectToExport?.Creation.InternalVersion;
      if (projectCreationVersion == null)
         return false;

      return projectCreationVersion >= ProjectVersions.V7_3_0;
   }

   public async Task<T> LoadModelFromProjectFileAsync<T>(string fileName, PKSimBuildingBlockType buildingBlockType, string buildingBlockName)
   {
      var projectSnapshot = await LoadSnapshotFromFileAsync<Project>(fileName);
      var snapshot = projectSnapshot.BuildingBlockByTypeAndName(buildingBlockType, buildingBlockName);
      return await LoadModelFromSnapshot<T>(snapshot);
   }

   protected override Task<PKSimProject> ProjectFrom(Project snapshot, bool runSimulations) => _projectMapper.MapToModel(snapshot, new ProjectContext(runSimulations));

   protected override OSPSuite.Core.Snapshots.Mappers.SnapshotContext GetSnapshotContext() => new SnapshotContext(_projectRetriever.Current, ProjectVersions.Current);

   protected override PKSimProject GetProject() => _projectRetriever.Current;
}