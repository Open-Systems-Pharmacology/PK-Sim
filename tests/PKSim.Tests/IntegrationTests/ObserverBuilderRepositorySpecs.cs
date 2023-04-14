using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ObserverBuilderRepository : ContextForIntegration<IObserverBuilderRepository>
   {
      }

   
   public class When_retrieving_all_observer_builders_from_the_repository : concern_for_ObserverBuilderRepository
   {
      private IEnumerable<IPKSimObserverBuilder> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void every_container_observer_should_have_nonempty_criteria_list()
      {
         foreach (var containerObserverBuilder in _result.Select(observer => observer.ObserverBuilder).OfType<ContainerObserverBuilder>())
         {
            containerObserverBuilder.ContainerCriteria.Count().ShouldBeGreaterThan(0);
         }
      }
   }

}
