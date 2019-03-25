using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ObserverSetPropertiesXmlSerializer : BaseXmlSerializer<ObserverSetProperties>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.ObserverSetMappings, x => x.AddObserverSetMapping);
      }
   }
}