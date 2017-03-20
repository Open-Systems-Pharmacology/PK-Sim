using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class EventMappingReportBuilder : ReportBuilder<IEventMapping>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public EventMappingReportBuilder(IReportGenerator reportGenerator, IBuildingBlockRepository buildingBlockRepository)
      {
         _reportGenerator = reportGenerator;
         _buildingBlockRepository = buildingBlockRepository;
      }

      protected override void FillUpReport(IEventMapping eventMapping, ReportPart reportPart)
      {
         var templateEvent = _buildingBlockRepository.FindById(eventMapping.TemplateEventId);
         if (templateEvent == null) return;

         reportPart.AddToContent(string.Format("{0}, {1}", PKSimConstants.UI.ReportIs("Event", templateEvent.Name), _reportGenerator.StringReportFor(eventMapping.StartTime)));
      }
   }
}