using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_DistributionScalingMethodSpecification : ContextSpecification<DistributionScalingMethodSpecification>
   {
      protected ParameterScaling _distributedScaling;
      protected ParameterScaling _nonDistributedScaling;
      protected IParameter _nonDistribuedParameter1;
      private IParameterTask _parameterTask;

      protected override void Context()
      {
         _distributedScaling = A.Fake<ParameterScaling>();
         _nonDistributedScaling = A.Fake<ParameterScaling>();
         _nonDistribuedParameter1 = A.Fake<IParameter>();
         A.CallTo(() => _distributedScaling.IsDistributedScaling).Returns(true);


         _parameterTask = A.Fake<IParameterTask>();
         sut = new DistributionScalingMethodSpecification(_parameterTask);
      }
   }

   
   public class When_the_distribution_scaling_method_specification_is_asked_if_a_parameter_scaling_matches_its_constraint : concern_for_DistributionScalingMethodSpecification
   {
      [Observation]
      public void should_return_true_for_a_parameter_scaling_between_2_distributed_parameter()
      {
         sut.IsSatisfiedBy(_distributedScaling).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_parameter_scaling_between_one_or_more_parameter_that_is_not_distributed()
      {
         sut.IsSatisfiedBy(_nonDistributedScaling).ShouldBeFalse();
      }
   }

   
   public class When_the_distribution_scaling_method_specification_is_asked_to_return_the_method_for_a_parameter_scaling : concern_for_DistributionScalingMethodSpecification
   {
      [Observation]
      public void the_returned_scaling_method_should_be_a_distribution_scaling_method()
      {
         sut.Method.ShouldBeAnInstanceOf<DistributionScalingMethod>();
      }
   }

   
   public class When_the_distribution_scaling_method_specification_is_asked_if_is_a_default_for_a_parameter_scaling : concern_for_DistributionScalingMethodSpecification
   {
      [Observation]
      public void should_return_true_for_any_parameter_scaling_that_is_a_scaling_between_2_distributed_parameters()
      {
         sut.IsDefaultFor(_distributedScaling).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_any_parameter_scaling_that_is_not_a_scaling_between_2_distributed_parameters()
      {
         sut.IsDefaultFor(_nonDistributedScaling).ShouldBeFalse();
      }
   }
}