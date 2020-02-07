using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Xml.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class CalculationMethodXmlSerializer : CategoryItemXmlSerializer<CalculationMethod>
   {
      protected override void TypedDeserialize(CalculationMethod objectToDeserialize, XElement outputToDeserialize, SerializationContext serializationContext)
      {
         //nothing to do here
      }

      public override CalculationMethod CreateObject(XElement element, SerializationContext serializationContext)
      {
         var name = element.GetAttribute(CoreConstants.Serialization.Attribute.Name);
         var calculationMethodRepository = serializationContext.Resolve<ICalculationMethodRepository>();
         return calculationMethodRepository.FindBy(name);
      }
   }
}