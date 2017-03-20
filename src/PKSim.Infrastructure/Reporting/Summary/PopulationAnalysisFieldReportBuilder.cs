using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Reporting;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class PopulationAnalysisFieldReportBuilder : ReportBuilder<IPopulationAnalysisField>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public PopulationAnalysisFieldReportBuilder(IReportGenerator reportGenerator,IObjectTypeResolver objectTypeResolver)
      {
         _reportGenerator = reportGenerator;
         _objectTypeResolver = objectTypeResolver;
      }

      protected override void FillUpReport(IPopulationAnalysisField populationAnalysisField, ReportPart reportPart)
      {
         reportPart.Title = _objectTypeResolver.TypeFor(populationAnalysisField);
         reportPart.AddToContent(populationAnalysisField.Name);

         var groupingField = populationAnalysisField as PopulationAnalysisGroupingField;
         if (groupingField == null) return;

         reportPart.AddPart(_reportGenerator.ReportFor(groupingField.GroupingDefinition));
      }
   }
}