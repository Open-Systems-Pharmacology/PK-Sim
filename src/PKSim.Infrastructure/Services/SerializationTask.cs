using OSPSuite.Serializer.Xml;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization;

namespace PKSim.Infrastructure.Services
{
   public class SerializationTask : ISerializationTask
   {
      private readonly IStringSerializer _stringSerializer;
      private readonly IObjectIdResetter _objectIdResetter;

      public SerializationTask(IStringSerializer stringSerializer, IObjectIdResetter objectIdResetter)
      {
         _stringSerializer = stringSerializer;
         _objectIdResetter = objectIdResetter;
      }

      public void SaveModelPart<T>(T objectToSerialize, string filename)
      {
         var xmlContent = _stringSerializer.Serialize(objectToSerialize);
         XmlHelper.SaveXmlContentToFile(xmlContent, filename);
      }

      public T Load<T>(string fileName, bool resetIds = false)
      {
         var xmlContent = XmlHelper.XmlContentFromFile(fileName);
         var deserializedObject = _stringSerializer.Deserialize<T>(xmlContent);
         if (resetIds)
            _objectIdResetter.ResetIdFor(deserializedObject);

         return deserializedObject;
      }
   }
}