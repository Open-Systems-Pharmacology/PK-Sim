using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class EventPropertiesXmlSerializer : BaseXmlSerializer<EventProperties>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.EventMappings, x => x.AddEventMapping);
      }
   }
}