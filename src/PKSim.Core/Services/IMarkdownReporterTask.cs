using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface IMarkdownReporterTask
   {
      Task ExportToMarkdown(object objectToExport, string file);
      string ExportToMarkdownString(object objectToExport);
   }
}