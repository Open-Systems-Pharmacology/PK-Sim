using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ParameterTeXBuilder : OSPSuiteTeXBuilder<IParameter>
   {
      private readonly ITeXBuilderRepository _texBuilderRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ParameterTeXBuilder(ITeXBuilderRepository texBuilderRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _texBuilderRepository = texBuilderRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override void Build(IParameter parameter, OSPSuiteTracker buildTracker)
      {
         var parameterDisplay = PKSimConstants.UI.ReportIs(_representationInfoRepository.DisplayNameFor(parameter), ParameterMessages.DisplayValueFor(parameter));
         _texBuilderRepository.Report(parameterDisplay, buildTracker);
         _texBuilderRepository.Report(new LineBreak(), buildTracker);
      }
   }
}