using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotTask
   {
      /// <summary>
      ///    Exports the given <paramref name="modelToExport" /> to snapshot. User will be ask to specify the file where the
      ///    snapshot will be exported
      /// </summary>
      Task ExportModelToSnapshotAsync<T>(T modelToExport) where T : class, IObjectBase;

      /// <summary>
      ///    Exports the given <paramref name="modelToExport" /> to snapshot file <paramref name="fileFullPath" />
      /// </summary>
      Task ExportModelToSnapshotAsync<T>(T modelToExport, string fileFullPath) where T : class, IObjectBase;

      /// <summary>
      ///    Exports the given <paramref name="snapshotObject" /> to file. <paramref name="snapshotObject" /> is already a
      ///    snapshot object and won't be mapped to snapshot
      /// </summary>
      Task ExportSnapshotAsync(IWithName snapshotObject);

      Task<IEnumerable<T>> LoadModelsFromSnapshotFileAsync<T>() where T : class, IObjectBase;

      Task<IEnumerable<T>> LoadSnapshotsAsync<T>(string fileName);

      Task<IEnumerable<T>> LoadModelsFromSnapshotFileAsync<T>(string fileName) where T : class;

      Task<PKSimProject> LoadProjectFromSnapshotFileAsync(string fileName, bool runSimulations = true);

      Task<PKSimProject> LoadProjectFromSnapshotAsync(Project snapshot, bool runSimulations);

      Task<T> LoadSnapshotFromFileAsync<T>(string fileName) where T : IWithName;

      /// <summary>
      ///    Returns <c>true</c> if <paramref name="objectToExport" /> was created with a version of PK-Sim fully supporting
      ///    snapshot (7.3 and higher) otherwise <c>false</c>
      /// </summary>
      bool IsVersionCompatibleWithSnapshotExport<T>(T objectToExport) where T : class, IWithCreationMetaData;

      Task<T> LoadModelFromProjectFileAsync<T>(string fileName, PKSimBuildingBlockType buildingBlockType, string buildingBlockName);
   }

   public class SnapshotTask : ISnapshotTask
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IExecutionContext _executionContext;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly ProjectMapper _projectMapper;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly ISnapshotMapper _snapshotMapper;

      public SnapshotTask(
         IDialogCreator dialogCreator,
         IJsonSerializer jsonSerializer,
         ISnapshotMapper snapshotMapper,
         IExecutionContext executionContext,
         IObjectTypeResolver objectTypeResolver,
         ProjectMapper projectMapper)
      {
         _dialogCreator = dialogCreator;
         _executionContext = executionContext;
         _objectTypeResolver = objectTypeResolver;
         _projectMapper = projectMapper;
         _jsonSerializer = jsonSerializer;
         _snapshotMapper = snapshotMapper;
      }

      public async Task ExportModelToSnapshotAsync<T>(T modelToExport) where T : class, IObjectBase
      {
         if (modelToExport == null)
            return;

         var fileName = fileNameForExport(modelToExport);
         if (string.IsNullOrEmpty(fileName))
            return;

         await ExportModelToSnapshotAsync(modelToExport, fileName);
      }

      public Task ExportModelToSnapshotAsync<T>(T modelToExport, string fileFullPath) where T : class, IObjectBase
      {
         _executionContext.Load(modelToExport);
         return exportSnapshotFor(modelToExport, fileFullPath);
      }

      private string fileNameForExport(IWithName objectToExport)
      {
         var message = PKSimConstants.UI.SelectSnapshotExportFile(objectToExport.Name, _objectTypeResolver.TypeFor(objectToExport));
         return _dialogCreator.AskForFileToSave(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, objectToExport.Name);
      }

      public async Task ExportSnapshotAsync(IWithName snapshotObject)
      {
         var fileName = fileNameForExport(snapshotObject);
         if (string.IsNullOrEmpty(fileName))
            return;

         await saveSnapshotToFile(snapshotObject, fileName);
      }

      public Task<IEnumerable<T>> LoadModelsFromSnapshotFileAsync<T>() where T : class, IObjectBase
      {
         var fileName = fileNameForSnapshotImport<T>();
         return LoadModelsFromSnapshotFileAsync<T>(fileName);
      }

      private string fileNameForSnapshotImport<T>()
      {
         var message = PKSimConstants.UI.LoadObjectFromSnapshot(_objectTypeResolver.TypeFor<T>());
         return _dialogCreator.AskForFileToOpen(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT);
      }

      public async Task<T> LoadSnapshotFromFileAsync<T>(string fileName) where T : IWithName
      {
         var snapshots = await LoadSnapshotsAsync<T>(fileName);
         var snapshot = snapshots.FirstOrDefault();

         if (snapshot != null && string.IsNullOrEmpty(snapshot.Name))
            snapshot.Name = FileHelper.FileNameFromFileFullPath(fileName);

         return snapshot;
      }

      public async Task<IEnumerable<T>> LoadSnapshotsAsync<T>(string fileName)
      {
         var snapshots = await loadSnapshot(fileName, typeof(T));
         return snapshots.OfType<T>();
      }

      private async Task<IEnumerable<object>> loadSnapshot(string fileName, Type snapshotType)
      {
         if (string.IsNullOrEmpty(fileName))
            return Enumerable.Empty<object>();

         return await _jsonSerializer.DeserializeAsArray(fileName, snapshotType);
      }

      public async Task<IEnumerable<T>> LoadModelsFromSnapshotFileAsync<T>(string fileName) where T : class
      {
         var snapshotType = _snapshotMapper.SnapshotTypeFor<T>();
         var snapshots = await loadSnapshot(fileName, snapshotType);

         return await loadModelsFromSnapshotsAsync<T>(snapshots);
      }

      private async Task<IEnumerable<T>> loadModelsFromSnapshotsAsync<T>(IEnumerable<object> snapshots) 
      {
         if (snapshots == null)
            return Enumerable.Empty<T>();

         var tasks = snapshots.Select(_snapshotMapper.MapToModel);
         var models = await Task.WhenAll(tasks);
         return models.OfType<T>();
      }

      private async Task<T> loadModelFromSnapshot<T>(object snapshot) 
      {
         if (snapshot == null)
            return default(T);

         var models = await loadModelsFromSnapshotsAsync<T>(new[] {snapshot});
         return models.FirstOrDefault();
      }

      public async Task<PKSimProject> LoadProjectFromSnapshotFileAsync(string fileName, bool runSimulations = true)
      {
         var projectSnapshot = await LoadSnapshotFromFileAsync<Project>(fileName);
         var project = await LoadProjectFromSnapshotAsync(projectSnapshot, runSimulations);
         return projectWithUpdatedProperties(project, FileHelper.FileNameFromFileFullPath(fileName));
      }

      public async Task<PKSimProject> LoadProjectFromSnapshotAsync(Project snapshot, bool runSimulations)
      {
         var project = await _projectMapper.MapToModel(snapshot, new ProjectContext(runSimulations));
         return projectWithUpdatedProperties(project, snapshot?.Name);
      }

      private PKSimProject projectWithUpdatedProperties(PKSimProject project, string name)
      {
         if (project == null)
            return null;

         project.HasChanged = true;
         project.Name = name;
         return project;
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
         return await loadModelFromSnapshot<T>(snapshot);
      }

      private async Task exportSnapshotFor<T>(T objectToExport, string fileName)
      {
         var snapshot = await _snapshotMapper.MapToSnapshot(objectToExport);
         await saveSnapshotToFile(snapshot, fileName);
      }

      private Task saveSnapshotToFile(object snapshot, string fileName) => _jsonSerializer.Serialize(snapshot, fileName);
   }
}