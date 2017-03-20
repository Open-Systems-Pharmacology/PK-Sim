using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class EventMappingXmlSerializer : BaseXmlSerializer<EventMapping>
   {
      public override void PerformMapping()
      {
         Map(x => x.StartTime);
         Map(x => x.TemplateEventId);
      }
   }
}