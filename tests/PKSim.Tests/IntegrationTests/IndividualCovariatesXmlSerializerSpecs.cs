using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Populations;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CovariatesValuesXmlSerializer : ContextForSerialization<CovariateValues>
   {
     
   }

   public class When_serializing_an_individual_covariate : concern_for_CovariatesValuesXmlSerializer
   {
      private CovariateValues _covariates;
      private CovariateValues _deserialized;

      protected override void Context()
      {
         base.Context();
         _covariates = new CovariateValues("toto");
         _covariates.Add( "Value1");
         _covariates.Add( "Value2");
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_covariates);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_individual_covariates()
      {
         _deserialized.Count.ShouldBeEqualTo(2);
         _deserialized.Values.ShouldOnlyContainInOrder("Value1", "Value2");
      }
   }
}