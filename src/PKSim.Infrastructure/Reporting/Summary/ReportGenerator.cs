using System;
using OSPSuite.Core.Domain;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ReportGenerator : IReportGenerator
   {
      private readonly IReportBuilderRepository _reportBuilderRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ReportGenerator(IReportBuilderRepository reportBuilderRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _reportBuilderRepository = reportBuilderRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public ReportPart ReportFor<TObject>(TObject objectToReport)
      {
         try
         {
            var builder = _reportBuilderRepository.BuilderFor(objectToReport);
            return builder != null ? builder.Report(objectToReport) : defaultReportFor(objectToReport);
         }
         catch (Exception)
         {
            //App should not crash if an exception occurs while resolving the report for a specific object
            return defaultReportFor(objectToReport);
         }
      }

      private ReportPart defaultReportFor<TObject>(TObject objectToReport)
      {
         var report = new ReportPart();
         var objectBase = objectToReport as IObjectBase;
         if (objectBase != null)
            report.AddToContent(_representationInfoRepository.DescriptionFor(objectBase));

         return report;
      }

      public string StringReportFor<TObject>(TObject objectToReport)
      {
         return ReportFor(objectToReport).ToStringReport();
      }
   }
}