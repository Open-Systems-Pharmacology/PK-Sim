using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Compression;
using OSPSuite.Core.Domain;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization;
using FakeItEasy;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_CompressedSerializationManager : ContextSpecification<IStringSerializer>
   {
      protected IStringCompression _compression;
      protected IStringSerializer _underlyingSerializationManager;
      protected string _compressedString;
      protected IObjectBase _deserializedObject;
      private string _uncompressedSting;

      protected override void Context()
      {
         _compression = A.Fake<IStringCompression>();
         _deserializedObject = A.Fake<IObjectBase>();
         _underlyingSerializationManager = A.Fake<IStringSerializer>();
         _compressedString = "aa";
         _uncompressedSting = "bb";
         A.CallTo(() => _compression.Decompress(_compressedString)).Returns(_uncompressedSting);
         A.CallTo(() => _compression.Compress(_uncompressedSting)).Returns(_compressedString);
         A.CallTo(() => _underlyingSerializationManager.Deserialize<IObjectBase>(_uncompressedSting)).Returns(_deserializedObject);
         A.CallTo(() => _underlyingSerializationManager.Serialize(_deserializedObject)).Returns(_uncompressedSting);
         sut = new CompressedStringSerializer(_underlyingSerializationManager, _compression);
      }
   }

   
   public class When_deserializing_an_object_from_a_compressed_string : concern_for_CompressedSerializationManager
   {
      [Observation]
      public void should_decompress_the_string_and_leverage_the_underlying_deserialize_the_object()
      {
         sut.Deserialize<IObjectBase>(_compressedString).ShouldBeEqualTo(_deserializedObject);
      }
   }

   
   public class When_serializing_an_object_to_a_compressed_string : concern_for_CompressedSerializationManager
   {
      [Observation]
      public void should_decompress_the_string_and_leverage_the_underlying_deserialize_the_object()
      {
         sut.Serialize(_deserializedObject).ShouldBeEqualTo(_compressedString);
      }
   }
}