using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_FlatParameterRateRepository : ContextForIntegration<IFlatParameterRateRepository>
    {
    }

    
    public class When_resolving_all_parameter_rates_definied_as_a_flat_table : concern_for_FlatParameterRateRepository
    {
        private IEnumerable<ParameterRateMetaData> _result;

        protected override void Because()
        {
            _result = sut.All();
        }

        [Observation]
        public void should_retrieve_some_object_from_the_underlying_database()
        {
            _result.Count().ShouldBeGreaterThan(0);
            ParameterRateMetaData firstElement = _result.ElementAt(0);
            firstElement.Rate.ShouldNotBeNull();
            firstElement.CalculationMethod.ShouldNotBeNull();
        }
    }
}	