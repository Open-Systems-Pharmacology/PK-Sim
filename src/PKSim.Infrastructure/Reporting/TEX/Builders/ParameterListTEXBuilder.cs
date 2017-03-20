using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ParameterListTeXBuilder : OSPSuiteTeXBuilder<ParameterList>
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ParameterListTeXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _builderRepository = builderRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override void Build(ParameterList parameterList, OSPSuiteTracker buildTracker)
      {
         _builderRepository.Report(parameterList.ToTable(_representationInfoRepository), buildTracker);
      }
   }
}