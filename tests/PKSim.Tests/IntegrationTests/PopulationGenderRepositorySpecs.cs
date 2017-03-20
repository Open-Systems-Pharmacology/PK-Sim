using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_PopulationGenderRepository : ContextForIntegration<IFlatPopulationGenderRepository>
    {
     }

    
    public class When_retrieving_all_populationgenders_from_the_repository : concern_for_PopulationGenderRepository
    {
        private IEnumerable<FlatPopulationGender> _result;

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