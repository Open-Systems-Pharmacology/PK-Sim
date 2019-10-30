using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ObserverSetReportBuilder : ReportBuilder<ObserverSet>
   {
      protected override void FillUpReport(ObserverSet observerSet, ReportPart reportPart)
      {
         reportPart.Title = observerSet.Name;
         observerSet.Observers.Each(x =>
         {
            reportPart.AddToContent(x.Name);
         });
      }
   }
}