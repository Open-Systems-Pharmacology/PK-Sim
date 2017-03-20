using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class InteractionPropertiesXmlSerializer : BaseXmlSerializer<InteractionProperties>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.Interactions, x => x.AddInteraction);
      }
   }
}