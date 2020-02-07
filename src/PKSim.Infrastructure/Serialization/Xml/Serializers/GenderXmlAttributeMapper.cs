using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class GenderXmlAttributeMapper : AttributeMapper<Gender, SerializationContext>
   {
      public override string Convert(Gender gender, SerializationContext context)
      {
         return gender.Name;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var genderRepository = context.Resolve<IGenderRepository>();
         return genderRepository.FindByName(attributeValue);
      }
   }
}