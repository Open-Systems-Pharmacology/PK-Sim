using System.Xml.Linq;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Xml.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class AdvancedProtocolXmlSerializer : BuildingBlockXmlSerializer<AdvancedProtocol>
   {
      protected override XElement TypedSerialize(AdvancedProtocol protocol, SerializationContext serializationContext)
      {
         var protocolNode = base.TypedSerialize(protocol, serializationContext);
         protocolNode.AddAttribute(CoreConstants.Serialization.Attribute.TimeUnit, protocol.TimeUnit.Name);
         return protocolNode;
      }

      protected override void TypedDeserialize(AdvancedProtocol protocol, XElement protocolElement, SerializationContext serializationContext)
      {
         base.TypedDeserialize(protocol, protocolElement, serializationContext);
         var dimensionRepository = serializationContext.Resolve<IDimensionRepository>();
         protocol.TimeUnit = dimensionRepository.Time.Unit(protocolElement.GetAttribute(CoreConstants.Serialization.Attribute.TimeUnit));
      }
   }

   public class SimpleProtocolXmlSerializer : BuildingBlockXmlSerializer<SimpleProtocol>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ApplicationType);
         Map(x => x.FormulationKey);
         Map(x => x.DosingInterval);
         Map(x => x.TargetOrgan);
         Map(x => x.TargetCompartment);
      }
   }
}