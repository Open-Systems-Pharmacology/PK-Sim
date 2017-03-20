using System.Drawing;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;

namespace PKSim.IntegrationTests
{
   public class When_serializing_some_chart_settings : ContextForSerialization<ChartSettings>
   {
      private ChartSettings _chartSettings1;
      private ChartSettings _deserializedSettings;

      protected override void Context()
      {
         base.Context();
         _chartSettings1 = new ChartSettings { BackColor = Color.Orange, DiagramBackColor = Color.Firebrick, LegendPosition = LegendPositions.Bottom, SideMarginsEnabled = false};
      }

      protected override void Because()
      {
         _deserializedSettings = SerializeAndDeserialize(_chartSettings1);
      }

      [Observation]
      public void should_have_deserialized_all_chart_settings()
      {
         verify(_deserializedSettings, _chartSettings1);
      }

      private void verify(ChartSettings deserializedSetting, ChartSettings chartSettings)
      {
         deserializedSetting.BackColor.ShouldBeEqualTo(chartSettings.BackColor);
         deserializedSetting.DiagramBackColor.ShouldBeEqualTo(chartSettings.DiagramBackColor);
         deserializedSetting.LegendPosition.ShouldBeEqualTo(LegendPositions.Bottom);
         deserializedSetting.SideMarginsEnabled.ShouldBeEqualTo(false);
      }
   }
}