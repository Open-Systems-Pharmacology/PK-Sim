using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ParameterValueVersionXmlSerializer : CategoryItemXmlSerializer<ParameterValueVersion>
   {
      protected override void TypedDeserialize(ParameterValueVersion objectToDeserialize, XElement outputToDeserialize, SerializationContext serializationContext)
      {
         //nothing to do here as object are loaded from repository;
      }

      public override ParameterValueVersion CreateObject(XElement element, SerializationContext serializationContext)
      {
         var name = element.GetAttribute(CoreConstants.Serialization.Attribute.Name);
         var parameterValueVersionRepository = serializationContext.Resolve<IParameterValueVersionRepository>();
         return parameterValueVersionRepository.FindBy(name);
      }
   }
}