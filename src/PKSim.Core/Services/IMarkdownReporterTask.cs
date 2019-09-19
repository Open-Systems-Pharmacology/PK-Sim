using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface IMarkdownReporterTask
   {
      Task ExportToMarkdown(object objectToExport, string file, int? indentationLevel = null);
      string ExportToMarkdownString(object objectToExport, int? indentationLevel = null);
   }
}