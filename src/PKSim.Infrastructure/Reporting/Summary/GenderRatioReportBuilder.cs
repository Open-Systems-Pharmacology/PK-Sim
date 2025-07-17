using OSPSuite.Utility.Format;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class GenderRatioReportBuilder : ReportBuilder<GenderRatio>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly NumericFormatter<double> _formatter;

      public GenderRatioReportBuilder(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
         _formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
      }

      protected override void FillUpReport(GenderRatio genderRatio, ReportPart reportPart)
      {
         reportPart.AddToContent("{0} at {1}%", _representationInfoRepository.DisplayNameFor(genderRatio.Gender), _formatter.Format(genderRatio.Ratio));
      }
   }
}