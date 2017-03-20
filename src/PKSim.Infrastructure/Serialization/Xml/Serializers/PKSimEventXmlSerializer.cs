using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PKSimEventXmlSerializer : BuildingBlockXmlSerializer<PKSimEvent>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.TemplateName);
      }
   }
}