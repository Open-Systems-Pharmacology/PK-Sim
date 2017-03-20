using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
    public class CalculationMethodXmlSerializer : CategoryItemXmlSerializer<CalculationMethod>
    {
        private readonly ICalculationMethodRepository _calculationMethodRepository;

        public CalculationMethodXmlSerializer()
        {
            _calculationMethodRepository = IoC.Resolve<ICalculationMethodRepository>();

        }

        protected override void TypedDeserialize(CalculationMethod objectToDeserialize, XElement outputToDeserialize, SerializationContext serializationContext)
        {
            //nothing to do here
        }

        public override CalculationMethod CreateObject(XElement element, SerializationContext serializationContext)
        {
            var name = element.GetAttribute(CoreConstants.Serialization.Attribute.Name);
            return _calculationMethodRepository.FindBy(name);
        }

    }
}