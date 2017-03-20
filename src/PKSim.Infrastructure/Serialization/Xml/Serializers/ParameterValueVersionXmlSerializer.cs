using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ParameterValueVersionXmlSerializer : CategoryItemXmlSerializer<ParameterValueVersion>
   {
      private readonly IParameterValueVersionRepository _parameterValueVersionRepository;

      public ParameterValueVersionXmlSerializer()
      {
         _parameterValueVersionRepository = IoC.Resolve<IParameterValueVersionRepository>();
      }

      protected override void TypedDeserialize(ParameterValueVersion objectToDeserialize, XElement outputToDeserialize, SerializationContext serializationContext)
      {
         //nothing to do here as object are loaded from repository;
      }

      public override ParameterValueVersion CreateObject(XElement element, SerializationContext serializationContext)
      {
         var name = element.GetAttribute(CoreConstants.Serialization.Attribute.Name);
         return _parameterValueVersionRepository.FindBy(name);
      }
   }
}