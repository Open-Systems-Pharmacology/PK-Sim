using System;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility.Compression;

namespace PKSim.Core.Services
{
   public interface ICompressedSerializationManager : ISerializationManager
   {
      byte[] Serialize<TObject>(TObject objectToSerialize, bool compress);
   }

   public class CompressedSerializationManager : ICompressedSerializationManager
   {
      private readonly ICompression _compression;
      private readonly ISerializationManager _serializationManager;

      public CompressedSerializationManager(ICompression compression, ISerializationManager serializationManager)
      {
         _compression = compression;
         _serializationManager = serializationManager;
      }

      public byte[] Serialize<TObject>(TObject objectToSerialize)
      {
         return _compression.Compress(_serializationManager.Serialize(objectToSerialize));
      }

      public TObject Deserialize<TObject>(byte[] serializationBytes, SerializationContext serializationContext = null)
      {
         return _serializationManager.Deserialize<TObject>(decompressedByte(serializationBytes), serializationContext);
      }

      public void Deserialize<TObject>(TObject objectToDeserialize, byte[] serializationBytes, SerializationContext serializationContext = null)
      {
         byte[] bytes;
         try
         {
            bytes = decompressedByte(serializationBytes);
         }
#pragma warning disable 0168
         catch (Exception e)
#pragma warning restore 0168
         {
            //the stream was not compressed
            bytes = serializationBytes;
         }

         _serializationManager.Deserialize(objectToDeserialize, bytes, serializationContext);
      }

      public byte[] Serialize<TObject>(TObject objectToSerialize, bool compress)
      {
         if (compress)
            return Serialize(objectToSerialize);

         return _serializationManager.Serialize(objectToSerialize);
      }

      private byte[] decompressedByte(byte[] serializationBytes)
      {
         return _compression.Decompress(serializationBytes);
      }
   }
}