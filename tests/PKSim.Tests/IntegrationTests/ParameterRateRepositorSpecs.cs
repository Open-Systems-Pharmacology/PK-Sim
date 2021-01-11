using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

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


      [Observation]
      public void should_have_set_the_default_flag_to_the_expected_parameters_for_some_know_parameters()
      {
         var lipophilicityParameter = _result.Find(x => x.BuildingBlockType == PKSimBuildingBlockType.Compound &&
                                                        x.ParameterName == CoreConstants.Parameters.LIPOPHILICITY);

         lipophilicityParameter.IsInput.ShouldBeTrue();
      }
   }
}