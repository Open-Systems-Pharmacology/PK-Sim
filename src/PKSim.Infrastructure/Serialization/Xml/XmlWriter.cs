using System.Xml.Linq;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml
{
   public interface IXmlWriter<T>
   {
      XElement WriteFor(T objectToSerialize, SerializationContext serializationContext);
   }

   public class XmlWriter<T> : IXmlWriter<T>
   {
      private readonly IPKSimXmlSerializerRepository _serializerRepository;

      public XmlWriter(IPKSimXmlSerializerRepository serializerRepository)
      {
         _serializerRepository = serializerRepository;
      }

      public virtual XElement WriteFor(T objectToSerialize, SerializationContext serializationContext)
      {
         var itemSerializer = _serializerRepository.SerializerFor(objectToSerialize);
         var element = itemSerializer.Serialize(objectToSerialize, serializationContext);
         _serializerRepository.SerializeFormulaCache(element, serializationContext, objectToSerialize.GetType());
         return element;
      }
   }
}