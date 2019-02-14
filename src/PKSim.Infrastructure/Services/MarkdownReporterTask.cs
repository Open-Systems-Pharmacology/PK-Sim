using System.IO;
using System.Threading.Tasks;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class MarkdownReporterTask : IMarkdownReporterTask
   {
      private readonly IReportGenerator _reportGenerator;

      public MarkdownReporterTask(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      public Task ExportToMarkdown(object objectToExport, string file)
      {
         return Task.Run(() => File.WriteAllText(file, ExportToMarkdownString(objectToExport)));
      }

      public string ExportToMarkdownString(object objectToExport)
      {
         return _reportGenerator.StringReportFor(objectToExport);
      }
   }
}