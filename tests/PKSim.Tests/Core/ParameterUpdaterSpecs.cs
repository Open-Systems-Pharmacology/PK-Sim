using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterUpdater : ContextSpecification<IParameterUpdater>
   {
      protected IParameter _sourceParameter;
      protected IParameter _targetParameter;
      protected IParameterToFomulaTypeMapper _formulaTypeMapper;
      protected ICommand _result;
      protected IParameterTask _parameterTask;
      protected IExecutionContext _executionContext;
      private IEntityPathResolver _entityPathResolver;
      private IFavoriteTask _favoriteTask;

      protected override void Context()
      {
         _formulaTypeMapper = A.Fake<IParameterToFomulaTypeMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _favoriteTask = A.Fake<IFavoriteTask>();
         _parameterTask = new ParameterTask(_entityPathResolver, _executionContext,_favoriteTask);
         sut = new ParameterUpdater(_formulaTypeMapper, _parameterTask);
         _sourceParameter = DomainHelperForSpecs.ConstantParameterWithValue(0);
         _sourceParameter.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _targetParameter = DomainHelperForSpecs.ConstantParameterWithValue(1);
         _targetParameter.BuildingBlockType = PKSimBuildingBlockType.Individual;
         A.CallTo(() => _executionContext.BuildingBlockContaining(_targetParameter)).Returns(A.Fake<IPKSimBuildingBlock>());
      }

      protected override void Because()
      {
         _result = sut.UpdateValue(_sourceParameter, _targetParameter);
      }
   }

   public class When_updating_the_value_of_a_constant_parameter_with_a_constant_parameter_having_the_same_value : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.Value = 10;
         _targetParameter.Value = _sourceParameter.Value;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Constant);
      }

      [Observation]
      public void should_not_do_anything()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_updating_the_value_of_a_constant_parameter_with_a_new_value : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.Value = 10;
         _targetParameter.Value = 15;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Constant);
      }

      [Observation]
      public void should_update_the_value_of_the_parameter_with_the_value_if_the_value_has_changed()
      {
         _targetParameter.Value.ShouldBeEqualTo(_sourceParameter.Value);
      }
   }

   public class When_updating_the_value_of_a_rate_parameter_whose_value_was_overriden_with_a_parameter_having_a_different_value_set_by_the_user : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.Value = 10;
         _targetParameter.Value = 15;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Constant);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Rate);
      }

      [Observation]
      public void should_update_the_value_of_the_parameter_with_the_value_if_the_value_has_changed()
      {
         _targetParameter.Value.ShouldBeEqualTo(_sourceParameter.Value);
      }
   }

   public class When_updating_the_value_of_an_unchanged_rate_parameter_from_a_parameter_whose_value_was_not_set_by_the_user : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.IsFixedValue = false;
         _targetParameter.IsFixedValue = false;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Rate);
      }

      [Observation]
      public void should_not_update_the_target_parameter()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_updating_the_value_of_an_unchanged_constant_parameter_from_a_parameter_whose_value_was_not_set_by_the_user_but_the_display_unit_was_changed : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.IsFixedValue = false;
         _sourceParameter.DisplayUnit = _sourceParameter.Dimension.Units.First();
         _targetParameter = DomainHelperForSpecs.ConstantParameterWithValue(0);
         _targetParameter.DisplayUnit = _sourceParameter.Dimension.Units.Last();

         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Constant);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Constant);
      }

      [Observation]
      public void should_update_the_display_unit_of_the_target_parameter()
      {
         _result.IsEmpty().ShouldBeFalse();
      }
   }

   public class When_updating_two_constant_parameters_with_different_values_and_display_units : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.DisplayUnit = _sourceParameter.Dimension.Units.First();
         _targetParameter.DisplayUnit = _sourceParameter.Dimension.Units.Last();

         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Constant);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Constant);
      }

      [Observation]
      public void should_update_the_value_and_the_display_unit_of_the_target_parameter()
      {
         _result.IsEmpty().ShouldBeFalse();
         _result.DowncastTo<IPKSimMacroCommand>().Count.ShouldBeEqualTo(2);
      }
   }

   public class When_updating_a_relative_expression_parameters_with_different_values : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Constant);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Constant);
         _sourceParameter.Name = CoreConstants.Parameter.REL_EXP;
         _targetParameter.Name = CoreConstants.Parameter.REL_EXP;
      }

      [Observation]
      public void should_return_a_simple_parmaeter_set_value_and_not_a_relative_expression_set_value_command()
      {
         _result.ShouldBeAnInstanceOf<SetParameterValueCommand>();
      }
   }

   public class When_updating_the_value_of_an_unchanged_rate_parameter_from_a_parameter_whose_value_was_set_by_the_user : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _targetParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _sourceParameter.IsFixedValue = true;
         _targetParameter.IsFixedValue = false;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Rate);
      }

      [Observation]
      public void should_update_the_target_parameter_even_if_the_values_are_the_same()
      {
         _result.IsEmpty().ShouldBeFalse();
         _targetParameter.Value.ShouldBeEqualTo(_sourceParameter.Value);
      }
   }

   public class When_updating_the_value_of_a_rate_parameter_whose_value_was_overriden_from_a_parameter_whose_value_was_unchanged : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         _sourceParameter.IsFixedValue = false;
         _targetParameter.IsFixedValue = true;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Rate);
      }

      [Observation]
      public void should_reset_the_target_parameter_to_its_original_formula()
      {
         _result.IsEmpty().ShouldBeFalse();
         _targetParameter.Value.ShouldBeEqualTo(_targetParameter.Value);
      }
   }

   public class When_updating_a_distributed_parameter_from_a_source_parameter_that_is_a_distributed_parameter_with_the_same_percentile : concern_for_ParameterUpdater
   {
      protected IDistributedParameter _sourceDistributedParameter;
      protected IDistributedParameter _targetDistributedParameter;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Distribution);
         _sourceDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _targetDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();

         _sourceDistributedParameter.Percentile = 0.5;
         _targetDistributedParameter.Percentile = 0.5;

         _targetParameter = _targetDistributedParameter;
         _sourceParameter = _sourceDistributedParameter;
      }

      [Observation]
      public void should_not_update_the_percentile_value()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_updating_a_distributed_parameter_from_a_source_parameter_that_is_a_not_a_distributed_parameter_but_with_the_same_value : concern_for_ParameterUpdater
   {
      protected IDistributedParameter _sourceDistributedParameter;
      protected IDistributedParameter _targetDistributedParameter;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Distribution);
         _targetDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _targetParameter = _targetDistributedParameter;
         _sourceParameter.Value = 1.2;
         _targetParameter.Value = 1.2;
      }

      [Observation]
      public void should_not_update_the_value()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_updating_a_distributed_parameter_from_a_source_parameter_with_a_different_value_that_is_a_not_a_distributed_parameter : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Distribution);
         var targetDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _targetParameter = targetDistributedParameter;
         _sourceParameter.Value = 1.5;
         _targetParameter.Value = 1.0;
         A.CallTo(() => _executionContext.BuildingBlockContaining(_targetParameter)).Returns(A.Fake<IPKSimBuildingBlock>());
      }

      [Observation]
      public void should_not_update_the_value_of_the_parameter()
      {
         _targetParameter.Value.ShouldBeEqualTo(_sourceParameter.Value, 1e-7);
      }
   }

   public class When_updating_a_distributed_parameter_from_a_distributed_table_parameter_having_the_same_percentile : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Distribution);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Table);
         var targetDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _targetParameter = targetDistributedParameter;
         _sourceParameter.Formula = new DistributedTableFormula {Percentile = targetDistributedParameter.Percentile};
      }

      [Observation]
      public void should_have_used_a_command_that_is_not_a_set_percentile_command()
      {
         _result.IsAnImplementationOf<SetParameterPercentileCommand>().ShouldBeFalse();
      }
   }

   public class When_updating_a_distributed_parameter_from_a_distributed_table_parameter_having_the_different_percentile : concern_for_ParameterUpdater
   {
      private IDistributedParameter _targetDistributedParameter;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Distribution);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Table);
         _targetDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _targetParameter = _targetDistributedParameter;
         _sourceParameter.Formula = new DistributedTableFormula {Percentile = 0.2};
      }

      [Observation]
      public void should_have_set_the_percentile_of_the_source_formula_into_the_target_parameter()
      {
         _targetDistributedParameter.Percentile.ShouldBeEqualTo(0.2);
      }
   }

   public class When_updating_a_distributed_table_parameter_from_a_distributed_parameter_having_the_same_percentile : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Table);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Distribution);
         var sourceDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _sourceParameter = sourceDistributedParameter;
         _targetParameter.Formula = new DistributedTableFormula {Percentile = 0.5};
      }

      [Observation]
      public void should_have_let_the_table_formula_unchanged()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_updating_a_distributed_table_parameter_from_a_distributed_parameter_having_a_different_percentile : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Table);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Distribution);
         var sourceDistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _sourceParameter = sourceDistributedParameter;
         var distributedTableFormula = new DistributedTableFormula();
         _targetParameter.Formula = distributedTableFormula;
      }

      [Observation]
      public void should_return_an_update_distributed_table_formula_command()
      {
         _result.ShouldBeAnInstanceOf<UpdateDistributedTableFormulaPercentileCommand>();
      }
   }

   public class When_updating_a_table_parameter_from_a_constant_parameter : concern_for_ParameterUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_targetParameter)).Returns(FormulaType.Table);
         A.CallTo(() => _formulaTypeMapper.MapFrom(_sourceParameter)).Returns(FormulaType.Constant);
         _targetParameter.Formula = new TableFormula();
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }
}