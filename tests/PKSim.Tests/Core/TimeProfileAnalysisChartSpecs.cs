using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_TimeProfileAnalysisChart : ContextSpecification<TimeProfileAnalysisChart>
   {
      protected override void Context()
      {
         sut = new TimeProfileAnalysisChart();
      }
   }

   public class When_updating_properties_from_another_time_profile_analysis_chart : concern_for_TimeProfileAnalysisChart
   {
      private ICloneManager _cloneManager;
      private TimeProfileAnalysisChart _sourceChart;

      protected override void Context()
      {
         base.Context();
         _cloneManager = A.Fake<ICloneManager>();
         _sourceChart = new TimeProfileAnalysisChart();
         _sourceChart.AddSecondaryAxis(new AxisSettings {Min = 0, Max = 1, AutoRange = false});
         _sourceChart.AddSecondaryAxis(new AxisSettings {Min = 10, Max = 11, AutoRange = true});
         _sourceChart.PrimaryXAxisSettings.Min = 0;
         _sourceChart.PrimaryXAxisSettings.Max = 10;
         _sourceChart.PrimaryYAxisSettings.Min = 10;
         _sourceChart.PrimaryYAxisSettings.Max = 100;
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_sourceChart, _cloneManager);
      }

      [Observation]
      public void the_destination_has_the_same_primary_x_primary_y_and_secondary_y_settings()
      {
         compareAxes(sut.PrimaryXAxisSettings, _sourceChart.PrimaryXAxisSettings);
         compareAxes(sut.PrimaryYAxisSettings, _sourceChart.PrimaryYAxisSettings);
         compareAxes(sut.SecondaryYAxisSettings[0], _sourceChart.SecondaryYAxisSettings[0]);
         compareAxes(sut.SecondaryYAxisSettings[1], _sourceChart.SecondaryYAxisSettings[1]);
      }

      private void compareAxes(AxisSettings destinationSettings, AxisSettings sourceSettings)
      {
         destinationSettings.Min.ShouldBeEqualTo(sourceSettings.Min);
         destinationSettings.Max.ShouldBeEqualTo(sourceSettings.Max);
         destinationSettings.AutoRange.ShouldBeEqualTo(sourceSettings.AutoRange);
      }

   }

   public class When_retreiving_secondary_axes_settings_from_the_chart : concern_for_TimeProfileAnalysisChart
   {
      protected override void Because()
      {
         sut.AxisSettingsForSecondaryYAxis(3);
      }

      [Observation]
      public void the_axis_settings_should_be_returned_even_when_no_axes_settings_were_added_initially()
      {
         sut.SecondaryYAxisSettings.Count.ShouldBeEqualTo(4);
      }
   }

}
