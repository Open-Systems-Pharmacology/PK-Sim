using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_string_cache : ContextForSerialization<Cache<string, string>>
   {
      private Cache<string, string> _cache;
      private Cache<string, string> _deserialized;

      protected override void Context()
      {
         base.Context();
         _cache = new Cache<string, string>();
         _cache.Add("Key1", "Value1");
         _cache.Add("Key2", "Value2");
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_cache);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_cache()
      {
         _deserialized.Count.ShouldBeEqualTo(2);
         _deserialized["Key1"].ShouldBeEqualTo("Value1");
         _deserialized["Key2"].ShouldBeEqualTo("Value2");
      }
   }
}