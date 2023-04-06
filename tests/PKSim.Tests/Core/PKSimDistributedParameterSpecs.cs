using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Core.Maths.Statistics;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimDistributedParameter : ContextSpecification<IDistributedParameter>
   {
      protected IParameter _meanParameter;
      protected IParameter _stdParameter;
      protected IParameter _defaultPercentileParameter;
      protected IObjectPathFactory _pathFactory;
      private PKSimParameter _percentileParameter;

      protected override void Context()
      {
         sut = new PKSimDistributedParameter();
         _pathFactory = new ObjectPathFactoryForSpecs();
         _meanParameter = new PKSimParameter {Name = Constants.Distribution.MEAN}.WithFormula(new ExplicitFormula("10"));
         _stdParameter = new PKSimParameter {Name = Constants.Distribution.DEVIATION}.WithFormula(new ExplicitFormula("10"));
         _percentileParameter = new PKSimParameter {Name = Constants.Distribution.PERCENTILE}.WithFormula(new ExplicitFormula("0.5"));
         sut.Add(_meanParameter);
         sut.Add(_stdParameter);
         sut.Add(_percentileParameter);
         sut.Formula = new NormalDistributionFormula();
         sut.Formula.AddObjectPath(_pathFactory.CreateRelativeFormulaUsablePath(sut, _meanParameter));
         sut.Formula.AddObjectPath(_pathFactory.CreateRelativeFormulaUsablePath(sut, _stdParameter));
      }
   }

   public class When_setting_the_value_of_a_distributed_parameter : concern_for_PKSimDistributedParameter
   {
      private double _internalValue;

      protected override void Context()
      {
         base.Context();
         _internalValue = 30;
      }

      protected override void Because()
      {
         sut.Value = _internalValue;
      }

      [Observation]
      public void the_parameter_should_not_be_marked_as_set_by_the_user()
      {
         sut.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void the_parameter_value_should_have_been_set_to_the_internal_value()
      {
         //since the parameter is not fixed, value will be calculated from percentile=>we allow tolerance
         sut.Value.ShouldBeEqualTo(_internalValue, 1e-6);
      }
   }

   public class When_resetting_a_distributed_parameter : concern_for_PKSimDistributedParameter
   {
      protected override void Context()
      {
         base.Context();
         sut.DefaultValue = sut.Value;
         sut.Value = 10;
      }

      protected override void Because()
      {
         sut.ResetToDefault();
      }

      [Observation]
      public void the_parameter_should_not_be_marked_as_being_fixed()
      {
         sut.IsFixedValue.ShouldBeFalse();
      }
   }

   public abstract class concern_for_PKSimDistributedParameterUniformDistributed : ContextSpecification<IDistributedParameter>
   {
      protected IParameter _minimumParameter;
      protected IParameter _maximumParameter;
      protected IParameter _defaultPercentileParameter;
      protected ObjectPathFactory _pathFactory;
      private PKSimParameter _percentileParameter;
      protected DistributionFormula _distributedFormula;
      protected RandomGenerator _randomGenerator;
      protected double _minValue;
      protected double _maxValue;
      protected double _value;

      protected override void Context()
      {
         _randomGenerator = new RandomGenerator();
         sut = new PKSimDistributedParameter().WithName("P");
         _minValue = 5;
         _maxValue = 10;
         sut.Info.MinValue = _minValue;
         sut.Info.MaxValue = _maxValue;

         _pathFactory = new ObjectPathFactoryForSpecs();
         _minimumParameter = new PKSimParameter {Name = Constants.Distribution.MINIMUM}.WithFormula(new ExplicitFormula("0"));
         _maximumParameter = new PKSimParameter {Name = Constants.Distribution.MAXIMUM}.WithFormula(new ExplicitFormula("1"));
         _percentileParameter = new PKSimParameter {Name = Constants.Distribution.PERCENTILE}.WithFormula(new ExplicitFormula("0.5"));
         sut.Add(_minimumParameter);
         sut.Add(_maximumParameter);
         sut.Add(_percentileParameter);
         _distributedFormula = new UniformDistributionFormula();
         _distributedFormula.AddObjectPath(_pathFactory.CreateRelativeFormulaUsablePath(sut, _minimumParameter).WithAlias(Constants.Distribution.MINIMUM));
         _distributedFormula.AddObjectPath(_pathFactory.CreateRelativeFormulaUsablePath(sut, _maximumParameter).WithAlias(Constants.Distribution.MAXIMUM));
         sut.Formula = _distributedFormula;
      }

      protected override void Because()
      {
         _value = sut.RandomDeviateIn(_randomGenerator);
      }
   }

   public class When_generating_a_random_uniform_deviate_with_a_parameter_min_bigger_than_the_distribution_min : concern_for_PKSimDistributedParameterUniformDistributed
   {
      protected override void Context()
      {
         base.Context();
         _minimumParameter.Value = 1;
         _maximumParameter.Value = 8;
      }

      [Observation]
      public void should_use_the_parameter_min_as_lower_bound()
      {
         _value.ShouldBeGreaterThan(_minValue);
         _value.ShouldBeSmallerThan(_maximumParameter.Value);
      }
   }

   public class When_generating_a_random_uniform_deviate_with_a_parameter_min_smaller_than_the_distribution_min : concern_for_PKSimDistributedParameterUniformDistributed
   {
      protected override void Context()
      {
         base.Context();
         _minimumParameter.Value = 7;
         _maximumParameter.Value = 9;
      }

      [Observation]
      public void should_use_the_distributrion_min_as_lower_bound()
      {
         _value.ShouldBeGreaterThan(_minimumParameter.Value);
         _value.ShouldBeSmallerThan(_maximumParameter.Value);
      }
   }

   public class When_generating_a_random_uniform_deviate_with_a_parameter_max_smaller_than_the_distribution_max : concern_for_PKSimDistributedParameterUniformDistributed
   {
      protected override void Context()
      {
         base.Context();
         _minimumParameter.Value = 7;
         _maximumParameter.Value = 12;
      }

      [Observation]
      public void should_use_the_parameter_max_as_upper_bound()
      {
         _value.ShouldBeGreaterThan(_minimumParameter.Value);
         _value.ShouldBeSmallerThan(_maxValue);
      }
   }

   public class When_generating_a_random_uniform_deviate_with_a_parameter_max_bigger_than_the_distribution_max : concern_for_PKSimDistributedParameterUniformDistributed
   {
      protected override void Context()
      {
         base.Context();
         _minimumParameter.Value = 7;
         _maximumParameter.Value = 9;
      }

      [Observation]
      public void should_use_the_distribution_max_as_upper_bound()
      {
         _value.ShouldBeGreaterThan(_minimumParameter.Value);
         _value.ShouldBeSmallerThan(_maximumParameter.Value);
      }
   }

   public class When_generating_a_random_uniform_deviate_with_a_distribution_max_bigger_than_the_parameter_max_and_a_distribution_min_bigger_than_the_max : concern_for_PKSimDistributedParameterUniformDistributed
   {
      protected override void Context()
      {
         base.Context();
         _minimumParameter.Value = 15;
         _maximumParameter.Value = 20;
      }

      protected override void Because()
      {
         //required to override default implementation
      }

      [Observation]
      public void should_use_the_distribution_max_as_upper_bound()
      {
         The.Action(() => sut.RandomDeviateIn(_randomGenerator)).ShouldThrowAn<LimitsNotInUniformDistributionFunctionRangeException>();
      }
   }
}