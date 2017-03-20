using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class AdvancedProtocolXmlSerializer : BuildingBlockXmlSerializer<AdvancedProtocol>
   {
      private readonly IDimensionRepository _dimensionRepository;

      public AdvancedProtocolXmlSerializer()
      {
         _dimensionRepository = IoC.Resolve<IDimensionRepository>();
      }

      protected override XElement TypedSerialize(AdvancedProtocol protocol, SerializationContext serializationContext)
      {
         var protocolNode = base.TypedSerialize(protocol, serializationContext);
         protocolNode.AddAttribute(CoreConstants.Serialization.Attribute.TimeUnit, protocol.TimeUnit.Name);
         return protocolNode;
      }

      protected override void TypedDeserialize(AdvancedProtocol protocol, XElement protocolElement, SerializationContext serializationContext)
      {
         base.TypedDeserialize(protocol, protocolElement, serializationContext);
         protocol.TimeUnit = _dimensionRepository.Time.Unit(protocolElement.GetAttribute(CoreConstants.Serialization.Attribute.TimeUnit));
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