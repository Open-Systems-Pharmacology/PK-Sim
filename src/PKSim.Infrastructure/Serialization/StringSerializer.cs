using System.Text;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Serialization
{
   public class StringSerializer : IStringSerializer
   {
      private readonly ISerializationManager _serializationManager;

      public StringSerializer(ISerializationManager serializationManager)
      {
         _serializationManager = serializationManager;
      }

      public string Serialize<TObject>(TObject objectToSerialize)
      {
         var bytes = _serializationManager.Serialize(objectToSerialize);
         return Encoding.UTF8.GetString(bytes);
      }

      public TObject Deserialize<TObject>(string serializationString)
      {
         return _serializationManager.Deserialize<TObject>(bytesFrom(serializationString));
      }

      public void Deserialize<TObject>(TObject objectToDeserialize, string serializationString)
      {
         _serializationManager.Deserialize(objectToDeserialize, bytesFrom(serializationString));
      }

      private byte[] bytesFrom(string serializationString)
      {
         return Encoding.UTF8.GetBytes(serializationString);
      }
   }
}