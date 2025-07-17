using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public abstract class DefaultBuildingBlockMarkdownBuilder<T> : BuildingBlockMarkdownBuilder<T> where T : PKSimBuildingBlock
   {
      private readonly IReportGenerator _reportGenerator;

      protected DefaultBuildingBlockMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IReportGenerator reportGenerator) : base(markdownBuilderRepository)
      {
         _reportGenerator = reportGenerator;
      }

      public override void Report(T buildingBlock, MarkdownTracker tracker, int indentationLevel)
      {
         base.Report(buildingBlock, tracker, indentationLevel);
         tracker.Add(_reportGenerator.StringReportFor(buildingBlock));
         ReportParametersIn(buildingBlock, tracker, indentationLevel);
      }
   }
}