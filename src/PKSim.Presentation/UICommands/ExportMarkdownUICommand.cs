using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportMarkdownUICommand<T> : ObjectUICommand<T> where T : class, IObjectBase
   {
      private readonly IMarkdownReporterTask _markdownReporterTask;
      private readonly IDialogCreator _dialogCreator;

      public ExportMarkdownUICommand(IMarkdownReporterTask markdownReporterTask, IDialogCreator dialogCreator)
      {
         _markdownReporterTask = markdownReporterTask;
         _dialogCreator = dialogCreator;
      }

      protected override async void PerformExecute()
      {
         var file = _dialogCreator.AskForFileToSave("Select markdown file", Constants.Filter.FileFilter("Markdown File", CoreConstants.Filter.MARKDOWN_EXTENSION), Constants.DirectoryKey.REPORT, Subject.Name);
         await _markdownReporterTask.SecureAwait(x => _markdownReporterTask.ExportToMarkdown(Subject, file));
      }
   }
}