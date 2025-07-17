using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Services;
using static OSPSuite.Core.Domain.Constants.Filter;

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
         var file = _dialogCreator.AskForFileToSave("Select markdown file", FileFilter("Markdown File", MARKDOWN_EXTENSION), Constants.DirectoryKey.REPORT, Subject.Name);
         if (string.IsNullOrEmpty(file))
            return;

         await _markdownReporterTask.SecureAwait(x => _markdownReporterTask.ExportToMarkdown(Subject, file));
      }
   }
}