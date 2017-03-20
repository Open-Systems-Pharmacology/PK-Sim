using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class EventReportBuilder:ReportBuilder< PKSim.Core.Model.PKSimEvent>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IReportGenerator _reportGenerator;

      public EventReportBuilder(IRepresentationInfoRepository representationInfoRepository,IReportGenerator reportGenerator)
      {
         _representationInfoRepository = representationInfoRepository;
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport( PKSim.Core.Model.PKSimEvent pkSimEvent, ReportPart reportPart)
      {
         reportPart.Title = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.EVENT, pkSimEvent.TemplateName);
         pkSimEvent.AllVisibleParameters().Each(p => reportPart.AddToContent(_reportGenerator.ReportFor(p)));
      }
   }
}