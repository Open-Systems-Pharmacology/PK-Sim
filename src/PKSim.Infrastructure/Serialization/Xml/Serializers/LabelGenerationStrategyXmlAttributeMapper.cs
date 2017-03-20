using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class LabelGenerationStrategyXmlAttributeMapper : AttributeMapper<LabelGenerationStrategy, SerializationContext>
   {
      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var labelGenerationStrategyId = EnumHelper.ParseValue<LabelGenerationStrategyId>(attributeValue);
         return LabelGenerationStrategies.ById(labelGenerationStrategyId);
      }

      public override string Convert(LabelGenerationStrategy valueToConvert, SerializationContext context)
      {
         return valueToConvert.Id.ToString();
      }
   }
}