using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotTask
   {
      void ExportSnapshot<T>(T objectToExport) where T : class, IObjectBase;
   }

   public class SnapshotTask : ISnapshotTask
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IExecutionContext _executionContext;
      private readonly ISnapshotSerializer _snapshotSerializer;
      private readonly ISnapshotMapper _snapshotMapper;

      public SnapshotTask(IDialogCreator dialogCreator, ISnapshotSerializer snapshotSerializer, ISnapshotMapper snapshotMapper, IExecutionContext executionContext)
      {
         _dialogCreator = dialogCreator;
         _executionContext = executionContext;
         _snapshotSerializer = snapshotSerializer;
         _snapshotMapper = snapshotMapper;
      }

      public void ExportSnapshot<T>(T objectToExport) where T : class, IObjectBase
      {
         if (objectToExport == null)
            return;

         var message = PKSimConstants.UI.SelectSnapshotExportFile(objectToExport.Name, _executionContext.TypeFor(objectToExport));
         var fileName = _dialogCreator.AskForFileToSave(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT, objectToExport.Name);
         if (string.IsNullOrEmpty(fileName))
            return;

         _executionContext.Load(objectToExport);

         exportSnapshotFor(objectToExport, fileName);
      }

      private void exportSnapshotFor<T>(T objectToExport, string fileName)
      {
         var snapshot = _snapshotMapper.MapToSnapshot(objectToExport);
         _snapshotSerializer.Serialize(snapshot, fileName);
      }
   }
}