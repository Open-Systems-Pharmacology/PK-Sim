using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class ObserverSetMarkdownBuilder : DefaultBuildingBlockMarkdownBuilder<ObserverSet>
   {
      public ObserverSetMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IReportGenerator reportGenerator) : base(markdownBuilderRepository, reportGenerator)
      {
      }
   }
}