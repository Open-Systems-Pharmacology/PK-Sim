using System.IO;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportParameterIdentificationToRUICommand : ObjectUICommand<ParameterIdentification>
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IParameterIdentificationExportTask _parameterIdentificationExportTask;

      public ExportParameterIdentificationToRUICommand(
         IDialogCreator dialogCreator,
         IParameterIdentificationExportTask parameterIdentificationExportTask)
      {
         _dialogCreator = dialogCreator;
         _parameterIdentificationExportTask = parameterIdentificationExportTask;
      }

      protected override async void PerformExecute()
      {
         var folder = getExportPath();
         if (string.IsNullOrEmpty(folder))
            return;

         await _parameterIdentificationExportTask.SecureAwait(x => x.ExportParameterIdentification(Subject, folder));
      }

      private string getExportPath()
      {
         var folder = _dialogCreator.AskForFolder(Captions.ParameterIdentification.SelectDirectoryForParameterIdentificationExport, Constants.DirectoryKey.REPORT);
         if (string.IsNullOrEmpty(folder))
            return string.Empty;

         var newDirectoryName = Subject.Name;
         folder = Path.Combine(folder, newDirectoryName);

         if (!DirectoryHelper.DirectoryExists(folder))
            return DirectoryHelper.CreateDirectory(folder);

         if (_dialogCreator.MessageBoxYesNo(Captions.DoYouWantToDeleteDirectory(newDirectoryName), Captions.Delete, Captions.Cancel, defaultButton: ViewResult.No) == ViewResult.No)
            return string.Empty;

         DirectoryHelper.DeleteDirectory(folder, true);
         return DirectoryHelper.CreateDirectory(folder);
      }
   }
}