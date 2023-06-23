using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using ICoreCalculationMethodRepository = PKSim.Core.Repositories.ICoreCalculationMethodRepository;

namespace PKSim.IntegrationTests
{

   public abstract class concern_for_CalculationMethodForModelRepository : ContextForIntegration<ICoreCalculationMethodRepository>
   {
   }

   
   public class When_retrieving_all_calculation_methods_for_model_from_the_repository : concern_for_CalculationMethodForModelRepository
   {
      private IEnumerable<CoreCalculationMethod> _result;

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

}
