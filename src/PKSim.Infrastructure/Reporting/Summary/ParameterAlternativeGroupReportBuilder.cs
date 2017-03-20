using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ParameterAlternativeGroupReportBuilder : ReportBuilder<PKSim.Core.Model.ParameterAlternativeGroup>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IReportGenerator _reportGenerator;

      public ParameterAlternativeGroupReportBuilder(IRepresentationInfoRepository representationInfoRepository,IReportGenerator reportGenerator)
      {
         _representationInfoRepository = representationInfoRepository;
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(PKSim.Core.Model.ParameterAlternativeGroup parameterAlternativeGroup, ReportPart reportPart)
      {
         reportPart.Title = _representationInfoRepository.DisplayNameFor(parameterAlternativeGroup);
         reportPart.AddToContent(_reportGenerator.ReportFor(parameterAlternativeGroup.DefaultAlternative));
      }
   }
}