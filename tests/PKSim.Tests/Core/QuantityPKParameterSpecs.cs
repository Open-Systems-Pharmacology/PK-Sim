using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_QuantityPKParameter : ContextSpecification<QuantityPKParameter>
   {
      protected override void Context()
      {
         sut = new QuantityPKParameter();
      }
   }

   public class When_adding_some_pk_parameter_values : concern_for_QuantityPKParameter
   {
      protected override void Because()
      {
         sut.SetValue(0, 0.1f);
         sut.SetValue(1, 0.2f);
      }

      [Observation]
      public void should_be_able_to_retrieve_all_the_values()
      {
         sut.ValuesAsArray.ShouldOnlyContain(0.1f, 0.2f);
      }
   }
}