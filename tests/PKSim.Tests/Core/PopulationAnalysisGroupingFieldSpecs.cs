using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisGroupingField : ContextSpecification<PopulationAnalysisGroupingField>
   {
      protected FixedLimitsGroupingDefinition _fixedLimitsGroupingDefinition;

      protected const string _fieldName = "Field";
      protected const string _sutName = "AgeClass";
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationAnalysis _populationAnalysis;
      protected PopulationAnalysisNumericField _numericField;
      protected DataTable _dt;

      private DataTable createData()
      {
         var dt = new DataTable();
         dt.Columns.Add(_fieldName, typeof (double));
         dt.BeginLoadData();
         for (var i = 0; i < 100; i++)
         {
            var newRow = dt.NewRow();
            newRow[_fieldName] = i;
            dt.Rows.Add(newRow);
         }
         dt.AcceptChanges();
         dt.EndLoadData();
         return dt;
      }

      protected override void Context()
      {
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysis = A.Fake<PopulationAnalysis>();
         A.CallTo(() => _populationDataCollector.NumberOfItems).Returns(100);
         _numericField = A.Fake<PopulationAnalysisNumericField>();

         A.CallTo(() => _populationAnalysis.FieldByName(_fieldName)).Returns(_numericField);
         _fixedLimitsGroupingDefinition = new FixedLimitsGroupingDefinition(_fieldName);
         _fixedLimitsGroupingDefinition.AddItems(PopulationAnalysisHelperForSpecs.AgeGroups);
         _fixedLimitsGroupingDefinition.SetLimits((new[] {14D, 18D}).OrderBy(x => x));

         _dt = createData();
         A.CallTo(() => _numericField.GetValues(_populationDataCollector)).Returns(_dt.AllValuesInColumn<double>(_fieldName).ToList());
         sut = new PopulationAnalysisGroupingField(_fixedLimitsGroupingDefinition) {Name = _sutName};
      }
   }

   public class creating_a_population_analysis_derived_field_with_fixed_limits_grouping : concern_for_PopulationAnalysisGroupingField
   {
      private string _expectedResultForFixedLimitsGroupingDefinition;

      protected override void Context()
      {
         base.Context();
         _expectedResultForFixedLimitsGroupingDefinition = initExpectedResultForFixedLimitsGroupingDefinition();
      }

      private string initExpectedResultForFixedLimitsGroupingDefinition()
      {
         return string.Format("iif([{0}] < 14, 'Children', iif([{0}] < 18, 'Adolescents', 'Adults'))", _fieldName);
      }

      [Observation]
      public void the_expression_should_be_generated()
      {
         sut.Expression.ShouldBeEqualTo(_expectedResultForFixedLimitsGroupingDefinition);
      }

      [Observation]
      public void the_expression_should_be_set_as_generated()
      {
         sut.Expression.ShouldBeEqualTo(_expectedResultForFixedLimitsGroupingDefinition);
      }

      [Observation]
      public void should_sort_values_by_labels()
      {
         var labels = _fixedLimitsGroupingDefinition.Labels;
         sut.Compare(labels[0], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(0, 1));
         sut.Compare(labels[0], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(0, 2));
         sut.Compare(labels[1], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(1, 0));
         sut.Compare(labels[1], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(1, 2));
         sut.Compare(labels[2], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(2, 0));
         sut.Compare(labels[2], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(2, 1));
      }
   }

   public class creating_a_population_analysis_derived_field_with_fixed_limits_grouping_and_reference_grouping_item : concern_for_PopulationAnalysisGroupingField
   {
      private string _expectedResultForFixedLimitsGroupingDefinition;
      private const string _referenceLabel = "Reference";

      protected override void Context()
      {
         base.Context();
         _expectedResultForFixedLimitsGroupingDefinition = initExpectedResultForFixedLimitsGroupingDefinition();
         sut.ReferenceGroupingItem = new GroupingItem() {Color = Color.Black, Label = _referenceLabel, Symbol = Symbols.Circle};
      }

      private string initExpectedResultForFixedLimitsGroupingDefinition()
      {
         return string.Format("iif([{0}] < 14, 'Children', iif([{0}] < 18, 'Adolescents', 'Adults'))", _fieldName);
      }

      [Observation]
      public void the_expression_should_be_generated()
      {
         sut.Expression.ShouldBeEqualTo(_expectedResultForFixedLimitsGroupingDefinition);
      }

      [Observation]
      public void the_expression_should_be_set_as_generated()
      {
         sut.Expression.ShouldBeEqualTo(_expectedResultForFixedLimitsGroupingDefinition);
      }

      [Observation]
      public void the_grouping_items_should_be_set_correctly()
      {
         var labels = _fixedLimitsGroupingDefinition.Labels;

         sut.GroupingItems.Count().ShouldBeEqualTo(4);
         sut.GroupingByName(labels[0]).ShouldNotBeNull();
         sut.GroupingByName(labels[1]).ShouldNotBeNull();
         sut.GroupingByName(labels[2]).ShouldNotBeNull();
         sut.GroupingByName(_referenceLabel).ShouldNotBeNull();
      }

      [Observation]
      public void should_sort_values_by_labels_and_reference_is_last()
      {
         var labels = _fixedLimitsGroupingDefinition.Labels;
         sut.Compare(labels[0], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(0, 1));
         sut.Compare(labels[0], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(0, 2));
         sut.Compare(labels[1], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(1, 0));
         sut.Compare(labels[1], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(1, 2));
         sut.Compare(labels[2], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(2, 0));
         sut.Compare(labels[2], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(2, 1));

         //with reference
         sut.Compare(labels[0], _referenceLabel).ShouldBeEqualTo(-1);
         sut.Compare(labels[1], _referenceLabel).ShouldBeEqualTo(-1);
         sut.Compare(labels[2], _referenceLabel).ShouldBeEqualTo(-1);
         sut.Compare(_referenceLabel, _referenceLabel).ShouldBeEqualTo(0);
         sut.Compare(_referenceLabel, labels[0]).ShouldBeEqualTo(1);
         sut.Compare(_referenceLabel, labels[1]).ShouldBeEqualTo(1);
         sut.Compare(_referenceLabel, labels[2]).ShouldBeEqualTo(1);
      }
   }

   public class creating_a_population_analysis_derived_field_with_value_mapping_grouping : concern_for_PopulationAnalysisGroupingField
   {
      private string _expectedResultForValueMappingGroupingDefinition;
      private ValueMappingGroupingDefinition _valueGroupingDefinition;
      private GroupingItem _grouping1;
      private GroupingItem _grouping2;

      protected override void Context()
      {
         base.Context();
         _expectedResultForValueMappingGroupingDefinition = initExpectedResultForValueMappingGroupingDefinition();
         _valueGroupingDefinition = new ValueMappingGroupingDefinition(_fieldName);
         _grouping1 = new GroupingItem {Label = "Label1"};
         _grouping2 = new GroupingItem {Label = "Label2"};
         _valueGroupingDefinition.AddValueLabel("1", _grouping1);
         _valueGroupingDefinition.AddValueLabel("2", _grouping2);
         sut = new PopulationAnalysisGroupingField(_valueGroupingDefinition) { Name = _sutName };
      }

      private string initExpectedResultForValueMappingGroupingDefinition()
      {
         return string.Format("iif([{0}] = '1', 'Label1', iif([{0}] = '2', 'Label2', 'Unknown'))", _fieldName);
      }


      [Observation]
      public void the_expression_should_be_generated()
      {
         sut.Expression.ShouldBeEqualTo(_expectedResultForValueMappingGroupingDefinition);
      }

      [Observation]
      public void the_expression_should_be_set_as_generated()
      {
         sut.Expression.ShouldBeEqualTo(_expectedResultForValueMappingGroupingDefinition);
      }

      [Observation]
      public void the_grouping_items_should_be_set_correctly()
      {
         sut.GroupingItems.Count().ShouldBeEqualTo(2);
         sut.GroupingByName("Label1").ShouldNotBeNull();
         sut.GroupingByName("Label2").ShouldNotBeNull();
      }
   
      [Observation]
      public void should_sort_values_according_to_sequence_if_value_mapping_is_used()
      {
         var mapping = _valueGroupingDefinition.Labels.ToList();
         var labels = _fixedLimitsGroupingDefinition.Labels;
         var index0 = mapping.IndexOf(labels[0]);
         var index1 = mapping.IndexOf(labels[1]);
         var index2 = mapping.IndexOf(labels[2]);
         sut.Compare(labels[0], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(index0, index1));
         sut.Compare(labels[0], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(index0, index2));
         sut.Compare(labels[1], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(index1, index0));
         sut.Compare(labels[1], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(index1, index2));
         sut.Compare(labels[2], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(index2, index0));
         sut.Compare(labels[2], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(index2, index1));
      }

      [Observation]
      public void should_sort_values_according_to_sequence_if_value_mapping_is_used_and_reference_grouping_item_set()
      {
         const string referenceLabel = "Reference";
         sut.ReferenceGroupingItem = new GroupingItem { Label = referenceLabel };
         var mapping = _valueGroupingDefinition.Labels.ToList();
         var labels = _fixedLimitsGroupingDefinition.Labels;
         var index0 = mapping.IndexOf(labels[0]);
         var index1 = mapping.IndexOf(labels[1]);
         var index2 = mapping.IndexOf(labels[2]);
         sut.Compare(labels[0], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(index0, index1));
         sut.Compare(labels[0], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(index0, index2));
         sut.Compare(labels[1], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(index1, index0));
         sut.Compare(labels[1], labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(index1, index2));
         sut.Compare(labels[2], labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(index2, index0));
         sut.Compare(labels[2], labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(index2, index1));
         //with reference
         sut.Compare(labels[0], referenceLabel).ShouldBeEqualTo(-1);
         sut.Compare(labels[1], referenceLabel).ShouldBeEqualTo(-1);
         sut.Compare(labels[2], referenceLabel).ShouldBeEqualTo(-1);
         sut.Compare(referenceLabel, referenceLabel).ShouldBeEqualTo(0);
         sut.Compare(referenceLabel, labels[0]).ShouldBeEqualTo(1);
         sut.Compare(referenceLabel, labels[1]).ShouldBeEqualTo(1);
         sut.Compare(referenceLabel, labels[2]).ShouldBeEqualTo(1);
         sut.ReferenceGroupingItem = null;
      }

      [Observation]
      public void should_set_grouping_items_if_value_mapping_is_used_and_reference_grouping_item_set()
      {
         const string referenceLabel = "Reference";
         sut.ReferenceGroupingItem = new GroupingItem { Label = referenceLabel };

         sut.GroupingItems.Count().ShouldBeEqualTo(3);
         sut.GroupingByName("Label1").ShouldNotBeNull();
         sut.GroupingByName("Label2").ShouldNotBeNull();
         sut.GroupingByName(referenceLabel).ShouldNotBeNull();
         
         sut.ReferenceGroupingItem = null;
      }
   }

   public class When_checking_if_a_derived_type_is_a_derived_type_for_a_given_type : concern_for_PopulationAnalysisGroupingField
   {
      private PopulationAnalysis _populationAnalyses;

      [Observation]
      public void should_return_false_if_the_grouping_definition_has_no_fields()
      {
         _populationAnalyses = A.Fake<PopulationAnalysis>();
         sut = new PopulationAnalysisGroupingField(new FixedLimitsGroupingDefinition(null)) {Name = _sutName, PopulationAnalysis = _populationAnalyses};
         sut.IsDerivedTypeFor<PopulationAnalysisParameterField>().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_grouping_expression_is_using_a_field_of_the_given_type()
      {
         sut.PopulationAnalysis = A.Fake<PopulationAnalysis>();
         A.CallTo(() => sut.PopulationAnalysis.FieldByName(_fieldName)).Returns(new PopulationAnalysisParameterField());
         sut.IsDerivedTypeFor<PopulationAnalysisParameterField>().ShouldBeTrue();
      }
   }

   public class When_checking_if_a_derived_type_is_a_derived_type_for_a_given_type_and_the_grouping_information_is_also_using_a_derived_type : concern_for_PopulationAnalysisGroupingField
   {
      private PopulationAnalysisGroupingField _derivedField;
      private PopulationAnalysis _populationAnalyses;

      protected override void Context()
      {
         base.Context();
         _populationAnalyses = A.Fake<PopulationAnalysis>();
         _derivedField = A.Fake<PopulationAnalysisGroupingField>();
         A.CallTo(() => _populationAnalyses.FieldByName(_fieldName)).Returns(_derivedField);
         sut.PopulationAnalysis = _populationAnalyses;
      }

      [Observation]
      public void should_return_true_if_the_derived_field_as_the_accurate_type()
      {
         A.CallTo(() => _derivedField.IsDerivedTypeFor(typeof (PopulationAnalysisParameterField))).Returns(true);
         sut.IsDerivedTypeFor<PopulationAnalysisParameterField>().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_derived_field_does_not_have_the_accurate_type()
      {
         A.CallTo(() => _derivedField.IsDerivedTypeFor<PopulationAnalysisParameterField>()).Returns(false);
         sut.IsDerivedTypeFor<PopulationAnalysisParameterField>().ShouldBeFalse();
      }
   }

   public class When_parsing_an_expression : concern_for_PopulationAnalysisGroupingField
   {
      [Observation]
      public void should_find_fields_in_expression()
      {
         var testExpression = "[Field1] + [Field2]";
         var test = new PopulationAnalysisExpressionField(testExpression);
         test.FindFieldsIn(testExpression).ShouldContain("Field1");
         test.FindFieldsIn(testExpression).ShouldContain("Field2");
      }
   }
}