using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_RateObjectPathsRepository : ContextForIntegration<IRateObjectPathsRepository>
    {
    }

    
    public class When_getting_rate_paths : concern_for_RateObjectPathsRepository
    {
        private IEnumerable<IFormulaUsablePath> _rateObjectPaths1;
        private IEnumerable<IFormulaUsablePath> _rateObjectPaths2;
        private IEnumerable<IRateObjectPaths> _allRateObjectPaths;

        protected override void Because()
        {
            _allRateObjectPaths = sut.All();
            _allRateObjectPaths = sut.All();

            const string calcMethod = "Individual_PKSim";
            const string rate = "PARAM_Q_lng";

            _rateObjectPaths1 = sut.ObjectPathsFor(rate,calcMethod);
            _rateObjectPaths2 = sut.ObjectPathsFor(new RateKey(calcMethod, rate));
        }

        [Observation]
        public void should_return_at_least_one_rate_path()
        {
            _allRateObjectPaths.Count().ShouldBeGreaterThan(0);
        }

        [Observation]
        public void should_return_same_path_by_ratekey_and_calcmethodrate()
        {
            _rateObjectPaths1.ShouldBeEqualTo(_rateObjectPaths2);
        }
    }
}	