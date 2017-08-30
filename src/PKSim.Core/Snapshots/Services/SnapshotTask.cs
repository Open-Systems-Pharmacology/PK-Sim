using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotTask
   {
      /// <summary>
      ///    Exports the given <paramref name="objectToExport" /> to snapshot. User will be ask to specify the file where the
      ///    snapshot will be exported
      /// </summary>
      void ExportSnapshot<T>(T objectToExport) where T : class, IObjectBase;

      IEnumerable<T> LoadFromSnapshot<T>(string discriminator) where T : class, IObjectBase;
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

      public void ExportSnapshot<T>(T objectToExport) where T : class, IObjectBase
      {
         if (objectToExport == null)
            return;

         var message = PKSimConstants.UI.SelectSnapshotExportFile(objectToExport.Name, _objectTypeResolver.TypeFor(objectToExport));
         var fileName = _dialogCreator.AskForFileToSave(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, objectToExport.Name);
         if (string.IsNullOrEmpty(fileName))
            return;

         _executionContext.Load(objectToExport);

         exportSnapshotFor(objectToExport, fileName);
      }

      public IEnumerable<T> LoadFromSnapshot<T>(string discriminator) where T : class, IObjectBase
      {
         var message = PKSimConstants.UI.LoadFromSnapshotFile(_objectTypeResolver.TypeFor<T>());
         var fileName = _dialogCreator.AskForFileToOpen(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT);
         if (string.IsNullOrEmpty(fileName))
            return Enumerable.Empty<T>();

         var snapshotType = _snapshotMapper.SnapshotTypeFor<T>();

         var snapshots = _snapshotSerializer.DeserializeAsArray(fileName, snapshotType);
         if (snapshots == null)
            return Enumerable.Empty<T>();

         return snapshots.Select(_snapshotMapper.MapToModel).Cast<T>();
      }

      private void exportSnapshotFor<T>(T objectToExport, string fileName)
      {
         var snapshot = _snapshotMapper.MapToSnapshot(objectToExport);
         _snapshotSerializer.Serialize(snapshot, fileName);
      }
   }
}