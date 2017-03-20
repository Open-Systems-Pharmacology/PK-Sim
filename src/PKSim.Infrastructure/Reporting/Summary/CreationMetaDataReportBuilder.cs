using PKSim.Core.Reporting;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class CreationMetaDataReportBuilder : ReportBuilder<CreationMetaData>
   {
      protected override void FillUpReport(CreationMetaData creationMetaData, ReportPart reportPart)
      {
         reportPart.AddToContent(creationMetaData.ToDisplayString());
      }
   }
}