using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ParameterValueRepository : ContextForIntegration<IParameterValueRepository>
   {
   }

   public class When_retrieving_all_parameter_values_from_the_repository : concern_for_ParameterValueRepository
   {
      private IEnumerable<ParameterValueMetaData> _result;

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
      public void should_have_updated_the_value_origin_of_all_parameters_defined_in_the_repository()
      {
         foreach (var parameterValueMetaData in _result)
         {
            parameterValueMetaData.ValueOrigin.ShouldNotBeNull();
         }
      }
   }
}