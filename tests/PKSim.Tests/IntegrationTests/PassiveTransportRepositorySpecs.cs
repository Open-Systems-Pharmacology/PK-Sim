using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_PassiveTransportRepository : ContextForIntegration<IPassiveTransportRepository>
    {
    }

    
    public class When_retrieving_all_passive_transports_from_the_repository : concern_for_PassiveTransportRepository
    {
        private IEnumerable<PKSimTransport> _result;

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
        public void every_passive_transport_should_have_nonempty_source_and_target_criteria()
        {
           foreach (var transport in _result)
           {
              transport.SourceCriteria.Count.ShouldBeGreaterThan(0);
              transport.TargetCriteria.Count.ShouldBeGreaterThan(0);
           }
        }
    }

}
