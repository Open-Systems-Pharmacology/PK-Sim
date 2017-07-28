using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatPopulationContainerRepository : ContextForIntegration<IFlatPopulationContainerRepository>
   {
   }

   public class When_retrieving_all_containers_from_repository : concern_for_FlatPopulationContainerRepository
   {
      private IEnumerable<FlatPopulationContainer> _result;

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