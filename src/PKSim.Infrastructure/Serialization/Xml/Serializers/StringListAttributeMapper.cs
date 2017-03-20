using System.Collections.Generic;
using OSPSuite.Serializer.Attributes;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class StringListAttributeMapper : AttributeMapper<IList<string>, SerializationContext>
   {
      public override string Convert(IList<string> stringList, SerializationContext context)
      {
         return stringList.ToPathString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return new List<string>(attributeValue.ToPathArray());
      }
   }
}