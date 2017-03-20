using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_OverrideScalingMethodSpecification : ContextSpecification<OverrideScalingMethodSpecification>
   {
      protected ParameterScaling _parameterScaling;
      private IParameterTask _parmeterTask;

      protected override void Context()
      {
         _parameterScaling = A.Fake<ParameterScaling>();
         _parmeterTask = A.Fake<IParameterTask>();
         sut = new OverrideScalingMethodSpecification(_parmeterTask);
      }
   }

   public class When_the_override_scaling_method_specification_is_asked_if_a_parameter_scaling_matches_its_constraint : concern_for_OverrideScalingMethodSpecification
   {
      [Observation]
      public void should_return_true_for_all_parameter()
      {
         sut.IsSatisfiedBy(_parameterScaling).ShouldBeTrue();
      }
   }

   public class When_the_override_scaling_method_specification_is_asked_to_return_the_method_for_a_parameter_scaling : concern_for_OverrideScalingMethodSpecification
   {
      [Observation]
      public void the_returned_scaling_method_should_be_an_override_scaling_method()
      {
         sut.Method.ShouldBeAnInstanceOf<OverrideScalingMethod>();
      }
   }

   public class When_the_override_scaling_method_specification_is_asked_if_is_a_default_for_a_parameter_scaling : concern_for_OverrideScalingMethodSpecification
   {
      private ParameterScaling _parameterScalingWithSourceNotSetByUser;
      private ParameterScaling _distributedScaling;

      protected override void Context()
      {
         base.Context();
         _parameterScalingWithSourceNotSetByUser = A.Fake<ParameterScaling>();
         var parameterWithValueNotSetByUser = A.Fake<IParameter>();
         parameterWithValueNotSetByUser.IsFixedValue = false;
         A.CallTo(() => _parameterScalingWithSourceNotSetByUser.IsDistributedScaling).Returns(false);
         A.CallTo(() => _parameterScalingWithSourceNotSetByUser.SourceParameter).Returns(parameterWithValueNotSetByUser);
         A.CallTo(() => _parameterScalingWithSourceNotSetByUser.TargetParameter).Returns(A.Fake<IParameter>());

         _distributedScaling = A.Fake<ParameterScaling>();
         A.CallTo(() => _distributedScaling.IsDistributedScaling).Returns(true);
      }

      [Observation]
      public void should_return_false_for_a_source_parameter_that_is_not_distributed_and_that_was_set_not_set_by_the_user()
      {
         sut.IsDefaultFor(_parameterScalingWithSourceNotSetByUser).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_for_a_distributed_parameter()
      {
         sut.IsDefaultFor(_distributedScaling).ShouldBeFalse();
      }
   }
}