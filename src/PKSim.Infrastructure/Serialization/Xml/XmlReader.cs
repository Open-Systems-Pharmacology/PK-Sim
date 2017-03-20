using System;
using System.Xml.Linq;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Xml;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml
{
   public interface IXmlReader<T>
   {
      T ReadFrom(XElement element, SerializationContext serializationContext);
      void ReadFrom(T objectToRead, XElement element, SerializationContext serializationContext);
   }

   public class XmlReader<T> : IXmlReader<T> where T : class
   {
      private readonly IPKSimXmlSerializerRepository _serializerRepository;

      public XmlReader(IPKSimXmlSerializerRepository serializerRepository)
      {
         _serializerRepository = serializerRepository;
      }

      public T ReadFrom(XElement element, SerializationContext serializationContext)
      {
         var itemSerializer = resolveSerializer(typeof (T), element);
         _serializerRepository.DeserializeFormulaCache(element, serializationContext, itemSerializer.ObjectType);
         return itemSerializer.Deserialize<T>(element, serializationContext);
      }

      public void ReadFrom(T objectToRead, XElement element, SerializationContext serializationContext)
      {
         var typeToDeserialize = objectToRead.GetType();
         _serializerRepository.DeserializeFormulaCache(element, serializationContext, typeToDeserialize);
         var itemSerializer = resolveSerializer(typeToDeserialize, element);
         itemSerializer.Deserialize(objectToRead, element, serializationContext);
      }

      private IXmlSerializer<SerializationContext> resolveSerializer(Type typeOfObject, XElement element)
      {
         try
         {
            return _serializerRepository.SerializerFor(element);
         }
         //this can happen if the given type was not registered explicitly in the serializer.
         catch (SerializerNotFoundException)
         {
            return _serializerRepository.SerializerFor(typeOfObject);
         }
      }
   }
}