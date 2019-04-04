using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class IndividualMarkdownBuilder : DefaultBuildingBlockMarkdownBuilder<Individual>
   {
      public IndividualMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IReportGenerator reportGenerator) : base(markdownBuilderRepository, reportGenerator)
      {
      }
   }
}