using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ResetExpressionParameterCommand : ContextSpecification<ResetExpressionParameterCommand>
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
         _parameterToReset = DomainHelperForSpecs.ConstantParameterWithValue(_originValue).WithName("tralala").WithId("tralala").WithDimension(_dimension);
         _parameterToReset.Formula = new ExplicitFormula("10");
         container.Add(oneParameter);
         container.Add(_parameterToReset);
         _parameterToReset.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_parameterToReset, oneParameter));

         A.CallTo(() => _executionContext.Get<IParameter>(_parameterToReset.Id)).Returns(_parameterToReset);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameterToReset)).Returns(A.Fake<IPKSimBuildingBlock>());

         _parameterInContainerRepository = A.Fake<IValueOriginRepository>();
         A.CallTo(() => _executionContext.Resolve<IValueOriginRepository>()).Returns(_parameterInContainerRepository);

         sut = new ResetExpressionParameterCommand(_parameterToReset);
      }
   }

   public class When_executing_the_reset_command_for_a_non_default_expression_parameter : concern_for_ResetExpressionParameterCommand
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
      public void should_have_reset_the_parameter_value_to_its_original_value()
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

   public class When_executing_the_reset_command_for_a_default__expression_parameter : concern_for_ResetExpressionParameterCommand
   {
      private ValueOrigin _databaseValueOrigin;
      private ExpressionProfile _expressionProfile;
      private IExpressionProfileUpdater _expressionProfileUpdater;

      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
         _parameterToReset.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;

         _databaseValueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.InVivo,
            Source = ValueOriginSources.Database
         };

         _expressionProfile = new ExpressionProfile();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();

         A.CallTo(() => _parameterInContainerRepository.ValueOriginFor(_parameterToReset)).Returns(_databaseValueOrigin);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameterToReset)).Returns(_expressionProfile);
         A.CallTo(() => _executionContext.Resolve<IExpressionProfileUpdater>()).Returns(_expressionProfileUpdater);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_reset_the_parameter_value_to_its_original_value()
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
         _parameterToReset.IsDefault.ShouldBeTrue();
         _parameterToReset.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.InVivo);
         _parameterToReset.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Database);
      }

      [Observation]
      public void the_expression_update_task_should_be_used_to_synchronize_simulation_subjects()
      {
         A.CallTo(() => _expressionProfileUpdater.SynchronizeAllSimulationSubjectsWithExpressionProfile(_expressionProfile)).MustHaveHappened();
      }
   }

   public class When_executing_the_inverse_command_of_the_reset_expression_parameter_command : concern_for_ResetExpressionParameterCommand
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
      public void should_set_the_value_of_the_parameter_again_using_a_set_command()
      {
         _parameterToReset.Value.ShouldBeEqualTo(25);
         _parameterToReset.IsFixedValue.ShouldBeTrue();
      }
   }
}