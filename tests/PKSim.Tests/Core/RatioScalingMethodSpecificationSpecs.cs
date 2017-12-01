using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_RatioScalingMethodSpecification : ContextSpecification<RatioScalingMethodSpecification>
   {
      protected ParameterScaling _parameterScalingWithDefaultZero;
      private IParameterTask _parameterTask;

      protected override void Context()
      {
         _parameterScalingWithDefaultZero = A.Fake<ParameterScaling>();
         _parameterTask = A.Fake<IParameterTask>();
         sut = new RatioScalingMethodSpecification(_parameterTask);
      }
   }

   
   public class When_the_ration_scaling_method_specification_is_asked_if_a_parameter_scaling_matches_its_constraint : concern_for_RatioScalingMethodSpecification
   {
      private IParameter _parameterWithDefaultValueZero;
      private IParameter _parameterWithDefaultValueNotZero;
      private IParameter _enzymeExpressionParameter;
      private IParameter _parameterNotSetByUser;
      private ParameterScaling _parameterScalingWithDefaultSourceValueNotZero;
      private ParameterScaling _parameterScalingForEnzymeExpression;
      private ParameterScaling _parameterScalingNotSetByUser;

      protected override void Context()
      {
         base.Context();
         _enzymeExpressionParameter = A.Fake<IParameter>().WithName(CoreConstants.Parameter.REL_EXP);
        
         _parameterWithDefaultValueZero = A.Fake<IParameter>();
         _parameterWithDefaultValueZero.Value = 0;
       
         _parameterWithDefaultValueNotZero = A.Fake<IParameter>();
         _parameterWithDefaultValueNotZero.Value = 15;

         _parameterWithDefaultValueNotZero.IsFixedValue = true;
         A.CallTo(() => _parameterScalingWithDefaultZero.SourceParameter).Returns(_parameterWithDefaultValueZero);
         A.CallTo(() => _parameterScalingWithDefaultZero.TargetParameter).Returns(_parameterWithDefaultValueNotZero);
         _parameterScalingWithDefaultSourceValueNotZero = A.Fake<ParameterScaling>();
         A.CallTo(() => _parameterScalingWithDefaultSourceValueNotZero.SourceParameter).Returns(_parameterWithDefaultValueNotZero);
         A.CallTo(() => _parameterScalingWithDefaultSourceValueNotZero.TargetParameter).Returns(_parameterWithDefaultValueNotZero);

         _parameterScalingForEnzymeExpression = A.Fake<ParameterScaling>();
         A.CallTo(() => _parameterScalingForEnzymeExpression.SourceParameter).Returns(_enzymeExpressionParameter);
         A.CallTo(() => _parameterScalingForEnzymeExpression.TargetParameter).Returns(_enzymeExpressionParameter);

         _parameterScalingNotSetByUser = A.Fake<ParameterScaling>();
         _parameterNotSetByUser =A.Fake<IParameter>();
         _parameterNotSetByUser.IsFixedValue = false;
         A.CallTo(() => _parameterScalingNotSetByUser.SourceParameter).Returns(_parameterNotSetByUser);
         A.CallTo(() => _parameterScalingNotSetByUser.TargetParameter).Returns(A.Fake<IParameter>());

      }

      [Observation]
      public void should_return_false_for_a_parameter_for_which_the_default_value_is_zero()
      {
         sut.IsSatisfiedBy(_parameterScalingWithDefaultZero).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_for_a_parameter_defined_for_an_enzyme_expression()
      {
         sut.IsSatisfiedBy(_parameterScalingForEnzymeExpression).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_for_a_parameter_whose_value_was_not_set_by_the_user()
      {
         sut.IsSatisfiedBy(_parameterScalingNotSetByUser).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_for_any_other_parameters()
      {
         sut.IsSatisfiedBy(_parameterScalingWithDefaultSourceValueNotZero).ShouldBeTrue();
      }
   }

   
   public class When_the_ratio_scaling_method_specification_is_asked_to_return_the_method_for_a_parameter_scaling : concern_for_RatioScalingMethodSpecification
   {
      [Observation]
      public void the_returned_scaling_method_should_be_a_ratio_scaling_method()
      {
         sut.Method.ShouldBeAnInstanceOf<RatioScalingMethod>();
      }
   }

   
   public class When_the_ratio_scaling_method_specification_is_asked_if_is_a_default_for_a_parameter_scaling : concern_for_RatioScalingMethodSpecification
   {
      private ParameterScaling _distributedParameterSacling;
      private ParameterScaling _scalingForParameterNotSetByUser;
      private ParameterScaling _parameterScalingWithSourceValueSetByUser;

      protected override void Context()
      {
         base.Context();
         var parameterSetByUser = A.Fake<IParameter>();
         parameterSetByUser.IsFixedValue = true;
         _distributedParameterSacling = A.Fake<ParameterScaling>();
         A.CallTo(() => _distributedParameterSacling.IsDistributedScaling).Returns(true);
         _parameterScalingWithSourceValueSetByUser = A.Fake<ParameterScaling>();
         A.CallTo(() => _parameterScalingWithSourceValueSetByUser.SourceParameter).Returns(parameterSetByUser);
         A.CallTo(() => _parameterScalingWithSourceValueSetByUser.TargetParameter).Returns(A.Fake<IParameter>());
         A.CallTo(() => _distributedParameterSacling.SourceParameter).Returns(A.Fake<IDistributedParameter>());
         A.CallTo(() => _distributedParameterSacling.TargetParameter).Returns(A.Fake<IDistributedParameter>());
         _scalingForParameterNotSetByUser = A.Fake<ParameterScaling>();
         var parameterNotSet = A.Fake<IParameter>();
         parameterNotSet.IsFixedValue = false;
         A.CallTo(() => _scalingForParameterNotSetByUser.SourceParameter).Returns(parameterNotSet);
         A.CallTo(() => _scalingForParameterNotSetByUser.TargetParameter).Returns(A.Fake<IParameter>());
      }

      [Observation]
      public void should_return_true_for_a_parameter_that_was_set_by_the_user()
      {
         sut.IsDefaultFor(_parameterScalingWithSourceValueSetByUser).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_parameter_scaling_between_two_distributed_parameter()
      {
         sut.IsDefaultFor(_distributedParameterSacling).ShouldBeFalse();
      }

   }
}