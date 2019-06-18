using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ObserverSetMappingXmlSerializer : BaseXmlSerializer<ObserverSetMapping>
   {
      public override void PerformMapping()
      {
         Map(x => x.TemplateObserverSetId);
      }
   }
}