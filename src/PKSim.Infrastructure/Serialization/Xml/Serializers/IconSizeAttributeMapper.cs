using OSPSuite.Serializer.Attributes;
using OSPSuite.Assets;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IconSizeAttributeMapper : AttributeMapper<IconSize, SerializationContext>
   {
      public override string Convert(IconSize valueToConvert, SerializationContext context)
      {
         return valueToConvert.Id;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return IconSizes.ById(attributeValue);
      }
   }
}