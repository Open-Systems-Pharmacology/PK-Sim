using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Core;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportHistoryToExcelCommand : IUICommand
   {
      private readonly IWorkspace _workspace;
      private readonly IReportTask _reportTask;
      private readonly IDialogCreator _dialogCreator;

      public ExportHistoryToExcelCommand(IWorkspace workspace, IReportTask reportTask, IDialogCreator dialogCreator)
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