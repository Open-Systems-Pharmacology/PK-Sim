using PKSim.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Core.Model;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using ParameterMessages = PKSim.Core.Model.ParameterMessages;

namespace PKSim.Core
{
   public class When_retrieving_the_parameter_message_description_when_changing_the_unit_of_a_parameter : StaticContextSpecification
   {
      private IParameter _parameter;
      private string _description;
      private Unit _oldUnit;
      private Unit _newUnit;

      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter().WithFormula(new ConstantFormula(10));
         var dim = new Dimension(new BaseDimensionRepresentation(), "time", "h");
         _newUnit = dim.AddUnit("min", 60, 0);
         _oldUnit = dim.AddUnit("day", 1400, 0);
         _parameter.Dimension = dim;
         _parameter.DisplayUnit = _oldUnit;
         _parameter.Value = dim.UnitValueToBaseUnitValue(_oldUnit, 10);
         NumericFormatterOptions.Instance.DecimalPlace = 2;
      }

      protected override void Because()
      {
         _description = ParameterMessages.SetParameterUnit(_parameter, "P", _oldUnit, _newUnit);
      }

      [Observation]
      public void should_return_the_expected_message_formatted_with_the_value_changed_between_old_unit_and_new_unit()
      {
         _description.ShouldBeEqualTo(PKSimConstants.Command.SetParameterUnitDescription("P", "10.00", "day", "min"));
      }
   }

   public class When_retrieving_the_parameter_display_for_a_boolean_parameter_where_only_numerical_values_are_allowed : StaticContextSpecification
   {
      private PKSimParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter().WithFormula(new ConstantFormula(1)).WithName(CoreConstants.Parameter.EHC_ENABLED);
      }

      [Observation]
      public void should_return_the_boolean_value_as_string_when_string_values_are_allowed()
      {
         ParameterMessages.DisplayValueFor(_parameter, numericalDisplayOnly: false).ShouldBeEqualTo(PKSimConstants.UI.Yes);
      }

      [Observation]
      public void should_return_the_boolean_value_as_double_when_numerical_values_only_are_allowed()
      {
         ParameterMessages.DisplayValueFor(_parameter, numericalDisplayOnly: true).ShouldBeEqualTo("1");
      }
   }
}