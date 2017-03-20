using PKSim.Assets;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ObservedDataReportBuilder : ReportBuilder<DataRepository>
   {
      private readonly IReportGenerator _reportGenerator;

      public ObservedDataReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(DataRepository dataRepository, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.ObjectTypes.ObservedData;
         reportPart.AddToContent(dataRepository.Name);
         reportPart.AddPart(_reportGenerator.ReportFor(dataRepository.ExtendedProperties));

         foreach (var column in dataRepository.AllButBaseGrid())
         {
            if (column.DataInfo == null)
               continue;

            reportPart.AddPart(_reportGenerator.ReportFor(column));
         }
      }
   }
}