using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ApplicationTypeXmlAttributeMapper : AttributeMapper<ApplicationType, SerializationContext>
   {
      public override string Convert(ApplicationType valueToConvert, SerializationContext context)
      {
         return valueToConvert.Name;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return ApplicationTypes.ByName(attributeValue);
      }
   }
}