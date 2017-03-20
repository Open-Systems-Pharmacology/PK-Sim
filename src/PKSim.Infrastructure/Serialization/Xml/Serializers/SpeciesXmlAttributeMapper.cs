using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SpeciesXmlAttributeMapper : AttributeMapper<Species, SerializationContext>
    {
        private readonly ISpeciesRepository _speciesRepository;

        public SpeciesXmlAttributeMapper() : this(IoC.Resolve<ISpeciesRepository>())
        {
        }

        public SpeciesXmlAttributeMapper(ISpeciesRepository speciesRepository)
        {
            _speciesRepository = speciesRepository;
        }

        public override string Convert(Species species, SerializationContext context)
        {
           if(species==null) return string.Empty;
            return species.Name;
        }

        public override object ConvertFrom(string attributeValue, SerializationContext context)
        {
            return _speciesRepository.FindByName(attributeValue);
        }
    }
}