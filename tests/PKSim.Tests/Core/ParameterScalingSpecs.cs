using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterScaling : ContextSpecification<ParameterScaling>
   {
      protected IParameter _sourceParameter;
      protected IParameter _targetParameter;
   }

   public class When_retrieving_the_scaled_value_for_a_parameter_in_gui_unit : concern_for_ParameterScaling
   {
      private ScalingMethod _scalingMethod;
     
      protected override void Context()
      {
         _targetParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _sourceParameter = DomainHelperForSpecs.ConstantParameterWithValue(20);
         _targetParameter.ValueInDisplayUnit = 30;
         _scalingMethod = A.Fake<ScalingMethod>();
         sut = new ParameterScaling(_sourceParameter, _targetParameter) {ScalingMethod = _scalingMethod};
      }

      [Observation]
      public void should_use_the_gui_unit_from_the_target_parameter_to_calculate_the_scaled_value_in_gui_unit()
      {
         sut.TargetScaledValueInDisplayUnit.ShouldBeEqualTo(_targetParameter.ValueInDisplayUnit);
      }
   }

   public class When_destructuring_the_parameter_scaling : concern_for_ParameterScaling
   {
      protected override void Context()
      {
         _targetParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _sourceParameter = DomainHelperForSpecs.ConstantParameterWithValue(20);
         sut = new ParameterScaling(_sourceParameter, _targetParameter);
      }

      [Observation]
      public void should_reutrn_the_expected_parameters()
      {
         var (source, target) = sut;
         source.ShouldBeEqualTo(_sourceParameter);
         target.ShouldBeEqualTo(_targetParameter);
      }
   }

   public class When_retrieving_the_scaled_value_for_a_parameter : concern_for_ParameterScaling
   {
      private ScalingMethod _scalingMethod;
      private double _scaledValue;

      protected override void Context()
      {
         _targetParameter = A.Fake<IParameter>();
         _sourceParameter = A.Fake<IParameter>();
         _scaledValue = 20;
         _scalingMethod = A.Fake<ScalingMethod>();
         sut = new ParameterScaling(_sourceParameter, _targetParameter);
         sut.ScalingMethod = _scalingMethod;
         A.CallTo(() => _scalingMethod.ScaledValueFor(sut)).Returns(_scaledValue);
      }

      [Observation]
      public void should_leverage_the_active_scaling_method_and_calculate_the_scaled_value_for_the_target_parameter()
      {
         sut.ScaledValue.ShouldBeEqualTo(_scaledValue);
      }
   }

   public class When_performing_the_actual_scaling : concern_for_ParameterScaling
   {
      private ScalingMethod _scalingMethod;
      private IPKSimCommand _command;

      protected override void Context()
      {
         _command = A.Fake<IPKSimCommand>();
         _targetParameter = A.Fake<IParameter>();
         _sourceParameter = A.Fake<IParameter>();
         _scalingMethod = A.Fake<ScalingMethod>();
         sut = new ParameterScaling(_sourceParameter, _targetParameter) {ScalingMethod = _scalingMethod};
         A.CallTo(() => _scalingMethod.Scale(sut)).Returns(_command);
         
      }

      [Observation]
      public void should_return_the_command_created_by_the_scaling_action_from_the_scaling_method()
      {
         sut.Scale().ShouldBeEqualTo(_command);
      }
   }

   public class When_asked_if_a_scaling_between_parameters_with_at_least_one_not_distributed_is_a_distributed_scaling : concern_for_ParameterScaling
   {
      private ScalingMethod _scalingMethod;

      protected override void Context()
      {
         _targetParameter = A.Fake<IParameter>();
         _sourceParameter = A.Fake<IParameter>();
         _scalingMethod = A.Fake<ScalingMethod>();
         sut = new ParameterScaling(_sourceParameter, _targetParameter) {ScalingMethod = _scalingMethod};
      }

      [Observation]
      public void should_return_false()
      {
         sut.IsDistributedScaling.ShouldBeFalse();
      }
   }

   public class When_asked_if_a_scaling_between_two_distributed_parametersis_a_distributed_scaling : concern_for_ParameterScaling
   {
      private ScalingMethod _scalingMethod;

      protected override void Context()
      {
         _targetParameter = A.Fake<IDistributedParameter>();
         _sourceParameter = A.Fake<IDistributedParameter>();
         _scalingMethod = A.Fake<ScalingMethod>();
         sut = new ParameterScaling(_sourceParameter, _targetParameter);
         sut.ScalingMethod = _scalingMethod;
      }

      [Observation]
      public void should_return_true()
      {
         sut.IsDistributedScaling.ShouldBeTrue();
      }
   }
}