using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core
{
   public abstract class concern_for_ResetParameterCommand : ContextSpecification<ResetParameterCommand>
   {
      protected IParameter _parameterToReset;
      protected IExecutionContext _executionContext;
      protected double _originValue;
      private IDimension _dimension;
      protected IValueOriginRepository _parameterInContainerRepository;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _dimension = A.Fake<IDimension>();
         _originValue = 10;
         var container = new Container();
         var oneParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         _parameterToReset = DomainHelperForSpecs.ConstantParameterWithValue(_originValue).WithId("tralala").WithDimension(_dimension);
         _parameterToReset.Formula = new ExplicitFormula("10");
         container.Add(oneParameter);
         container.Add(_parameterToReset);
         _parameterToReset.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_parameterToReset, oneParameter));

         A.CallTo(() => _executionContext.Get<IParameter>(_parameterToReset.Id)).Returns(_parameterToReset);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameterToReset)).Returns(A.Fake<IPKSimBuildingBlock>());

         _parameterInContainerRepository = A.Fake<IValueOriginRepository>();
         A.CallTo(() => _executionContext.Resolve<IValueOriginRepository>()).Returns(_parameterInContainerRepository);

         sut = new ResetParameterCommand(_parameterToReset);
      }
   }

   public class When_executing_the_reset_command_for_a_non_default_parameter : concern_for_ResetParameterCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
         _parameterToReset.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;
         var valueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.Assumption,
            Source = ValueOriginSources.Internet,
         };

         A.CallTo(() => _parameterInContainerRepository.ValueOriginFor(_parameterToReset)).Returns(valueOrigin);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_reseted_the_parameter_value_to_its_original_value()
      {
         _parameterToReset.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_should_have_been_marked_as_fixed()
      {
         _parameterToReset.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_reset_the_parameter_value_origin()
      {
         _parameterToReset.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.Assumption);
         _parameterToReset.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Internet);
      }
   }

   public class When_executing_the_reset_command_for_a_default_parameter : concern_for_ResetParameterCommand
   {
      private ValueOrigin _databaseValueOrigin;

      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.ValueOrigin.Default = false;
         _parameterToReset.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
         _parameterToReset.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;

         _databaseValueOrigin = new ValueOrigin
         {
            Default = true,
            Method = ValueOriginDeterminationMethods.Measurement,
            Source = ValueOriginSources.Database
         };
         

         A.CallTo(() => _parameterInContainerRepository.ValueOriginFor(_parameterToReset)).Returns(_databaseValueOrigin);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_reseted_the_parameter_value_to_its_original_value()
      {
         _parameterToReset.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_should_have_been_marked_as_fixed()
      {
         _parameterToReset.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_reset_the_parameter_default_state_to_true()
      {
         _parameterToReset.ValueOrigin.Default.ShouldBeTrue();
         _parameterToReset.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.Measurement);
         _parameterToReset.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Database);
      }
   }

   public class When_executing_the_inverse_command_of_the_reset_parameter_command : concern_for_ResetParameterCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_simply_set_the_value_of_the_parameter_again_using_a_set_command()
      {
         _parameterToReset.Value.ShouldBeEqualTo(25);
         _parameterToReset.IsFixedValue.ShouldBeTrue();
      }
   }
}