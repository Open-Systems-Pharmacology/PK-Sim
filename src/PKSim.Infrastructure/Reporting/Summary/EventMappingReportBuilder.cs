using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class EventMappingReportBuilder : ReportBuilder<EventMapping>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public EventMappingReportBuilder(IReportGenerator reportGenerator, IBuildingBlockRepository buildingBlockRepository)
      {
         _reportGenerator = reportGenerator;
         _buildingBlockRepository = buildingBlockRepository;
      }

      protected override void FillUpReport(EventMapping eventMapping, ReportPart reportPart)
      {
         var templateEvent = _buildingBlockRepository.FindById(eventMapping.TemplateEventId);
         if (templateEvent == null) return;

         reportPart.AddToContent($"{PKSimConstants.UI.ReportIs("Event", templateEvent.Name)}, {_reportGenerator.StringReportFor(eventMapping.StartTime)}");
      }
   }
}