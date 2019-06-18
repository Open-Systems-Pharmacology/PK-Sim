using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class PopulationMarkdownBuilder: DefaultBuildingBlockMarkdownBuilder<Population>
   {
      public PopulationMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IReportGenerator reportGenerator) : base(markdownBuilderRepository, reportGenerator)
      {
      }
   }
}