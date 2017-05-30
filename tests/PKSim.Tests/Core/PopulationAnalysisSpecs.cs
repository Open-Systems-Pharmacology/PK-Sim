using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysis : ContextSpecification<PopulationPivotAnalysis>
   {
      protected PopulationAnalysisPKParameterField _pkField1;
      protected PopulationAnalysisPKParameterField _pkField2;
      protected PopulationAnalysisParameterField _paraField1;
      protected PopulationAnalysisParameterField _paraField2;
      protected PopulationAnalysisDerivedField _derivedField1;

      protected override void Context()
      {
         sut = new PopulationPivotAnalysis();
         _pkField1 = new PopulationAnalysisPKParameterField {PKParameter = "P1"}.WithName("Field1");
         _pkField2 = new PopulationAnalysisPKParameterField {PKParameter = "P2"}.WithName("Field2");
         _paraField1 = new PopulationAnalysisParameterField {ParameterPath = "Path3"}.WithName("Field3");
         _paraField2 = new PopulationAnalysisParameterField {ParameterPath = "Path4"}.WithName("Field4");
         _derivedField1 = A.Fake<PopulationAnalysisDerivedField>().WithName("Field5");
         sut.Add(_paraField1);
         sut.Add(_paraField2);
         sut.Add(_pkField1);
         sut.Add(_pkField2);
         sut.Add(_derivedField1);
      }
   }

   public class When_retrieving_all_the_field_of_a_given_type : concern_for_PopulationAnalysis
   {
      [Observation]
      public void should_only_return_the_pk_parameters_when_asked_for_them()
      {
         sut.All<PopulationAnalysisPKParameterField>().ShouldOnlyContain(_pkField1, _pkField2);
      }

      [Observation]
      public void should_only_return_the_population_parameters_when_asked_for_them()
      {
         sut.All<PopulationAnalysisParameterField>().ShouldOnlyContain(_paraField1, _paraField2);
      }

      [Observation]
      public void should_only_return_the_derived_fields_when_asked_for_them()
      {
         sut.All<PopulationAnalysisDerivedField>().ShouldOnlyContain(_derivedField1);
      }
   }

   public class When_retrieving_all_the_field_of_a_given_type_with_derived_fields : concern_for_PopulationAnalysis
   {
      [Observation]
      public void should_only_return_the_pk_parameters_and_the_derived_fields_based_on_pk_parmaeters_if_any()
      {
         A.CallTo(() => _derivedField1.IsDerivedTypeFor(typeof (PopulationAnalysisPKParameterField))).Returns(true);
         sut.All(typeof(PopulationAnalysisPKParameterField), withDerived: true).ShouldOnlyContain(_pkField1, _pkField2, _derivedField1);

         A.CallTo(() => _derivedField1.IsDerivedTypeFor(typeof (PopulationAnalysisPKParameterField))).Returns(false);
         sut.All(typeof(PopulationAnalysisPKParameterField), withDerived: true).ShouldOnlyContain(_pkField1, _pkField2);
      }

      [Observation]
      public void should_only_return_the_population_parameters_and_the_derived_fields_based_on_population_parmaeters_if_any()
      {
         A.CallTo(() => _derivedField1.IsDerivedTypeFor(typeof (PopulationAnalysisParameterField))).Returns(true);
         sut.All(typeof(PopulationAnalysisParameterField), withDerived: true).ShouldOnlyContain(_paraField1, _paraField2, _derivedField1);

         A.CallTo(() => _derivedField1.IsDerivedTypeFor(typeof (PopulationAnalysisParameterField))).Returns(false);
         sut.All(typeof(PopulationAnalysisParameterField), withDerived: true).ShouldOnlyContain(_paraField1, _paraField2);
      }

      [Observation]
      public void should_only_return_the_derived_fields_when_asked_for_them()
      {
         sut.All<PopulationAnalysisDerivedField>().ShouldOnlyContain(_derivedField1);
      }
   }

   public class When_setting_the_pivot_area_for_a_field_that_was_not_defined : concern_for_PopulationAnalysis
   {
      protected override void Because()
      {
         sut.SetPosition("XX", PivotArea.DataArea,1);
      }

      [Observation]
      public void should_not_add_any_area_to_the_analysis()
      {
         sut.AllFieldPositions.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_getting_the_pivot_area_for_a_field_that_was_not_defined : concern_for_PopulationAnalysis
   {
      [Observation]
      public void should_return_the_default_area()
      {
         sut.GetArea("XX").ShouldBeEqualTo(PivotArea.FilterArea);
      }
   }

   public class When_setting_the_pivot_area_for_an_existing_field_by_name : concern_for_PopulationAnalysis
   {
      protected override void Because()
      {
         sut.SetPosition(_paraField1.Name, PivotArea.DataArea,1);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_field_area_for_the_field_by_name()
      {
         sut.GetArea(_paraField1.Name).ShouldBeEqualTo(PivotArea.DataArea);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_field_area_for_the_field()
      {
         sut.GetArea(_paraField1).ShouldBeEqualTo(PivotArea.DataArea);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_area_index_for_the_field()
      {
         sut.GetAreaIndex(_paraField1).ShouldBeEqualTo(1);
      }
   }

   public class When_setting_the_pivot_area_for_an_existing_field : concern_for_PopulationAnalysis
   {
      protected override void Because()
      {
         sut.SetPosition(_paraField1, PivotArea.DataArea,1);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_field_area_for_the_field_by_name()
      {
         sut.GetArea(_paraField1.Name).ShouldBeEqualTo(PivotArea.DataArea);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_field_area_for_the_field()
      {
         sut.GetArea(_paraField1).ShouldBeEqualTo(PivotArea.DataArea);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_area_index_for_the_field()
      {
         sut.GetAreaIndex(_paraField1).ShouldBeEqualTo(1);
      }
   }

   public class When_sorting_fields : concern_for_PopulationAnalysis
   {
      protected override void Because()
      {
         sut.SetPosition(_paraField1, PivotArea.DataArea, 1);
         sut.SetPosition(_paraField2, PivotArea.DataArea, 2);
      }

      [Observation]
      public void should_compare_on_position()
      {
         sut.Compare(_paraField1.Name, _paraField2.Name).ShouldBeEqualTo(sut.GetAreaIndex(_paraField1.Name).CompareTo(sut.GetAreaIndex(_paraField2)));
      }
   }

   public class When_removing_a_field : concern_for_PopulationAnalysis
   {
      protected override void Context()
      {
         base.Context();
         sut.SetPosition(_paraField1, PivotArea.DataArea,1);
      }

      protected override void Because()
      {
         sut.Remove(_paraField1);
      }

      [Observation]
      public void should_have_removed_the_reference_to_the_field()
      {
         sut.AllFieldPositions.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_removing_a_field_that_was_not_added_to_area : concern_for_PopulationAnalysis
   {
      protected override void Because()
      {
         sut.Remove(_paraField1);
      }

      [Observation]
      public void should_have_removed_the_reference_to_the_field()
      {
         sut.AllFieldPositions.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_renaming_a_field_used_in_a_population_analyses : concern_for_PopulationAnalysis
   {
      private PopulationAnalysisGroupingField _groupingFieldUsing1;
      private PopulationAnalysisGroupingField _groupingFieldNotUsing;
      private PopulationAnalysisGroupingField _groupingFieldUsing2;
      private IPopulationAnalysisField _renamedField;
      private PopulationAnalysisExpressionField _expressionField;
      private const string _newName = "NEW";
      private const string _oldName = "OLD";

      protected override void Context()
      {
         _renamedField = new PopulationAnalysisParameterField {Name = _oldName, ParameterPath = "Path"};
         _groupingFieldUsing1 = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_oldName) { NumberOfBins = 5 }) { Name = "Using1" };
         _groupingFieldUsing2 = new PopulationAnalysisGroupingField(new FixedLimitsGroupingDefinition(_oldName) ) { Name = "Using2"};
         _groupingFieldNotUsing = new PopulationAnalysisGroupingField(new FixedLimitsGroupingDefinition("ANOTHER NAME")) {Name = "NotUsing"};
         _expressionField = new PopulationAnalysisExpressionField($"[{_oldName}] + [Field2]") {Name = "Expression"};
         sut = new PopulationPivotAnalysis();
         sut.Add(_renamedField);
         sut.Add(_groupingFieldUsing1);
         sut.Add(_groupingFieldNotUsing);
         sut.Add(_groupingFieldUsing2);
         sut.Add(_expressionField);
      }

      protected override void Because()
      {
         sut.RenameField(_oldName,_newName);
      }

      [Observation]
      public void should_rename_the_field_itself()
      {
         _renamedField.Name.ShouldBeEqualTo(_newName);
      }

      [Observation]
      public void should_update_the_grouping_definition_of_derived_fields_that_were_using_that_field()
      {
         _groupingFieldUsing1.GroupingDefinition.FieldName.ShouldBeEqualTo(_newName);
         _groupingFieldUsing2.GroupingDefinition.FieldName.ShouldBeEqualTo(_newName);
      }

      [Observation]
      public void should_have_let_the_grouping_definition_of_derived_fields_not_using_that_field_untouched()
      {
         _groupingFieldNotUsing.GroupingDefinition.FieldName.ShouldBeEqualTo("ANOTHER NAME");
      }

      [Observation]
      public void should_update_the_expression_fields()
      {
         _expressionField.Expression.Contains(_newName).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_fields_used_by_a_grouping_field : concern_for_PopulationAnalysis
   {
      private IReadOnlyCollection<IPopulationAnalysisField> _usedFields;
      private PopulationAnalysisDerivedField _derivedField;

      protected override void Context()
      {
         base.Context();
         _derivedField = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_pkField1.Name) { NumberOfBins = 5 });
         sut.Add(_derivedField);
      }

      protected override void Because()
      {
         _usedFields = sut.AllFieldsReferencedBy(_derivedField);
      }

      [Observation]
      public void should_return_the_derived_field_and_the_field_used_in_the_grouping_field()
      {
         _usedFields.ShouldContain(_pkField1);
      }
   }

   public class When_retrieving_the_fields_referencing_a_given_field : concern_for_PopulationAnalysis
   {
      private IReadOnlyCollection<IPopulationAnalysisField> _usedFields;
      private PopulationAnalysisDerivedField _derivedField;
      private PopulationAnalysisDerivedField _derivedFiel2;

      protected override void Context()
      {
         base.Context();
         _derivedField = new PopulationAnalysisExpressionField(string.Format("[{0}] + [{1}]", _pkField1.Name, _pkField2.Name));
         _derivedFiel2 = new PopulationAnalysisExpressionField(string.Format("[{0}]", _pkField2.Name));
         sut.Add(_derivedField);
         sut.Add(_derivedFiel2);

      }

      protected override void Because()
      {
         _usedFields = sut.AllFieldsReferencing(_pkField1);
      }

      [Observation]
      public void should_return_the_derived_fields_using_this_field()
      {
         _usedFields.ShouldOnlyContain(_derivedField);
      }
   }

   public class When_retrieving_the_fields_used_by_an_expression_field : concern_for_PopulationAnalysis
   {
      private IReadOnlyCollection<IPopulationAnalysisField> _usedFields;
      private PopulationAnalysisDerivedField _derivedField;

      protected override void Context()
      {
         base.Context();
         _derivedField = new PopulationAnalysisExpressionField(string.Format("[{0}] + [{1}]", _pkField1.Name, _pkField2.Name));
         sut.Add(_derivedField);

      }

      protected override void Because()
      {
         _usedFields = sut.AllFieldsReferencedBy(_derivedField);
      }

      [Observation]
      public void should_return_the_derived_field_and_the_field_used_in_the_expression()
      {
         _usedFields.ShouldContain(_pkField1,_pkField2);
      }
   }

   public class When_retrieving_the_fields_used_by_an_expression_using_derived_fields : concern_for_PopulationAnalysis
   {
      private IReadOnlyCollection<IPopulationAnalysisField> _usedFields;
      private PopulationAnalysisDerivedField _derivedField;
      private PopulationAnalysisDerivedField _expression1;
      private PopulationAnalysisDerivedField _subDerivedField;

      protected override void Context()
      {
         base.Context();
         _subDerivedField = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_pkField1.Name) { NumberOfBins = 5 }) { Name = "SubDerivedField" };
         _expression1 = new PopulationAnalysisExpressionField(string.Format("[{0}] + [{1}] +  [{2}]", _pkField1.Name, _pkField2.Name, _paraField1.Name)) { Name = "SubExpression" };
         _derivedField = new PopulationAnalysisExpressionField(string.Format("[{0}] + [{1}]", _subDerivedField.Name, _expression1.Name));
         sut.Add(_expression1);
         sut.Add(_expression1);
         sut.Add(_subDerivedField);

      }

      protected override void Because()
      {
         _usedFields = sut.AllFieldsReferencedBy(_derivedField);
      }

      [Observation]
      public void should_return_the_derived_field_and_all_other_fields_used_recursively()
      {
         _usedFields.ShouldContain(_pkField1, _pkField2,_paraField1,_expression1,_subDerivedField);
      }
   }

   public class When_retreving_all_fields_on_a_given_area : concern_for_PopulationAnalysis
   {
      protected override void Context()
      {
         base.Context();
         sut.SetPosition(_paraField1, PivotArea.DataArea,1);
         sut.SetPosition(_paraField2, PivotArea.DataArea,0);
         sut.SetPosition(_pkField1, PivotArea.RowArea,0);
      }

      [Observation]
      public void should_only_return_the_fields_defined_on_the_area_ordered_by_their_index()
      {
         sut.AllFieldsOn(PivotArea.DataArea).ShouldOnlyContainInOrder(_paraField2, _paraField1);
      }
   }

   public class When_retreving_all_fields_name_on_a_given_area : concern_for_PopulationAnalysis
   {
      protected override void Context()
      {
         base.Context();
         sut.SetPosition(_paraField1, PivotArea.DataArea, 1);
         sut.SetPosition(_paraField2, PivotArea.DataArea, 0);
         sut.SetPosition(_pkField1, PivotArea.RowArea, 0);
      }

      [Observation]
      public void should_only_return_the_name_of_fields_defined_on_the_area_ordered_by_their_index()
      {
         sut.AllFieldNamesOn(PivotArea.DataArea).ShouldOnlyContainInOrder(_paraField2.Name, _paraField1.Name);
      }
   }

   public class When_retreving_all_fields_on_a_given_area_and_of_a_given_type : concern_for_PopulationAnalysis
   {
      protected override void Context()
      {
         base.Context();
         sut.SetPosition(_paraField1, PivotArea.DataArea, 1);
         sut.SetPosition(_paraField2, PivotArea.DataArea, 0);
         sut.SetPosition(_pkField1, PivotArea.DataArea, 2);
      }

      [Observation]
      public void should_only_return_the_fields_defined_on_the_area_ordered_by_their_index()
      {
         sut.AllFieldsOn<PopulationAnalysisParameterField>(PivotArea.DataArea).ShouldOnlyContainInOrder(_paraField2, _paraField1);
      }
   }
}