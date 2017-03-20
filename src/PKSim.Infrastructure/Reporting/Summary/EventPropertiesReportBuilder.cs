using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class EventPropertiesReportBuilder : ReportBuilder<EventProperties>
   {
      private readonly IReportGenerator _reportGenerator;

      public EventPropertiesReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(EventProperties eventProperties, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.UI.SimulationEventsConfiguration;
         eventProperties.EventMappings.OrderBy(x=>x.StartTime.Value)
            .Each(x => reportPart.AddToContent(_reportGenerator.ReportFor(x)));
      }
   }
}