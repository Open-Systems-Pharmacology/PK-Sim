using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SystemicProcessTypeXmlAttributeMapper : AttributeMapper<SystemicProcessType, SerializationContext>
   {
      public override string Convert(SystemicProcessType valueToConvert, SerializationContext context)
      {
         return valueToConvert.SystemicProcessTypeId.ToString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return SystemicProcessTypes.ById(attributeValue);
      }
   }
}