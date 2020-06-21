using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PopulationXmlAttributeMapper : AttributeMapper<SpeciesPopulation, SerializationContext>
   {
      public override string Convert(SpeciesPopulation speciesPopulation, SerializationContext context)
      {
         return speciesPopulation.Name;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var populationRepository = context.Resolve<IPopulationRepository>();
         return populationRepository.FindByName(attributeValue);
      }
   }
}