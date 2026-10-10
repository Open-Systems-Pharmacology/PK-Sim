using System.Linq;
using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Core.ExpressionQueryForSpecs;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionQuerySerializer : ContextSpecification<IExpressionQuerySerializer>
   {
      protected ExpressionQuery _query;
      protected ExpressionQuery _result;

      protected const string LAYOUT_WITHOUT_FILTER = @"<XtraSerializer version=""1.0"" application=""PivotGrid"">
  <property name=""Prefilter"" isnull=""true"" iskey=""true"">
    <property name=""Criteria"" isnull=""true"" />
    <property name=""Enabled"">true</property>
  </property>
  <property name=""Fields"" iskey=""true"" value=""1"">
    <property name=""Item1"" isnull=""true"" iskey=""true"">
      <property name=""FilterValues"" isnull=""true"" iskey=""true"">
        <property name=""Values"">~Xtra#Array0, </property>
        <property name=""ValuesCore"" iskey=""true"" value=""0"" />
        <property name=""ShowBlanks"">true</property>
        <property name=""FilterType"">Excluded</property>
      </property>
      <property name=""FieldName"">GENDER</property>
    </property>
  </property>
</XtraSerializer>";

      protected const string LAYOUT_WITH_FIELD_FILTER = @"<XtraSerializer version=""1.0"" application=""PivotGrid"">
  <property name=""Fields"" iskey=""true"" value=""1"">
    <property name=""Item1"" isnull=""true"" iskey=""true"">
      <property name=""FilterValues"" isnull=""true"" iskey=""true"">
        <property name=""ValuesCore"" iskey=""true"" value=""1"">
          <property name=""Item1"" isnull=""true"" iskey=""true"">
            <property name=""Value"" type=""System.String"">MALE</property>
          </property>
        </property>
        <property name=""ShowBlanks"">false</property>
        <property name=""FilterType"">Included</property>
      </property>
      <property name=""FieldName"">GENDER</property>
    </property>
  </property>
</XtraSerializer>";

      protected const string LAYOUT_WITH_ACTIVE_FILTER = @"<XtraSerializer version=""1.0"" application=""PivotGrid"">
  <property name=""Prefilter"" isnull=""true"" iskey=""true"">
    <property name=""Enabled"">true</property>
  </property>
  <property name=""ActiveFilter"" isnull=""true"" iskey=""true"">
    <property name=""CriteriaString"">[NORM_VALUE] &gt; 5</property>
  </property>
</XtraSerializer>";

      protected override void Context()
      {
         sut = new ExpressionQuerySerializer(new ExpressionDataTableMapper());
         _query = Query();
         _query.SelectedUnit = EST;
         _query.LayoutSettings = LAYOUT_WITHOUT_FILTER;
         _query.Filter = new ExpressionDataFilter(new[]
         {
            new ExpressionDataFieldFilter(ExpressionDataFields.GENDER, ExpressionDataFilterType.Excluded, new object[] {FEMALE}, showBlanks: true),
            new ExpressionDataFieldFilter(ExpressionDataFields.AGE_MIN, ExpressionDataFilterType.Included, new object[] {20.0, 40.5}, showBlanks: false)
         });
      }

      protected string QueryConfigurationSavedBeforeTheFilter(string layoutSettings)
      {
         _query.LayoutSettings = layoutSettings;
         var element = XElement.Parse(sut.Serialize(_query));
         element.Element(CoreConstants.Serialization.Filter).Remove();
         return element.ToString(SaveOptions.DisableFormatting);
      }
   }

   public class When_restoring_a_saved_expression_query : concern_for_ExpressionQuerySerializer
   {
      protected override void Because()
      {
         _result = sut.Deserialize(sut.Serialize(_query));
      }

      [Observation]
      public void should_restore_the_protein_unit_and_layout()
      {
         _result.ProteinName.ShouldBeEqualTo(_query.ProteinName);
         _result.SelectedUnit.ShouldBeEqualTo(EST);
         XElement.Parse(_result.LayoutSettings).ToString().ShouldBeEqualTo(XElement.Parse(_query.LayoutSettings).ToString());
      }

      [Observation]
      public void should_restore_the_records()
      {
         _result.Records.Count.ShouldBeEqualTo(_query.Records.Count);
         var kidneyWithoutValue = _result.Records[4];
         kidneyWithoutValue.Tissue.ShouldBeEqualTo("kidney");
         kidneyWithoutValue.Gender.ShouldBeEqualTo(FEMALE);
         kidneyWithoutValue.AgeMin.ShouldBeNull();
         kidneyWithoutValue.NormValue.ShouldBeNull();
         _result.Records[2].NormValue.ShouldBeEqualTo(8.0);
         _result.Records[2].Age.ShouldBeEqualTo(_query.Records[2].Age);
         _result.Records.Select(x => x.DatabaseRecId).ShouldOnlyContainInOrder(_query.Records.Select(x => x.DatabaseRecId));
      }

      [Observation]
      public void should_restore_the_mapping_including_the_unmapped_tissues()
      {
         _result.Mapping.Select(x => $"{x.Tissue}->{x.Container}").ShouldOnlyContainInOrder(_query.Mapping.Select(x => $"{x.Tissue}->{x.Container}"));
         _result.Mapping.Last().Container.ShouldBeNull();
      }

      [Observation]
      public void should_restore_the_filter_with_typed_values()
      {
         _result.Filter.FieldFilters.Count.ShouldBeEqualTo(2);
         var genderFilter = _result.Filter.FieldFilters[0];
         genderFilter.FieldName.ShouldBeEqualTo(ExpressionDataFields.GENDER);
         genderFilter.FilterType.ShouldBeEqualTo(ExpressionDataFilterType.Excluded);
         genderFilter.ShowBlanks.ShouldBeTrue();
         genderFilter.Values.ShouldOnlyContain(FEMALE);
         var ageFilter = _result.Filter.FieldFilters[1];
         ageFilter.FilterType.ShouldBeEqualTo(ExpressionDataFilterType.Included);
         ageFilter.ShowBlanks.ShouldBeFalse();
         ageFilter.Values.ShouldOnlyContain(20.0, 40.5);
      }
   }

   public class When_restoring_an_expression_query_saved_before_the_filter_whose_layout_has_no_filter : concern_for_ExpressionQuerySerializer
   {
      protected override void Because()
      {
         _result = sut.Deserialize(QueryConfigurationSavedBeforeTheFilter(LAYOUT_WITHOUT_FILTER));
      }

      [Observation]
      public void should_restore_an_empty_filter()
      {
         _result.Filter.FieldFilters.ShouldBeEmpty();
      }
   }

   public class When_restoring_an_expression_query_saved_before_the_filter_whose_layout_has_field_filter_values : concern_for_ExpressionQuerySerializer
   {
      protected override void Because()
      {
         _result = sut.Deserialize(QueryConfigurationSavedBeforeTheFilter(LAYOUT_WITH_FIELD_FILTER));
      }

      [Observation]
      public void should_leave_the_filter_to_be_restored_by_the_view()
      {
         _result.Filter.ShouldBeNull();
      }
   }

   public class When_restoring_an_expression_query_saved_before_the_filter_whose_layout_has_filter_criteria : concern_for_ExpressionQuerySerializer
   {
      protected override void Because()
      {
         _result = sut.Deserialize(QueryConfigurationSavedBeforeTheFilter(LAYOUT_WITH_ACTIVE_FILTER));
      }

      [Observation]
      public void should_leave_the_filter_to_be_restored_by_the_view()
      {
         _result.Filter.ShouldBeNull();
      }
   }

   public class When_restoring_an_expression_query_saved_without_layout : concern_for_ExpressionQuerySerializer
   {
      protected override void Because()
      {
         _result = sut.Deserialize(QueryConfigurationSavedBeforeTheFilter(null));
      }

      [Observation]
      public void should_restore_an_empty_filter_and_no_layout()
      {
         _result.Filter.FieldFilters.ShouldBeEmpty();
         _result.LayoutSettings.ShouldBeNull();
      }
   }
}
