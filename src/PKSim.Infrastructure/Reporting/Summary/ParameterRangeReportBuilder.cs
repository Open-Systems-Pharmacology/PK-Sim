using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ParameterRangeReportBuilder : ReportBuilder<ParameterRange>
   {
      protected override void FillUpReport(ParameterRange parameterRange, ReportPart reportPart)
      {
         if (parameterRange.IsConstant)
            reportPart.AddToContent("{0} = {1} [{2}]", parameterRange.ParameterDisplayName, parameterRange.MinValueInDisplayUnit, parameterRange.Unit);
         else if (parameterRange.MinValue.HasValue && parameterRange.MaxValue.HasValue)
            reportPart.AddToContent("{0} [{3}] in [{1};{2}]", parameterRange.ParameterDisplayName, parameterRange.MinValueInDisplayUnit, parameterRange.MaxValueInDisplayUnit, parameterRange.Unit);
         else if (parameterRange.MinValue.HasValue)
            reportPart.AddToContent("{0} [{2}] in [{1};∞[", parameterRange.ParameterDisplayName, parameterRange.MinValueInDisplayUnit, parameterRange.Unit);
         else if (parameterRange.MaxValue.HasValue)
            reportPart.AddToContent("{0} [{2}] in ]0;{1}]", parameterRange.ParameterDisplayName, parameterRange.MaxValueInDisplayUnit, parameterRange.Unit);
         else
            reportPart.AddToContent("{0} [{1}] in ]0;∞[", parameterRange.ParameterDisplayName, parameterRange.Unit);
      }
   }
}
