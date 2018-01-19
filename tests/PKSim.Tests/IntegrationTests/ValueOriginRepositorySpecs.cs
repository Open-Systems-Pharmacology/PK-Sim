using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ValueOriginRepository : ContextForIntegration<IValueOriginRepository>
   {
      protected void Should_be_undefined(ValueOrigin valueOrigin)
      {
         valueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Undefined);
         valueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.Undefined);
      }
   }

   public class When_return_the_value_origin_repository_for_an_id_that_does_not_exist_in_the_database : concern_for_ValueOriginRepository
   {
      [Observation]
      public void should_return_the_default_value_origin()
      {
         Should_be_undefined(sut.FindBy(-50));
         Should_be_undefined(sut.FindBy(null));
      }
   }

   public class When_return_the_value_origin_repository_for_a_null_parameter : concern_for_ValueOriginRepository
   {
      [Observation]
      public void should_return_the_default_value_origin()
      {
         Should_be_undefined(sut.ValueOriginFor(null));
      }
   }

   public class When_return_the_expected_value_origin_repository_for_an_existing_id : concern_for_ValueOriginRepository
   {
      [Observation]
      public void should_return_the_default_value_origin()
      {
         var valueOrigin = sut.FindBy(1);
         valueOrigin.Source.ShouldNotBeEqualTo(ValueOriginSources.Undefined);
      }
   }
}