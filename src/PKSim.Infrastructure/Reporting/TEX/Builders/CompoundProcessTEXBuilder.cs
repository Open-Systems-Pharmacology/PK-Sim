using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Presentation.Extensions;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public abstract class CompoundProcessTEXBuilder<T> : OSPSuiteTeXBuilder<T> where T : CompoundProcess
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      protected CompoundProcessTEXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _builderRepository = builderRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override void Build(T process, OSPSuiteTracker buildTracker)
      {
         string processDisplay = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.PROCESS, process.InternalName);
         _builderRepository.Report(process.Name.AsFullLine(), buildTracker);
         _builderRepository.Report(this.ReportValue(PKSimConstants.UI.ProcessType, processDisplay), buildTracker);
         _builderRepository.Report(this.ReportDescription(process, buildTracker), buildTracker);
         _builderRepository.Report(tableParameterFor(process), buildTracker);
      }

      private object tableParameterFor(T partialProcess)
      {
         return new ParameterList(  PKSimConstants.UI.Parameters, partialProcess.AllVisibleParameters());
      }
   }

   public class CompoundProcessTeXBuilder : CompoundProcessTEXBuilder<CompoundProcess>
   {
      public CompoundProcessTeXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository) : base(builderRepository, representationInfoRepository)
      {
      }
   }
}