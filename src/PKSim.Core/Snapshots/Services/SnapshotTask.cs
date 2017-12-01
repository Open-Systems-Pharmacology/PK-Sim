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

      Task<IEnumerable<T>> LoadModelFromSnapshot<T>() where T : class, IObjectBase;

      Task<IEnumerable<T>> LoadSnapshot<T>() where T : IWithName;

      Task<IEnumerable<T>> LoadModelFromSnapshot<T>(string fileName) where T : class, IObjectBase;

      Task<PKSimProject> LoadProjectFromSnapshot(string fileName);
   }

   public class SnapshotTask : ISnapshotTask
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IExecutionContext _executionContext;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly ISnapshotSerializer _snapshotSerializer;
      private readonly ISnapshotMapper _snapshotMapper;

      public SnapshotTask(IDialogCreator dialogCreator, ISnapshotSerializer snapshotSerializer, ISnapshotMapper snapshotMapper, IExecutionContext executionContext, IObjectTypeResolver objectTypeResolver)
      {
         _dialogCreator = dialogCreator;
         _executionContext = executionContext;
         _objectTypeResolver = objectTypeResolver;
         _snapshotSerializer = snapshotSerializer;
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

      public Task<IEnumerable<T>> LoadModelFromSnapshot<T>() where T : class, IObjectBase
      {
         var fileName = fileNameForSnapshotImport<T>();
         return LoadModelFromSnapshot<T>(fileName);
      }

      private string fileNameForSnapshotImport<T>()
      {
         var message = PKSimConstants.UI.LoadObjectFromSnapshot(_objectTypeResolver.TypeFor<T>());
         return _dialogCreator.AskForFileToOpen(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT);
      }

      public async Task<IEnumerable<T>> LoadSnapshot<T>() where T : IWithName
      {
         var fileName = fileNameForSnapshotImport<T>();
         var snapshots = await loadSnapshot(fileName, typeof(T));
         return snapshots.OfType<T>();
      }

      private async Task<IEnumerable<object>> loadSnapshot(string fileName, Type snapshotType)
      {
         if (string.IsNullOrEmpty(fileName))
            return Enumerable.Empty<object>();

         return await _snapshotSerializer.DeserializeAsArray(fileName, snapshotType);
      }

      public async Task<IEnumerable<T>> LoadModelFromSnapshot<T>(string fileName) where T : class, IObjectBase
      {
         var snapshotType = _snapshotMapper.SnapshotTypeFor<T>();
         var snapshots = await loadSnapshot(fileName, snapshotType);

         if (snapshots == null)
            return Enumerable.Empty<T>();

         var tasks = snapshots.Select(_snapshotMapper.MapToModel);
         var models = await Task.WhenAll(tasks);
         return models.OfType<T>();
      }

      public async Task<PKSimProject> LoadProjectFromSnapshot(string fileName)
      {
         var project = (await LoadModelFromSnapshot<PKSimProject>(fileName)).FirstOrDefault();

         if (project == null)
            return null;

         project.HasChanged = true;
         project.Name = FileHelper.FileNameFromFileFullPath(fileName);
         return project;
      }

      private async Task exportSnapshotFor<T>(T objectToExport, string fileName)
      {
         var snapshot = await _snapshotMapper.MapToSnapshot(objectToExport);
         await saveSnapshotToFile(snapshot, fileName);
      }

      private Task saveSnapshotToFile(object snapshot, string fileName) => _snapshotSerializer.Serialize(snapshot, fileName);
   }
}