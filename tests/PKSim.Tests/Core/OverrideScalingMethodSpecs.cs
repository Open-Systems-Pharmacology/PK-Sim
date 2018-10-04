using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_OverrideScalingMethod : ContextSpecification<ScalingMethod>
   {
      protected ParameterScaling _parameterScaling;
      protected IParameter _sourceParameter;
      protected IParameterTask _parameterTask;
      protected IParameter _targetParameter;

      protected override void Context()
      {
         _parameterTask = A.Fake<IParameterTask>();
         _parameterScaling = A.Fake<ParameterScaling>();
         _sourceParameter = DomainHelperForSpecs.ConstantParameterWithValue(20);
         _targetParameter = DomainHelperForSpecs.ConstantParameterWithValue(30);
         _sourceParameter.ValueOrigin.UpdateFrom(new ValueOrigin {Method = ValueOriginDeterminationMethods.Assumption, Source = ValueOriginSources.ParameterIdentification});
         A.CallTo(() => _parameterScaling.SourceParameter).Returns(_sourceParameter);
         A.CallTo(() => _parameterScaling.TargetParameter).Returns(_targetParameter);
         sut = new OverrideScalingMethod(_parameterTask);
      }
   }

   public class When_the_override_scaling_method_is_asked_for_the_new_value_after_scaling : concern_for_OverrideScalingMethod
   {
      [Observation]
      public void should_return_the_value_of_the_source_parameter()
      {
         sut.ScaledValueFor(_parameterScaling).ShouldBeEqualTo(_sourceParameter.Value);
      }
   }

   public class When_performing_the_parameter_scaking_for_the_override_scaling_method : concern_for_OverrideScalingMethod
   {
      private IPKSimCommand _setParameterValueCommand;
      private IPKSimCommand _setParameterValueOriginCommand;

      protected override void Context()
      {
         base.Context();
         _setParameterValueCommand = A.Fake<IPKSimCommand>();
         _setParameterValueOriginCommand = A.Fake<IPKSimCommand>();

         A.CallTo(() => _parameterTask.SetParameterValue(_targetParameter, _sourceParameter.Value, true)).Returns(_setParameterValueCommand);
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetParameter, _sourceParameter.ValueOrigin)).Returns(_setParameterValueOriginCommand);
      }

      protected override void Because()
      {
         sut.Scale(_parameterScaling);
      }

      [Observation]
      public void should_update_the_target_parmaeter_value_with_the_source_parameter_value()
      {
         A.CallTo(() => _parameterTask.SetParameterValue(_targetParameter, _sourceParameter.Value, true)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_value_origin_of_the_target_using_the_value_origin_of_the_source_parameter()
      {
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetParameter, _sourceParameter.ValueOrigin)).MustHaveHappened();
      }
   }
}