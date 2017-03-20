using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_RatioScalingMethod : ContextSpecification<ScalingMethod>
   {
      protected ParameterScaling _parameterScaling;
      protected IParameter _targetParameter;
      protected IParameter _sourceParameter;
      protected double _scaledValue;
      private IParameterTask _parameterTask;

      protected override void Context()
      {
         _parameterScaling = A.Fake<ParameterScaling>();
         _targetParameter = A.Fake<IParameter>();
         _sourceParameter = A.Fake<IParameter>();
         _parameterTask = A.Fake<IParameterTask>();
         _sourceParameter.Value = 10;
         _targetParameter.Value = 50;
         _sourceParameter.DefaultValue = 20;
         A.CallTo(() => _parameterScaling.SourceParameter).Returns(_sourceParameter);
         A.CallTo(() => _parameterScaling.TargetParameter).Returns(_targetParameter);
         A.CallTo(() => _parameterScaling.TargetValue).Returns(_targetParameter.Value);

         _scaledValue = _targetParameter.Value * _sourceParameter.Value / _sourceParameter.DefaultValue.Value;
         sut = new RatioScalingMethod(_parameterTask);
      }
   }

   
   public class When_the_ratio_scaling_method_is_asked_for_the_new_value_after_scaling : concern_for_RatioScalingMethod
   {
      [Observation]
      public void should_return_a_value_proportional_to_the_ratio_of_the_default_value_with_the_current_value_of_the_source_parameter()
      {
         sut.ScaledValueFor(_parameterScaling).ShouldBeEqualTo(_scaledValue);
      }
   }
}