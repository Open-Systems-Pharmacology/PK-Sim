using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class SimulationMarkdownBuilder: DefaultBuildingBlockMarkdownBuilder<Simulation>
   {
      public SimulationMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IReportGenerator reportGenerator) : base(markdownBuilderRepository, reportGenerator)
      {
      }
   }
}