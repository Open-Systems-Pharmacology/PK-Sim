using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SchemaReportBuilder : ReportBuilder<Schema>
   {
      private readonly IReportGenerator _reportGenerator;

      public SchemaReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(Schema schema, ReportPart reportPart)
      {
         reportPart.Title = schema.Name;
         reportPart.AddToContent(_reportGenerator.ReportFor(schema.StartTime));
         reportPart.AddToContent(_reportGenerator.ReportFor(schema.TimeBetweenRepetitions));
         reportPart.AddToContent(_reportGenerator.ReportFor(schema.NumberOfRepetitions));
         schema.SchemaItems.Each(x => reportPart.AddPart(_reportGenerator.ReportFor(x)));
      }
   }
}