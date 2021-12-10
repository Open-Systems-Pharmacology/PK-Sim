using System.Globalization;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimParameter : ContextSpecification<IParameter>
   {
      protected double _originValue;
      protected bool _valueChangedEventRaised;

      protected override void Context()
      {
         _originValue = 15;
         var container = new Container();
         var otherParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P");
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         sut = new PKSimParameter().WithName("toto");
         container.Add(otherParameter);
         container.Add(sut);
         sut.Formula = new ExplicitFormula(_originValue.ToString(new NumberFormatInfo()));
         sut.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(sut, otherParameter));
         sut.PropertyChanged += (o, e) =>
                                   {
                                      if (e.PropertyName.Equals("Value"))
                                         _valueChangedEventRaised = true;
                                   };
      }
   }

   
   public class When_setting_the_value_for_a_parameter : concern_for_PKSimParameter
   {
      protected override void Because()
      {
         sut.Value = 20;
      }

      [Observation]
      public void the_parameter_should_be_marked_as_fixed()
      {
         sut.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void the_parameter_formula_should_not_have_been_changed()
      {
         sut.Formula.Calculate(sut).ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_value_changed_event_should_have_been_raised()
      {
         _valueChangedEventRaised.ShouldBeTrue();
      }
   }

   
   public class When_reseting_a_parameter_to_is_default_value : concern_for_PKSimParameter
   {
      protected override void Context()
      {
         base.Context();
         sut.Value = 20;
      }

      protected override void Because()
      {
         sut.ResetToDefault();
      }

      [Observation]
      public void the_parameter_should_not_be_fixed_anymore()
      {
         sut.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void the_parameter_formula_should_not_have_been_changed()
      {
         sut.Formula.Calculate(sut).ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_value_should_be_equal_to_the_formula_value()
      {
         sut.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_value_changed_event_should_have_been_raised()
      {
         _valueChangedEventRaised.ShouldBeTrue();
      }
   }

   
   public class When_setting_a_parameter_to_a_fixed_value_then_to_default_and_to_the_same_fixed_value : concern_for_PKSimParameter
   {
      protected override void Context()
      {
         base.Context();
         sut.Value = 20;
         sut.ResetToDefault();
         _valueChangedEventRaised = false;
      }

      protected override void Because()
      {
         sut.Value = 20;
      }

      [Observation]
      public void the_value_changed_event_should_have_been_raised()
      {
         _valueChangedEventRaised.ShouldBeTrue();
      }
   }

   
   public class When_setting_the_display_unit_of_a_parameter : concern_for_PKSimParameter
   {
      private Unit _unitToSet;
      private bool _unitChangedEventRaised;

      protected override void Context()
      {
         sut = DomainHelperForSpecs.ConstantParameterWithValue(10);
         sut.PropertyChanged += (o, e) =>
                                   {
                                      if (e.PropertyName.Equals("DisplayUnit"))
                                         _unitChangedEventRaised = true;
                                   };
         _unitToSet = sut.Dimension.Units.Last();
      }

      protected override void Because()
      {
         sut.DisplayUnit = _unitToSet;
      }

      [Observation]
      public void should_raise_the_property_changed_event()
      {
         _unitChangedEventRaised.ShouldBeTrue();
      }
   }
}