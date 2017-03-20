using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_DistributionScalingMethod : ContextSpecification<ScalingMethod>
   {
      protected ParameterScaling _parameterScaling;
      protected IDistributedParameter _distributedParameter1;
      protected IDistributedParameter _distributedParameter2;
      protected double _scaledValue;
      private IParameterTask _parameterTask;

      protected override void Context()
      {
         _parameterTask = A.Fake<IParameterTask>();
         _parameterScaling = A.Fake<ParameterScaling>();
         _distributedParameter1 = A.Fake<IDistributedParameter>();
         _distributedParameter2 = A.Fake<IDistributedParameter>();
         A.CallTo(() => _parameterScaling.SourceParameter).Returns(_distributedParameter1);
         A.CallTo(() => _parameterScaling.TargetParameter).Returns(_distributedParameter2);
         _scaledValue = 25;
         _distributedParameter1.Percentile = 0.8;
         A.CallTo(() => _distributedParameter2.ValueFor(_distributedParameter1.Percentile)).Returns(_scaledValue);
         sut = new DistributionScalingMethod(_parameterTask);
      }
   }

   
   public class When_the_distribution_scaling_method_is_asked_for_the_new_value_after_scaling : concern_for_DistributionScalingMethod
   {
      [Observation]
      public void should_return_the_value_of_the_target_parameter_calculated_from_the_percentile_set_in_the_source_parameter()
      {
         sut.ScaledValueFor(_parameterScaling).ShouldBeEqualTo(_scaledValue);
      }
   }
}