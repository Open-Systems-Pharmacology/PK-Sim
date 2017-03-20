using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_ParameterRateRepository : ContextForIntegration<IParameterRateRepository>
    {
    }

    
    public class When_retrieving_all_parameter_rates_from_the_repository : concern_for_ParameterRateRepository
    {
        private IEnumerable<ParameterRateMetaData> _result;

        protected override void Because()
        {
            _result = sut.All();
            _result = sut.All();
        }

        [Observation]
        public void should_return_at_least_one_element()
        {
            _result.Count().ShouldBeGreaterThan(0);
        }
    }
}