using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Summary;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ParameterRangeReportBuilder : ContextSpecification<ParameterRangeReportBuilder>
   {
      protected ParameterRange _parameterRange;

      protected override void Context()
      {
         _parameterRange = new ParameterRange
         {
            ParameterDisplayName = "Height",
            Dimension = DomainHelperForSpecs.LengthDimensionForSpecs()
         };
         _parameterRange.Unit = _parameterRange.Dimension.Unit("cm");
         sut = new ParameterRangeReportBuilder();
      }
   }

   public class When_reporting_a_parameter_range_whose_display_values_have_floating_point_noise_from_the_unit_conversion : concern_for_ParameterRangeReportBuilder
   {
      private ReportPart _result;

      protected override void Context()
      {
         base.Context();
         _parameterRange.MinValueInDisplayUnit = 7;
         _parameterRange.MaxValueInDisplayUnit = 190;
      }

      protected override void Because()
      {
         _result = sut.Report(_parameterRange);
      }

      [Observation]
      public void should_report_the_range_without_the_floating_point_noise()
      {
         _result.Content.ShouldBeEqualTo("Height [cm] in [7;190]\n");
      }
   }
}
