using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class AxisSettingsXmlSerializer : BaseXmlSerializer<AxisSettings>
   {
      public override void PerformMapping()
      {
         Map(x => x.Max);
         Map(x => x.Min);
         Map(x => x.AutoRange);
      }
   }
}