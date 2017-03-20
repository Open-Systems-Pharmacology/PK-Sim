using System.Linq;
using System.Xml.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class UsedBuildingBlockXmlSerializer : BaseXmlSerializer<UsedBuildingBlock>
   {
      public override void PerformMapping()
      {
         Map(x => x.Id);
         Map(x => x.TemplateId);
         Map(x => x.Name);
         Map(x => x.Altered);
         Map(x => x.BuildingBlockType);
         Map(x => x.Version);
         Map(x => x.StructureVersion);
      }

      protected override XElement TypedSerialize(UsedBuildingBlock usedBuildingBlock, SerializationContext context)
      {
         var usedBuildingBlockNode = base.TypedSerialize(usedBuildingBlock, context);

         var serializer = SerializerRepository.SerializerFor(usedBuildingBlock.BuildingBlock);
         usedBuildingBlockNode.Add(serializer.Serialize(usedBuildingBlock.BuildingBlock, context));
         return usedBuildingBlockNode;
      }

      protected override void TypedDeserialize(UsedBuildingBlock usedBuildingBlock, XElement usedBuildingBlockNode, SerializationContext context)
      {
         base.TypedDeserialize(usedBuildingBlock, usedBuildingBlockNode, context);
         var buildingBlockNode = usedBuildingBlockNode.Elements().First();
         var serializer = SerializerRepository.SerializerFor(buildingBlockNode);

         usedBuildingBlock.BuildingBlock = serializer.Deserialize<IPKSimBuildingBlock>(buildingBlockNode, context);
      }
   }
}