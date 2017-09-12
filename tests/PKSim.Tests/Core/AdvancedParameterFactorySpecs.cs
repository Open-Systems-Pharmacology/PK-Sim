using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core
{
   public abstract class concern_for_AdvancedParameterFactory : ContextSpecification<IAdvancedParameterFactory>
   {
      private IEntityPathResolver _entityPathResolver;
      protected IParameterFactory _parameterFactory;
      private IObjectBaseFactory _objectBaseFactory;
      protected IDistributedParameter _distributedParameter;
      protected ParameterDistributionMetaData _distributionMetaData;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _distributedParameter = A.Fake<IDistributedParameter>();
         A.CallTo(() => _parameterFactory.CreateFor(A<ParameterDistributionMetaData>._))
            .Invokes(x => _distributionMetaData = x.GetArgument<ParameterDistributionMetaData>(0)).Returns(_distributedParameter);

         sut = new AdvancedParameterFactory(_entityPathResolver, _parameterFactory, _objectBaseFactory);
      }
   }

   public class When_creating_a_uniform_advanced_parameter_for_a_parmaeter_that_has_no_max_values : concern_for_AdvancedParameterFactory
   {
      private AdvancedParameter _advancedParameter;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>();
         _parameter.Value = 10;
      }

      protected override void Because()
      {
         _advancedParameter = sut.Create(_parameter, DistributionTypes.Uniform);
      }

      [Observation]
      public void should_create_a_max_value_that_is_at_least_1000_time_bigger_the_default_value()
      {
         _distributionMetaData.MaxValue.Value.ShouldBeGreaterThan(1000 * _parameter.Value);
      }
   }
}