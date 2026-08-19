using System.Linq;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Format;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_OverwriteParameterValueDTO : ContextSpecification<OverwriteParameterValueDTO>
   {
      protected static readonly NumericFormatter<double> _numericFormatter = new(NumericFormatterOptions.Instance);
      protected ParameterValue _parameterValue;
      protected IDimension _dimension;

      protected override void Context()
      {
         _dimension = DomainHelperForSpecs.LengthDimensionForSpecs();
         _parameterValue = new ParameterValue
         {
            Path = "Organism|Aspirin|Permeability".ToObjectPath(),
            Value = 0.05,
            Dimension = _dimension,
            DisplayUnit = _dimension.Unit("cm"),
            Info = new ParameterInfo { MinValue = 0, MinIsAllowed = true, MaxValue = 0.2, MaxIsAllowed = true }
         };

         _parameterValue.ValueOrigin.Description = "From literature";

         sut = new OverwriteParameterValueDTO(_parameterValue);
      }
   }

   public class When_displaying_an_overwrite_parameter_value : concern_for_OverwriteParameterValueDTO
   {
      [Observation]
      public void should_return_the_value_in_display_unit()
      {
         sut.Value.ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_return_the_display_unit_of_the_parameter_value()
      {
         sut.DisplayUnit.Name.ShouldBeEqualTo("cm");
      }

      [Observation]
      public void should_return_all_units_of_the_dimension()
      {
         sut.AllUnits.ShouldOnlyContain(_dimension.Units.ToArray());
      }

      [Observation]
      public void should_return_the_value_origin_of_the_parameter_value()
      {
         sut.ValueOrigin.Description.ShouldBeEqualTo("From literature");
      }

      [Observation]
      public void should_return_the_base_unit_value_for_a_value_given_in_display_unit()
      {
         sut.ValueInBaseUnit(5).ShouldBeEqualTo(0.05);
      }
   }

   public class When_validating_a_value_entered_for_an_overwrite_parameter_value : concern_for_OverwriteParameterValueDTO
   {
      [Observation]
      public void should_accept_a_value_within_the_allowed_range()
      {
         sut.Validate(x => x.Value, 5.0).IsEmpty.ShouldBeTrue();
      }

      [Observation]
      public void should_reject_a_value_smaller_than_the_minimum_and_report_the_minimum_in_display_unit()
      {
         var brokenRules = sut.Validate(x => x.Value, -1.0);
         brokenRules.IsEmpty.ShouldBeFalse();
         brokenRules.Message.ShouldBeEqualTo(Validation.ValueBiggerThanMin("Permeability", _numericFormatter.Format(0), "cm"));
      }

      [Observation]
      public void should_reject_a_value_greater_than_the_maximum_and_report_the_maximum_in_display_unit()
      {
         var brokenRules = sut.Validate(x => x.Value, 21.0);
         brokenRules.IsEmpty.ShouldBeFalse();
         brokenRules.Message.ShouldBeEqualTo(Validation.ValueSmallerThanMax("Permeability", _numericFormatter.Format(20), "cm"));
      }

      [Observation]
      public void should_accept_an_empty_value()
      {
         sut.Validate(x => x.Value, null).IsEmpty.ShouldBeTrue();
      }
   }

   public class When_validating_a_value_entered_for_an_overwrite_parameter_value_whose_allowed_range_is_unknown : concern_for_OverwriteParameterValueDTO
   {
      protected override void Context()
      {
         base.Context();
         _parameterValue.Info = null;
      }

      [Observation]
      public void should_accept_any_value()
      {
         sut.Validate(x => x.Value, -1.0).IsEmpty.ShouldBeTrue();
      }
   }

   public class When_validating_a_value_entered_for_an_overwrite_parameter_value_that_may_not_be_equal_to_its_minimum : concern_for_OverwriteParameterValueDTO
   {
      protected override void Context()
      {
         base.Context();
         _parameterValue.Info.MinIsAllowed = false;
      }

      [Observation]
      public void should_reject_the_minimum_value()
      {
         sut.Validate(x => x.Value, 0.0).IsEmpty.ShouldBeFalse();
      }
   }
}
