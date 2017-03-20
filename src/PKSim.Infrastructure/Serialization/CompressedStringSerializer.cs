using OSPSuite.Utility.Compression;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Serialization
{
   public class CompressedStringSerializer : IStringSerializer
   {
      private readonly IStringSerializer _underlyingSerializationManager;
      private readonly IStringCompression _compression;

      public CompressedStringSerializer(IStringSerializer underlyingSerializationManager, IStringCompression compression)
      {
         _underlyingSerializationManager = underlyingSerializationManager;
         _compression = compression;
      }

      public string Serialize<TObject>(TObject objectToSerialize)
      {
         return _compression.Compress(_underlyingSerializationManager.Serialize(objectToSerialize));
      }

      public TObject Deserialize<TObject>(string serializationString)
      {
         return _underlyingSerializationManager.Deserialize<TObject>(_compression.Decompress(serializationString));
      }

      public void Deserialize<TObject>(TObject objectToDeserialize, string serializationString)
      {
         _underlyingSerializationManager.Deserialize(objectToDeserialize, _compression.Decompress(serializationString));
      }
   }
}