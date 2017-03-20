using OSPSuite.Core.Domain.Data;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class DataColumnReportBuilder : ReportBuilder<DataColumn>
   {
      private readonly IReportGenerator _reportGenerator;

      public DataColumnReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(DataColumn column, ReportPart reportPart)
      {
         if(column.DataInfo==null) return;
         reportPart.AddPart(_reportGenerator.ReportFor(column.DataInfo.ExtendedProperties).WithTitle(column.Name));
      }
   }
}