using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class ProtocolMarkdownBuilder : DefaultBuildingBlockMarkdownBuilder<Protocol>
   {
      public ProtocolMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IReportGenerator reportGenerator) : base(markdownBuilderRepository, reportGenerator)
      {
      }
   }
}