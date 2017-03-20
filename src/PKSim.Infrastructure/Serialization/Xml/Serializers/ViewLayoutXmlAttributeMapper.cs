using OSPSuite.Serializer.Attributes;
using PKSim.Presentation;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ViewLayoutXmlAttributeMapper : AttributeMapper<ViewLayout, SerializationContext>
   {
      public override string Convert(ViewLayout viewLayout, SerializationContext context)
      {
         return viewLayout.Id;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return ViewLayouts.ById(attributeValue);
      }
   }
}