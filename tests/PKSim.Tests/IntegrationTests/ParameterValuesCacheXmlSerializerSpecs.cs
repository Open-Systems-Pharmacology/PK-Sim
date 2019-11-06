using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Populations;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_parameter_cache : ContextForSerialization<ParameterValuesCache>
   {
      private ParameterValuesCache _parameterValuesCache;
      private ParameterValuesCache _deserializedValuesCache;

      protected override void Context()
      {
         base.Context();
         _parameterValuesCache = new ParameterValuesCache();
         var parameterValue1 = new ParameterValue("PATH1", 11, 0.1);
         var parameterValue2 = new ParameterValue("PATH2", 21, 0.2);
         _parameterValuesCache.Add(new[] {parameterValue1, parameterValue2});
         _parameterValuesCache.Add(new[] {new ParameterValue(parameterValue1.ParameterPath, 12, 0.3), new ParameterValue(parameterValue2.ParameterPath, 22, 0.4)});
      }

      protected override void Because()
      {
         _deserializedValuesCache = SerializeAndDeserialize(_parameterValuesCache);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_stream()
      {
         _deserializedValuesCache.ShouldNotBeNull();
      }

      [Observation]
      public void the_deserialize_cache_should_contain_the_expected_paths()
      {
         _parameterValuesCache.AllParameterPaths().ShouldContain("PATH1", "PATH2");
      }

      [Observation]
      public void the_deserialize_cache_should_contain_the_expected_values()
      {
         _parameterValuesCache.ValuesFor("PATH1").ShouldOnlyContain(11, 12);
         _parameterValuesCache.ValuesFor("PATH2").ShouldOnlyContain(21, 22);
      }

      [Observation]
      public void the_deserialize_cache_should_contain_the_expected_percentiles()
      {
         _parameterValuesCache.PercentilesFor("PATH1").ShouldOnlyContain(0.1, 0.3);
         _parameterValuesCache.PercentilesFor("PATH2").ShouldOnlyContain(0.2, 0.4);
      }
   }
}