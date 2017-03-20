using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DistributionTypeXmlAttributeMapper : AttributeMapper<DistributionType,SerializationContext>
   {
      public override string Convert(DistributionType valueToConvert, SerializationContext context)
      {
         return valueToConvert.Id;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return DistributionTypes.ById(attributeValue);
      }
   }
}