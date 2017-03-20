using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ParmeterAlternativeReportBuilder : ReportBuilder<ParameterAlternative>
   {
      private readonly IReportGenerator _reportGenerator;

      public ParmeterAlternativeReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(ParameterAlternative parameterAlternative, ReportPart reportPart)
      {
         reportPart.Title = parameterAlternative.Name;

         parameterAlternative.AllVisibleParameters().Each(p => reportPart.AddToContent(_reportGenerator.ReportFor(p)));

         var speciesDependent = parameterAlternative as ISpeciesDependentEntity;
         if (speciesDependent != null)
            reportPart.AddToContent(PKSimConstants.UI.ReportIs(PKSimConstants.UI.Species, speciesDependent.Species.DisplayName));
      }
   }
}