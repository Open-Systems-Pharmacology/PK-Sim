using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SpeciesRepository : ContextForIntegration<ISpeciesRepository>
   {
   }

   
   public class When_retrieving_all_species_from_the_repository : concern_for_SpeciesRepository
   {
      private IEnumerable<Species> _result;

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
      public void all_species_should_have_at_least_one_population()
      {
         foreach (var species in _result)
            species.Populations.Count().ShouldBeGreaterThan(0);
      }
   }
}