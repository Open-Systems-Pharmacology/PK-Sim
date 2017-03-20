using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_KeepScalingMethodSpecification : ContextSpecification<KeepScalingMethodSpecification>
   {
      protected ParameterScaling _parameterScaling;

      protected override void Context()
      {
         _parameterScaling = A.Fake<ParameterScaling>();
         sut = new KeepScalingMethodSpecification();
      }
   }

   public class When_the_keep_scaling_method_specification_is_asked_if_a_parameter_scaling_matches_its_constraint : concern_for_KeepScalingMethodSpecification
   {
      [Observation]
      public void should_return_true_for_all_parameter()
      {
         sut.IsSatisfiedBy(_parameterScaling).ShouldBeTrue();
      }
   }

   public class When_the_keep_scaling_method_specification_is_asked_to_return_the_method_for_a_parameter_scaling : concern_for_KeepScalingMethodSpecification
   {
      [Observation]
      public void the_returned_scaling_method_should_be_an_override_scaling_method()
      {
         sut.Method.ShouldBeAnInstanceOf<KeepScalingMethod>();
      }
   }

   public class When_the_keep_scaling_method_specification_is_asked_if_is_a_default_for_a_parameter_scaling : concern_for_KeepScalingMethodSpecification
   {
      [Observation]
      public void should_always_return_false()
      {
         sut.IsDefaultFor(_parameterScaling).ShouldBeFalse();
      }
   }
}