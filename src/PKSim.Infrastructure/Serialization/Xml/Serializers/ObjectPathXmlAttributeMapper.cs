using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ObjectPathXmlAttributeMapper : AttributeMapper<ObjectPath, SerializationContext>
   {
      public override string Convert(ObjectPath objectPath, SerializationContext context)
      {
         return objectPath == null ? string.Empty : objectPath.ToString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         if (string.IsNullOrEmpty(attributeValue))
            return new ObjectPath();

         return attributeValue.ToObjectPath();
      }
   }
}