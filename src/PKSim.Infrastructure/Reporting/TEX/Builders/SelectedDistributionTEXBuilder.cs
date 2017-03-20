using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Infrastructure.Reporting.TeX.Items;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class SelectedDistributionTeXBuilder : OSPSuiteTeXBuilder<SelectedDistribution>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public SelectedDistributionTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(SelectedDistribution selectedDistribution, OSPSuiteTracker buildTracker)
      {
         var parameterContainer = selectedDistribution.ParameterContainer;
         if (!parameterContainer.SelectedDistributions.Any())
            return;

         var report = new List<object>();
         report.Add(buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.Distribution.Pluralize(), selectedDistribution.StructureElementOffset));

         foreach (var distribution in parameterContainer.SelectedDistributions)
         {
            report.Add(buildTracker.GetStructureElementRelativeToLast(distribution.ParameterPath, selectedDistribution.StructureElementOffset + 1));
            report.Add(new Histogram(parameterContainer, distribution));
         }

         _builderRepository.Report(report,buildTracker);
      }
   }
   
}