using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility.Container;
using OSPSuite.Core.Domain;
using PKSim.Core.Extensions;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ObjectPathXmlAttributeMapper : AttributeMapper<IObjectPath,SerializationContext>
   {
      private readonly IObjectPathFactory _objectPathFactory;

      public ObjectPathXmlAttributeMapper() : this(IoC.Resolve<IObjectPathFactory>())
      {
      }

      public ObjectPathXmlAttributeMapper(IObjectPathFactory objectPathFactory)
      {
         _objectPathFactory = objectPathFactory;
      }

      public override string Convert(IObjectPath objectPath, SerializationContext context)
      {
         return objectPath == null ? string.Empty : objectPath.ToString();
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         if (string.IsNullOrEmpty(attributeValue))
            return new ObjectPath();

         return _objectPathFactory.CreateObjectPathFrom(attributeValue.ToPathArray());
      }
   }
}