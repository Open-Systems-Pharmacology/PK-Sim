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
      Task ExportModelToSnapshot<T>(T modelToExport) where T : class, IObjectBase;

      /// <summary>
      ///    Exports the given <paramref name="modelToExport" /> to snapshot file <paramref name="fileFullPath" />
      /// </summary>
      Task ExportModelToSnapshot<T>(T modelToExport, string fileFullPath) where T : class, IObjectBase;

      /// <summary>
      ///    Exports the given <paramref name="snapshotObject" /> to file. <paramref name="snapshotObject" /> is already a
      ///    snapshot object and won't be mapped to snapshot
      /// </summary>
      Task ExportSnapshot(IWithName snapshotObject);

      Task<IEnumerable<T>> LoadModelsFromSnapshotFile<T>() where T : class, IObjectBase;

      Task<IEnumerable<T>> LoadSnapshots<T>(string fileName);

      Task<IEnumerable<T>> LoadModelsFromSnapshotFile<T>(string fileName) where T : class, IObjectBase;

      Task<PKSimProject> LoadProjectFromSnapshotFile(string fileName);

      Task<PKSimProject> LoadProjectFromSnapshot(Project snapshot);

      Task<T> LoadSnapshotFromFile<T>(string fileName) where T : IWithName;

      /// <summary>
      ///    Returns <c>true</c> if <paramref name="objectToExport" /> was created with a version of PK-Sim fully supporting
      ///    snaphsot (7.3 and higher) otherwise <c>false</c>
      /// </summary>
      bool IsVersionCompatibleWithSnapshotExport<T>(T objectToExport) where T : class, IWithCreationMetaData;
   }

   public class SnapshotTask : ISnapshotTask
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IExecutionContext _executionContext;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly ISnapshotMapper _snapshotMapper;

      public SnapshotTask(IDialogCreator dialogCreator, IJsonSerializer jsonSerializer, ISnapshotMapper snapshotMapper, IExecutionContext executionContext, IObjectTypeResolver objectTypeResolver)
      {
         _dialogCreator = dialogCreator;
         _executionContext = executionContext;
         _objectTypeResolver = objectTypeResolver;
         _jsonSerializer = jsonSerializer;
         _snapshotMapper = snapshotMapper;
      }

      public async Task ExportModelToSnapshot<T>(T modelToExport) where T : class, IObjectBase
      {
         if (modelToExport == null)
            return;

         var fileName = fileNameForExport(modelToExport);
         if (string.IsNullOrEmpty(fileName))
            return;

         await ExportModelToSnapshot(modelToExport, fileName);
      }

      public Task ExportModelToSnapshot<T>(T modelToExport, string fileFullPath) where T : class, IObjectBase
      {
         _executionContext.Load(modelToExport);
         return exportSnapshotFor(modelToExport, fileFullPath);
      }

      private string fileNameForExport(IWithName objectToExport)
      {
         var message = PKSimConstants.UI.SelectSnapshotExportFile(objectToExport.Name, _objectTypeResolver.TypeFor(objectToExport));
         return _dialogCreator.AskForFileToSave(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, objectToExport.Name);
      }

      public async Task ExportSnapshot(IWithName snapshotObject)
      {
         var fileName = fileNameForExport(snapshotObject);
         if (string.IsNullOrEmpty(fileName))
            return;

         await saveSnapshotToFile(snapshotObject, fileName);
      }

      public Task<IEnumerable<T>> LoadModelsFromSnapshotFile<T>() where T : class, IObjectBase
      {
         var fileName = fileNameForSnapshotImport<T>();
         return LoadModelsFromSnapshotFile<T>(fileName);
      }

      private string fileNameForSnapshotImport<T>()
      {
         var message = PKSimConstants.UI.LoadObjectFromSnapshot(_objectTypeResolver.TypeFor<T>());
         return _dialogCreator.AskForFileToOpen(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT);
      }

      public async Task<T> LoadSnapshotFromFile<T>(string fileName) where T : IWithName
      {
         var snapshots = await LoadSnapshots<T>(fileName);
         var snapshot = snapshots.FirstOrDefault();

         if (snapshot != null && string.IsNullOrEmpty(snapshot.Name))
            snapshot.Name = FileHelper.FileNameFromFileFullPath(fileName);

         return snapshot;
      }

      public async Task<IEnumerable<T>> LoadSnapshots<T>(string fileName)
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

      public async Task<IEnumerable<T>> LoadModelsFromSnapshotFile<T>(string fileName) where T : class, IObjectBase
      {
         var snapshotType = _snapshotMapper.SnapshotTypeFor<T>();
         var snapshots = await loadSnapshot(fileName, snapshotType);

         return await loadModelsFromSnapshots<T>(snapshots);
      }

      private async Task<IEnumerable<T>> loadModelsFromSnapshots<T>(IEnumerable<object> snapshots) where T : class, IObjectBase
      {
         if (snapshots == null)
            return Enumerable.Empty<T>();

         var tasks = snapshots.Select(_snapshotMapper.MapToModel);
         var models = await Task.WhenAll(tasks);
         return models.OfType<T>();
      }

      public async Task<PKSimProject> LoadProjectFromSnapshotFile(string fileName)
      {
         var project = (await LoadModelsFromSnapshotFile<PKSimProject>(fileName)).FirstOrDefault();
         return projectWithUpdatedProperties(project, FileHelper.FileNameFromFileFullPath(fileName));
      }

      public async Task<PKSimProject> LoadProjectFromSnapshot(Project snapshot)
      {
         var project = (await loadModelsFromSnapshots<PKSimProject>(new[] {snapshot})).FirstOrDefault();
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

      private async Task exportSnapshotFor<T>(T objectToExport, string fileName)
      {
         var snapshot = await _snapshotMapper.MapToSnapshot(objectToExport);
         await saveSnapshotToFile(snapshot, fileName);
      }

      private Task saveSnapshotToFile(object snapshot, string fileName) => _jsonSerializer.Serialize(snapshot, fileName);
   }
}