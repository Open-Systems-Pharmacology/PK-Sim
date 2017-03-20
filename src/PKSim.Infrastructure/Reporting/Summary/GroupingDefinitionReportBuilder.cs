using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class NumberOfBinsGroupingDefinitionReportBuilder : ReportBuilder<NumberOfBinsGroupingDefinition>
   {
      protected override void FillUpReport(NumberOfBinsGroupingDefinition groupingDefinition, ReportPart reportPart)
      {
         reportPart.AddToContent(PKSimConstants.UI.ReportIs(PKSimConstants.UI.NumberOfBins, groupingDefinition.NumberOfBins));
      }
   }

   public class FixedLimitsGroupingDefinitionReportBuilder : ReportBuilder<FixedLimitsGroupingDefinition>
   {
      private readonly DoubleFormatter _doubleFormatter;

      public FixedLimitsGroupingDefinitionReportBuilder()
      {
         _doubleFormatter = new DoubleFormatter();
      }

      protected override void FillUpReport(FixedLimitsGroupingDefinition fixedLimits, ReportPart reportPart)
      {
         //-1 because we have one label more that limits
         int i = 0;
         reportPart.AddToContent("].., {0}] is {1}", format(fixedLimits.Limits[i], fixedLimits), fixedLimits.Labels[i]);

         for (i = 1; i < fixedLimits.Labels.Count - 1; i++)
         {
            reportPart.AddToContent("]{0}, {1}] is {2}", format(fixedLimits.Limits[i - 1], fixedLimits), format(fixedLimits.Limits[i], fixedLimits), fixedLimits.Labels[i]);
         }

         reportPart.AddToContent("]{0}, ..[ is {1}", format(fixedLimits.Limits[i - 1], fixedLimits), fixedLimits.Labels[i]);
      }

      private string format(double baseValue, FixedLimitsGroupingDefinition groupingDefinition)
      {
         return _doubleFormatter.Format(groupingDefinition.ConvertToDisplayUnit(baseValue), groupingDefinition.DisplayUnit.Name);
      }
   }
}