using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Markdown.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public abstract class BuildingBlockMarkdownBuilder<T> : MarkdownBuilder<T> where T : PKSimBuildingBlock
   {
      protected readonly IMarkdownBuilderRepository _markdownBuilderRepository;

      protected BuildingBlockMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository)
      {
         _markdownBuilderRepository = markdownBuilderRepository;
      }

      public override void Report(T buildingBlock, MarkdownTracker tracker, int indentationLevel)
      {
         tracker.Add($"{_markdownBuilderRepository.TypeFor(buildingBlock)}: {buildingBlock.Name}".ToMarkdownLevelElement(indentationLevel));
      }

      protected virtual void ReportParametersIn(T buildingBlock, MarkdownTracker tracker, int buildingBlockIndentationLevel)
      {
         _markdownBuilderRepository.Report(buildingBlock.AllUserDefinedParameters(), tracker, buildingBlockIndentationLevel + 1);
      }
   }
}