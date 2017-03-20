using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DosingIntervalXmlAttributeMapper : AttributeMapper<DosingInterval, SerializationContext>
   {
      public override string Convert(DosingInterval valueToConvert, SerializationContext context)
      {
         return valueToConvert.Id.ToString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var dosingIntervalId = EnumHelper.ParseValue<DosingIntervalId>(attributeValue);
         return DosingIntervals.ById(dosingIntervalId);
      }
   }
}