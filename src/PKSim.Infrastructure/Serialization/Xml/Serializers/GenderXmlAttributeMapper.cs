using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class GenderXmlAttributeMapper : AttributeMapper<Gender, SerializationContext>
   {
      private readonly IGenderRepository _genderRepository;

      public GenderXmlAttributeMapper() : this(IoC.Resolve<IGenderRepository>())
      {
      }

      public GenderXmlAttributeMapper(IGenderRepository genderRepository)
      {
         _genderRepository = genderRepository;
      }

      public override string Convert(Gender gender, SerializationContext context)
      {
         return gender.Name;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return _genderRepository.FindByName(attributeValue);
      }
   }
}