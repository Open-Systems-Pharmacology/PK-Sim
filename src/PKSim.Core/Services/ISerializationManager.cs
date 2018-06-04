using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Core.Services
{
   public interface IStringSerializer
   {
      string Serialize<TObject>(TObject objectToSerialize);
      TObject Deserialize<TObject>(string serializationString);
      void Deserialize<TObject>(TObject objectToDeserialize, string serializationString);
   }

   public interface ISerializationManager
   {
      byte[] Serialize<TObject>(TObject objectToSerialize);
      TObject Deserialize<TObject>(byte[] serializationBytes, SerializationContext serializationContext = null);
      void Deserialize<TObject>(TObject objectToDeserialize, byte[] serializationBytes, SerializationContext serializationContext = null);
   }
}