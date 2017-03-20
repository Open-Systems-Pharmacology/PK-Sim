using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ProtocolPropertiesXmlSerializer : BaseXmlSerializer<ProtocolProperties>
   {
      public override void PerformMapping()
      {
         MapReference(x => x.Protocol);
         MapEnumerable(x => x.FormulationMappings,x => x.AddFormulationMapping);
      }
   }
}