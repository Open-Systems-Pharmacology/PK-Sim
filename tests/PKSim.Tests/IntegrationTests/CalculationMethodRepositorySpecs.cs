using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CalculationMethodRepository : ContextForIntegration<ICalculationMethodRepository>
   {
   }

   public class When_retrieving_all_calculation_methods_from_the_repository : concern_for_CalculationMethodRepository
   {
      private IEnumerable<CalculationMethod> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_a_calculation_method_that_does_not_exist : concern_for_CalculationMethodRepository
   {
      [Observation]
      public void should_throw_a_calculation_method_not_found_exception()
      {
         The.Action(() => sut.FindBy("toto")).ShouldThrowAn<CalculationMethodNotFoundException>();
      }
   }
}