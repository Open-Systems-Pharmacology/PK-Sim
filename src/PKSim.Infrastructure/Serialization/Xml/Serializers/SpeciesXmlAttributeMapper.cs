using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SpeciesXmlAttributeMapper : AttributeMapper<Species, SerializationContext>
   {
      public override string Convert(Species species, SerializationContext context)
      {
         if (species == null) return string.Empty;
         return species.Name;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var speciesRepository = context.Resolve<ISpeciesRepository>();
         return speciesRepository.FindByName(attributeValue);
      }
   }
}