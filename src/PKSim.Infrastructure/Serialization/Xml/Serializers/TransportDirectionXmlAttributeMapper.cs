using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class TransportDirectionXmlAttributeMapper : AttributeMapper<TransportDirection, SerializationContext>
   {
      public override string Convert(TransportDirection valueToConvert, SerializationContext context)
      {
         return valueToConvert.Id.ToString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var transportDirectionId = EnumHelper.ParseValue<TransportDirectionId>(attributeValue);
         return TransportDirections.ById(transportDirectionId);
      }
   }
}