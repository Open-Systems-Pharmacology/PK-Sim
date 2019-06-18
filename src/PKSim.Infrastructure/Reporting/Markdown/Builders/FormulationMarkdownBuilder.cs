using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class FormulationMarkdownBuilder : BuildingBlockMarkdownBuilder<Formulation>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public FormulationMarkdownBuilder(IMarkdownBuilderRepository markdownBuilderRepository, IRepresentationInfoRepository representationInfoRepository) : base(markdownBuilderRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public override void Report(Formulation formulation, MarkdownTracker tracker, int indentationLevel)
      {
         base.Report(formulation, tracker, indentationLevel);
         var formulationInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, formulation.FormulationType);
         tracker.AddValue(PKSimConstants.UI.Type, formulationInfo.DisplayName);
         ReportParametersIn(formulation, tracker, indentationLevel);
      }
   }
}