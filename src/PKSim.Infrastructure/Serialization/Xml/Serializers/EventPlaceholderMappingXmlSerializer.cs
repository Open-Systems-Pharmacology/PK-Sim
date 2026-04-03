using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class EventPlaceholderMappingXmlSerializer : BaseXmlSerializer<EventPlaceholderMapping>
   {
      public override void PerformMapping()
      {
         Map(x => x.TemplateEventId);
         Map(x => x.EventKey);
      }
   }
}
