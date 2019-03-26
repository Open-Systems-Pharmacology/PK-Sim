using System.IO;
using System.Threading.Tasks;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.Markdown;

namespace PKSim.Infrastructure.Services
{
   public class MarkdownReporterTask : IMarkdownReporterTask
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IMarkdownBuilderRepository _markdownBuilderRepository;

      public MarkdownReporterTask(IReportGenerator reportGenerator, IMarkdownBuilderRepository markdownBuilderRepository)
      {
         _reportGenerator = reportGenerator;
         _markdownBuilderRepository = markdownBuilderRepository;
      }

      public Task ExportToMarkdown(object objectToExport, string file)
      {
         return Task.Run(() => File.WriteAllText(file, ExportToMarkdownString(objectToExport)));
      }

      public string ExportToMarkdownString(object objectToExport)
      {
         var builder = _markdownBuilderRepository.BuilderFor(objectToExport);
         if (builder == null)
            return _reportGenerator.StringReportFor(objectToExport);

         var tracker = new MarkdownTracker();
         builder.Report(objectToExport, tracker);
         return tracker.ToString();
      }
   }
}