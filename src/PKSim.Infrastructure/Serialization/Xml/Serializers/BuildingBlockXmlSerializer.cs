using System.Xml.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class BuildingBlockXmlSerializer<T> : EntityXmlSerializer<T>, IPKSimXmlSerializer where T : class, IPKSimBuildingBlock
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Version);
         Map(x => x.StructureVersion);
         Map(x => x.Root);
         Map(x => x.Creation);
      }

      protected override XElement TypedSerialize(T buildingBlock, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(buildingBlock, serializationContext);
         SerializerRepository.AddFormulaCacheElement(element, serializationContext);
         return element;
      }

      protected override void TypedDeserialize(T buildingBlock, XElement element, SerializationContext serializationContext)
      {
         SerializerRepository.DeserializeFormulaCacheIn(element, serializationContext);
         base.TypedDeserialize(buildingBlock, element, serializationContext);
      }
   }
}