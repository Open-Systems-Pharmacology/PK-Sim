using PKSim.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ParameterDistributionSettingsXmlSerializer : BaseXmlSerializer<ParameterDistributionSettings>
   {
      public override void PerformMapping()
      {
         Map(x => x.ParameterPath);
         Map(x => x.Settings);
      }
   }

   public class ParameterDistributionSettingsCacheXmlSerializer : BaseXmlSerializer<ParameterDistributionSettingsCache>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.AllParameterDistributions, x => x.Add);
      }
   }

   public class DistributionSettingsXmlSerializer : BaseXmlSerializer<DistributionSettings>
   {
      public override void PerformMapping()
      {
         Map(x => x.BarType);
         Map(x => x.AxisCountMode);
         Map(x => x.SelectedGender);
         Map(x => x.XAxisTitle);
         Map(x => x.YAxisTitle);
         Map(x => x.PlotCaption);
      }
   }
}