using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PopulationXmlAttributeMapper : AttributeMapper<SpeciesPopulation, SerializationContext>
   {
      private readonly IPopulationRepository _populationRepository;

      public PopulationXmlAttributeMapper() : this(IoC.Resolve<IPopulationRepository>())
      {
      }

      public PopulationXmlAttributeMapper(IPopulationRepository populationRepository)
      {
         _populationRepository = populationRepository;
      }

      public override string Convert(SpeciesPopulation speciesPopulation, SerializationContext context)
      {
         return speciesPopulation.Name;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return _populationRepository.FindByName(attributeValue);
      }
   }
}