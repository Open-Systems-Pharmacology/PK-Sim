using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisNumericField : ContextSpecification<PopulationAnalysisNumericField>
   {
      protected IDimension _dimension;
      protected Unit _displayUnit;
      protected double _displayValue;
      protected double _baseValue;
      protected string _parameterPath = "PATH";

      protected override void Context()
      {
         _dimension = A.Fake<IDimension>();
         _displayUnit = A.Fake<Unit>();
         _dimension.DefaultUnit = _displayUnit;
         _displayValue = 0.1;
         _baseValue = 1;
         sut = new PopulationAnalysisParameterField { Dimension = _dimension, DisplayUnit = _displayUnit, ParameterPath =_parameterPath};
      }
   }

   public class When_converting_a_value_in_base_unit_to_the_display_unit : concern_for_PopulationAnalysisNumericField
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_displayUnit, _baseValue)).Returns(_displayValue);
      }

      [Observation]
      public void should_return_the_converted_value()
      {
         sut.ValueInDisplayUnit(_baseValue).ShouldBeEqualTo(_displayValue);
      }
   }

   public class When_converting_a_value_in_display_unit_to_the_base_unit : concern_for_PopulationAnalysisNumericField
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dimension.UnitValueToBaseUnitValue(_displayUnit, _displayValue)).Returns(_baseValue);
      }

      [Observation]
      public void should_return_the_converted_value()
      {
         sut.ValueInCoreUnit(_displayValue).ShouldBeEqualTo(_baseValue);
      }
   }

   public class When_checking_if_a_population_analysis_numerical_field_can_be_used_for_grouping : concern_for_PopulationAnalysisNumericField
   {
      private List<double> _values;
      private IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         base.Context();
         _values=new List<double>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => _populationDataCollector.AllValuesFor(_parameterPath)).Returns(_values);
      }

      [Observation]
      public void should_return_true_if_the_values_have_distinct_min_and_max()
      {
         _values.AddRange(new []{1d,2d});
         sut.CanBeUsedForGroupingIn(_populationDataCollector).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_there_is_no_values_for_the_given_data_collector()
      {
         sut.CanBeUsedForGroupingIn(_populationDataCollector).ShouldBeFalse();

      }
      [Observation]
      public void should_return_false_if_the_values_are_constant()
      {
         _values.AddRange(new[] { 1d, 1d });
         sut.CanBeUsedForGroupingIn(_populationDataCollector).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_values_are_all_nan()
      {
         _values.AddRange(new[] { double.NaN, double.NaN });
         sut.CanBeUsedForGroupingIn(_populationDataCollector).ShouldBeFalse();
      }
   }
}