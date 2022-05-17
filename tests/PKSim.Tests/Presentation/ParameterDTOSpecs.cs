using System.ComponentModel;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Validation;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterDTO : ContextSpecification<ParameterDTO>
   {
      protected IParameter _parameter;
      protected IBusinessRuleSet _businessRuleSet;
      protected IBusinessRule _ruleForValue;
      protected IBusinessRule _anotherRule;

      protected override void Context()
      {
         _ruleForValue = A.Fake<IBusinessRule>();
         _anotherRule = A.Fake<IBusinessRule>();
         A.CallTo(() => _ruleForValue.IsRuleFor("Value")).Returns(true);
         A.CallTo(() => _anotherRule.IsRuleFor("Value")).Returns(false);
         _businessRuleSet = new BusinessRuleSet(_ruleForValue, _anotherRule);
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _parameter.Rules).Returns(_businessRuleSet);

         sut = new ParameterDTO(_parameter);
      }
   }

   public class When_an_event_is_raised_in_the_underlying_parameter : concern_for_ParameterDTO
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.PropertyChanged += (o, e) => { _eventRaised = e.PropertyName == "Value"; };
      }

      protected override void Because()
      {
         _parameter.PropertyChanged += Raise.FreeForm.With(_parameter, new PropertyChangedEventArgs("Value"));
      }

      [Observation]
      public void should_propagate_the_event()
      {
         _eventRaised.ShouldBeTrue();
      }
   }

   public class When_performing_validation_for_a_given_property : concern_for_ParameterDTO
   {
      private double _valueInDisplayUnit;
      private double _kernelValue;

      protected override void Context()
      {
         base.Context();
         _parameter.Editable = true;
         _valueInDisplayUnit = 5;
         _kernelValue = 3;
         A.CallTo(() => _parameter.Dimension.UnitValueToBaseUnitValue(_parameter.DisplayUnit, _valueInDisplayUnit)).Returns(_kernelValue);
         sut = new ParameterDTO(_parameter);
      }

      protected override void Because()
      {
         sut.Validate(item => item.Value, _valueInDisplayUnit);
      }

      [Observation]
      public void should_use_the_validation_rules_defined_in_the_underlying_parameter()
      {
         A.CallTo(() => _ruleForValue.IsSatisfiedBy(_parameter, _kernelValue)).MustHaveHappened();
      }
   }

   public class When_performing_validation_for_a_given_property_for_a_parameter_that_is_not_editable : concern_for_ParameterDTO
   {
      private double _valueToValidate;

      protected override void Context()
      {
         base.Context();
         _parameter.Editable = false;
         _valueToValidate = 5;
      }

      protected override void Because()
      {
         sut.Validate(item => item.Value, _valueToValidate);
      }

      [Observation]
      public void should_not_use_the_underlying_validation()
      {
         A.CallTo(() => _ruleForValue.IsSatisfiedBy(_parameter, _valueToValidate)).MustNotHaveHappened();
      }
   }

   public class When_asked_for_the_percentile_value_of_a_non_distributed_parameter : concern_for_ParameterDTO
   {
      [Observation]
      public void should_return_zero()
      {
         sut.Percentile.ShouldBeEqualTo(0);
      }
   }

   public class When_asked_for_the_percentile_value_of_a_distributed_parameter : concern_for_ParameterDTO
   {
      private double _percentileValue;

      protected override void Context()
      {
         base.Context();
         var distributedParameter = A.Fake<IDistributedParameter>();
         A.CallTo(() => distributedParameter.Rules).Returns(_businessRuleSet);
         _percentileValue = 0.5;
         distributedParameter.Percentile = _percentileValue;
         _parameter = distributedParameter;
         sut = new ParameterDTO(_parameter);
      }

      [Observation]
      public void should_return_the_percentile_of_the_underlying_parameter_in_percent()
      {
         sut.Percentile.ShouldBeEqualTo(_percentileValue * 100);
      }
   }

   public class When_asked_for_the_value_of_a_parameter : concern_for_ParameterDTO
   {
      private double _convertedValue;

      protected override void Context()
      {
         base.Context();
         _parameter.Value = 20;
         _convertedValue = 30;
         A.CallTo(() => _parameter.TryGetValueInDisplayUnit())
            .Returns((_convertedValue, true));
      }

      [Observation]
      public void should_return_the_value_of_the_parameter_in_the_user_unit()
      {
         sut.Value.ShouldBeEqualTo(_convertedValue);
      }
   }

   public class When_asked_for_the_value_of_a_parameter_in_kernel_unit : concern_for_ParameterDTO
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Value = 20;
      }

      [Observation]
      public void should_return_the_value_of_the_parameter()
      {
         sut.KernelValue.ShouldBeEqualTo(_parameter.Value);
      }
   }

   public class When_retrieving_the_value_of_a_parameter_dto_for_which_the_underlying_parameter_formula_cannot_be_computed : concern_for_ParameterDTO
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Formula = new ExplicitFormula("A + 5");
      }

      [Observation]
      public void should_return_a_value_of_zero()
      {
         sut.Value.ShouldBeEqualTo(0);
      }
   }
}