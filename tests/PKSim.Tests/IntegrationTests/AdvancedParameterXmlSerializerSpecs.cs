using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serializing_an_advanced_parameter : ContextForSerialization<AdvancedParameter>
   {
      private AdvancedParameter _advancedParameter;
      private AdvancedParameter _deserializedAdvancedParameter;

      protected override void Context()
      {
         base.Context();
         var advancedParameterFactory = IoC.Resolve<IAdvancedParameterFactory>();
         _advancedParameter = advancedParameterFactory.CreateDefaultFor(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P"));
      }

      protected override void Because()
      {
         _deserializedAdvancedParameter = SerializeAndDeserialize(_advancedParameter);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_stream()
      {
         _deserializedAdvancedParameter.ShouldNotBeNull();
      }

      [Observation]
      public void should_be_able_to_generate_new_random_values()
      {
         _deserializedAdvancedParameter.GenerateRandomValues(10).Count().ShouldBeEqualTo(10);
      }
   }
}