using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DistributionTypeXmlAttributeMapper : AttributeMapper<DistributionType,SerializationContext>
   {
      public override string Convert(DistributionType valueToConvert, SerializationContext context)
      {
         return valueToConvert.Id.ToString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return DistributionTypes.ById(EnumHelper.ParseValue<OSPSuite.Core.Domain.Formulas.DistributionType>(attributeValue));
      }
   }
}