using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public class EventMarkdownBuilder : BuildingBlockMarkdownBuilder<PKSimEvent>
   {
      private readonly IEventGroupRepository _eventGroupRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public EventMarkdownBuilder(
         IMarkdownBuilderRepository markdownBuilderRepository,
         IEventGroupRepository eventGroupRepository,
         IRepresentationInfoRepository representationInfoRepository
         ) : base(markdownBuilderRepository)
      {
         _eventGroupRepository = eventGroupRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override void Report(PKSimEvent pkSimEvent, MarkdownTracker tracker, int indentationLevel)
      {
         base.Report(pkSimEvent, tracker, indentationLevel);
         var eventTemplate = _eventGroupRepository.FindByName(pkSimEvent.TemplateName);
         tracker.AddValue(PKSimConstants.UI.Type, _representationInfoRepository.DisplayNameFor(eventTemplate));
         ReportParametersIn(pkSimEvent, tracker, indentationLevel);
      }
   }
}