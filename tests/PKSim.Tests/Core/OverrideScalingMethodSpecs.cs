using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_OverrideScalingMethod : ContextSpecification<ScalingMethod>
   {
      protected ParameterScaling _parameterScaling;
      protected IParameter _sourceParameter;
      private IParameterTask _parameteTask;

      protected override void Context()
      {
         _parameteTask = A.Fake<IParameterTask>();
         _parameterScaling = A.Fake<ParameterScaling>();
         _sourceParameter = A.Fake<IParameter>();
         _sourceParameter.Value = 50;
         A.CallTo(() => _parameterScaling.SourceParameter).Returns(_sourceParameter);
         sut = new OverrideScalingMethod(_parameteTask);
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
}