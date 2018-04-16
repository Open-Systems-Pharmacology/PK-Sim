using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class OrganXmlSerializer : PKSimContainerXmlSerializer<Organ>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.OrganType);
      }
   }
}