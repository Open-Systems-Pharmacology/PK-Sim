using OSPSuite.Serializer.Xml;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class BaseXmlSerializer<T> : XmlSerializer<T, SerializationContext>, IPKSimXmlSerializer
   {
      protected BaseXmlSerializer()
      {
      }

      protected BaseXmlSerializer(string name) : base(name)
      {
      }
   }
}