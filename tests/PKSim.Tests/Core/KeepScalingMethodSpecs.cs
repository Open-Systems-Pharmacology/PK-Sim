using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_KeepScalingMethod : ContextSpecification<ScalingMethod>
   {
      protected ParameterScaling _parameterScaling;
      protected IParameter _targetParameter;

      protected override void Context()
      {
         _targetParameter = A.Fake<IParameter>();
         _targetParameter.Value = 25;
         _parameterScaling = A.Fake<ParameterScaling>();
         A.CallTo(() => _parameterScaling.TargetValue).Returns(_targetParameter.Value);
         sut = new KeepScalingMethod();
      }
   }

   public class When_the_keep_scaling_method_is_asked_for_the_new_value_after_scaling : concern_for_KeepScalingMethod
   {
      [Observation]
      public void should_return_the_value_of_the_target_parameter()
      {
         sut.ScaledValueFor(_parameterScaling).ShouldBeEqualTo(_targetParameter.Value);
      }
   }
}