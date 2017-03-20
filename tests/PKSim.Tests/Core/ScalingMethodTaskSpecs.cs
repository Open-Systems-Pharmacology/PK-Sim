using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using FakeItEasy;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ScalingMethodTask : ContextSpecification<IScalingMethodTask>
   {
      protected IRepository<IScalingMethodSpecification> _repository;
      protected IScalingMethodSpecification _scalingMethodSpecOK1;
      protected IScalingMethodSpecification _scalingMethodSpecNotOK1;
      protected IScalingMethodSpecification _scalingMethodSpecOK2;
      protected ParameterScaling _parameterScaling;
      protected ScalingMethod _scalingMethod1;
      protected ScalingMethod _scalingMethod2;

      protected override void Context()
      {
         _scalingMethodSpecOK1 = A.Fake<IScalingMethodSpecification>();
         _scalingMethodSpecNotOK1 = A.Fake<IScalingMethodSpecification>();
         _scalingMethodSpecOK2 = A.Fake<IScalingMethodSpecification>();
         _repository = A.Fake<IRepository<IScalingMethodSpecification>>();
         _parameterScaling = A.Fake<ParameterScaling>();

         _scalingMethod1 = A.Fake<ScalingMethod>();
         _scalingMethod2 = A.Fake<ScalingMethod>();

         A.CallTo(() => _scalingMethodSpecOK1.IsSatisfiedBy(_parameterScaling)).Returns(true);
         A.CallTo(() => _scalingMethodSpecOK2.IsSatisfiedBy(_parameterScaling)).Returns(true);
         A.CallTo(() => _scalingMethodSpecNotOK1.IsSatisfiedBy(_parameterScaling)).Returns(false);

         A.CallTo(() => _scalingMethodSpecOK1.Method).Returns(_scalingMethod1);
         A.CallTo(() => _scalingMethodSpecOK2.Method).Returns(_scalingMethod2);
         A.CallTo(() => _repository.All()).Returns(new[] {_scalingMethodSpecOK1, _scalingMethodSpecOK2, _scalingMethodSpecNotOK1});
         sut = new ScalingMethodTask(_repository);
      }
   }

   public class When_retrieving_all_scaling_methods_for_a_give_parameter_scaling_configuration : concern_for_ScalingMethodTask
   {
      private IEnumerable<ScalingMethod> _result;

      protected override void Because()
      {
         _result = sut.AllMethodsFor(_parameterScaling);
      }

      [Observation]
      public void should_ask_all_available_scaling_methods_if_the_give_parameter_scaling_matches_their_scaling_criteria()
      {
         A.CallTo(() => _scalingMethodSpecOK1.IsSatisfiedBy(_parameterScaling)).MustHaveHappened();
         A.CallTo(() => _scalingMethodSpecOK2.IsSatisfiedBy(_parameterScaling)).MustHaveHappened();
         A.CallTo(() => _scalingMethodSpecNotOK1.IsSatisfiedBy(_parameterScaling)).MustHaveHappened();
      }

      [Observation]
      public void should_return_only_the_scaling_methods_avaliable_for_the_parameter_scaling()
      {
         _result.ShouldOnlyContain(_scalingMethod1, _scalingMethod2);
      }
   }

   public class When_retrieving_the_default_scaling_method_for_a_parameter_scaling_for_which_at_least_one_default_method_was_define : concern_for_ScalingMethodTask
   {
      private ScalingMethod _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _scalingMethodSpecOK1.IsDefaultFor(_parameterScaling)).Returns(false);
         A.CallTo(() => _scalingMethodSpecOK2.IsDefaultFor(_parameterScaling)).Returns(true);
         A.CallTo(() => _scalingMethodSpecNotOK1.IsDefaultFor(_parameterScaling)).Returns(false);
         A.CallTo(() => _scalingMethodSpecOK2.Method).Returns(A.Fake<ScalingMethod>());
      }

      protected override void Because()
      {
         _result = sut.DefaultMethodFor(_parameterScaling);
      }

      [Observation]
      public void should_query_all_available_scaling_methods_until_one_found_as_default_for_that_parameter_scaling()
      {
         A.CallTo(() => _scalingMethodSpecOK1.IsDefaultFor(_parameterScaling)).MustHaveHappened();
         A.CallTo(() => _scalingMethodSpecOK2.IsDefaultFor(_parameterScaling)).MustHaveHappened();
         A.CallTo(() => _scalingMethodSpecNotOK1.IsDefaultFor(_parameterScaling)).MustNotHaveHappened();
      }

      [Observation]
      public void should_return_the_first_available_scaling_method_marked_as_default()
      {
         _result.ShouldBeEqualTo(_scalingMethodSpecOK2.Method);
      }
   }

   public class When_retrieving_the_default_scaling_method_for_a_parameter_scaling_for_which_no_default_method_was_define : concern_for_ScalingMethodTask
   {
      private ScalingMethod _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _scalingMethodSpecOK1.IsDefaultFor(_parameterScaling)).Returns(false);
         A.CallTo(() => _scalingMethodSpecOK2.IsDefaultFor(_parameterScaling)).Returns(false);
         A.CallTo(() => _scalingMethodSpecNotOK1.IsDefaultFor(_parameterScaling)).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.DefaultMethodFor(_parameterScaling);
      }

      [Observation]
      public void should_return_the_override_scaling_method()
      {
         _result.ShouldBeAnInstanceOf<KeepScalingMethod>();
      }
   }
}