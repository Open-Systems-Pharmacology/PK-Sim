using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Format;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterRange : ContextSpecification<ParameterRange>
   {
      protected override void Context()
      {
         sut = new ParameterRange
         {
            Dimension = DomainHelperForSpecs.TimeDimensionForSpecs(),
            ParameterName = "PARAM"
         };
         sut.Unit = sut.Dimension.Unit("h");
      }

      protected void InitRange(double? min = null, double? max = null, double? dbMin = null, double? dbMax = null)
      {
         sut.MinValue = min;
         sut.MaxValue = max;
         sut.DbMinValue = dbMin;
         sut.DbMaxValue = dbMax;
      }
   }

   public class When_checking_if_a_value_in_display_unit_for_minimum_is_valid : concern_for_ParameterRange
   {
      [TestCase(null, null, null, 10.0, true)]
      [TestCase(null, 60.0, 120.0, 1.5, true)]
      [TestCase(null, 60.0, 120.0, 0.5, false)]
      [TestCase(80.0, 60.0, 120.0, 1.0, true)]
      [TestCase(80.0, null, null, 2.0, false)]
      public void should_return_the_validation_according_to_acceptable_range(double? max, double? dbMin, double? dbMax, double? valueInHours, bool valid)
      {
         InitRange(null, max, dbMin, dbMax);
         sut.Validate(x => x.MinValueInDisplayUnit, valueInHours).IsEmpty.ShouldBeEqualTo(valid);
      }

      [TestCase(90, 1)]
      public void should_return_a_message_when_invalid_using_the_display_database_values(double? dbMin, double? valueInHours)
      {
         InitRange(null, null, dbMin);
         var formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
         var validation = sut.Validate(x => x.MinValueInDisplayUnit, valueInHours);
         validation.IsEmpty.ShouldBeEqualTo(false);
         validation.Message.ShouldBeEqualTo(PKSimConstants.Rules.Parameter.MinGreaterThanDbMinValue(sut.ParameterName, formatter.Format(sut.DbMinValueInDisplayUnit.Value), sut.Unit.Name));
      }
   }

   public class When_checking_if_a_value_in_display_unit_for_maximum_is_valid : concern_for_ParameterRange
   {
      [TestCase(null, null, null, 10.0, true)]
      [TestCase(null, 60.0, 120.0, 1.5, true)]
      [TestCase(null, 60.0, 120.0, 3.0, false)]
      [TestCase(80.0, 60.0, 120.0, 1.0, false)]
      [TestCase(60.0, null, null, 2.0, true)]
      public void should_return_the_validation_according_to_acceptable_range(double? min, double? dbMin, double? dbMax, double? valueInHours, bool valid)
      {
         InitRange(min, null, dbMin, dbMax);
         sut.Validate(x => x.MaxValueInDisplayUnit, valueInHours).IsEmpty.ShouldBeEqualTo(valid);
      }

      [TestCase(90, 2)]
      public void should_return_a_message_when_invalid_using_the_display_database_values(double? dbMax, double? valueInHours)
      {
         InitRange(null, null, null, dbMax);
         var formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);
         var validation = sut.Validate(x => x.MaxValueInDisplayUnit, valueInHours);
         validation.IsEmpty.ShouldBeEqualTo(false);
         validation.Message.ShouldBeEqualTo(PKSimConstants.Rules.Parameter.MaxLessThanDbMaxValue(sut.ParameterName, formatter.Format(sut.DbMaxValueInDisplayUnit.Value), sut.Unit.Name));
      }
   }
}