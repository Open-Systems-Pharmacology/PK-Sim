using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;


namespace PKSim.Core
{
   public abstract class concern_for_AdvancedParameter : ContextSpecification<AdvancedParameter>
   {
      private IDistributedParameter _distributedParameter;

      protected override void Context()
      {
         _distributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         sut = new AdvancedParameter {DistributedParameter = _distributedParameter};
      }
   }

   public class When_generationg_some_random_values_consecutively : concern_for_AdvancedParameter
   {
      private IReadOnlyList<double> _values1;
      private IReadOnlyList<double> _values2;

      protected override void Because()
      {
         _values1 = sut.GenerateRandomValues(10).Select(x => x.Value).ToList();
         _values2 = sut.GenerateRandomValues(10).Select(x => x.Value).ToList();
      }

      [Observation]
      public void should_return_the_same_values()
      {
         _values1.ShouldBeEqualTo(_values2);
      }
   }
}	