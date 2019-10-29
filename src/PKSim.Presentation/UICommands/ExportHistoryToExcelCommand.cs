using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;

namespace PKSim.Presentation.UICommands
{
   public class ExportHistoryToExcelCommand : IUICommand
   {
      private readonly ICoreWorkspace _workspace;
      private readonly IHistoryExportTask _reportTask;
      private readonly IDialogCreator _dialogCreator;

      public ExportHistoryToExcelCommand(ICoreWorkspace workspace, IHistoryExportTask reportTask, IDialogCreator dialogCreator)
      {
         _workspace = workspace;
         _reportTask = reportTask;
         _dialogCreator = dialogCreator;
      }

      public void Execute()
      {
         var reportFileName = _dialogCreator.AskForFileToSave(PKSimConstants.UI.CreateReportTitle, Constants.Filter.EXCEL_SAVE_FILE_FILTER, Constants.DirectoryKey.REPORT, _workspace.Project.Name);
         if (reportFileName.IsNullOrEmpty()) return;
         var reportOptions = new ReportOptions {ReportFullPath = reportFileName, SheetName = _workspace.Project.Name, OpenReport = true};
         _reportTask.CreateReport(_workspace.HistoryManager, reportOptions);
      }
   }
}