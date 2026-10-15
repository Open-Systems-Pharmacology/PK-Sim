using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Core.ExpressionQueryForSpecs;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionQueryCalculator : ContextSpecification<IExpressionQueryCalculator>
   {
      protected ExpressionQuery _query;
      protected IReadOnlyList<ExpressionContainerInfo> _containers;
      protected IReadOnlyList<UnitExpression> _result;

      protected override void Context()
      {
         sut = new ExpressionQueryCalculator();
         _containers = Containers();
         _query = Query();
      }

      protected override void Because()
      {
         _result = sut.UnitExpressionsFor(_query, _containers);
      }

      protected ContainerExpression ExpressionFor(string unit, string containerName) =>
         _result.First(x => x.Unit == unit).ContainerExpressions.First(x => x.Container.ContainerName == containerName);

      protected void ShouldHaveExpression(string unit, string containerName, double expressionValue, double relativeExpression)
      {
         var containerExpression = ExpressionFor(unit, containerName);
         containerExpression.ExpressionValue.Value.ShouldBeEqualTo(expressionValue, 1e-12);
         containerExpression.RelativeExpression.Value.ShouldBeEqualTo(relativeExpression, 1e-12);
      }

      protected void ShouldHaveNoExpression(string unit, string containerName)
      {
         var containerExpression = ExpressionFor(unit, containerName);
         containerExpression.ExpressionValue.ShouldBeNull();
         containerExpression.RelativeExpression.ShouldBeNull();
      }
   }

   public class When_computing_the_unit_expressions_of_an_unfiltered_query : concern_for_ExpressionQueryCalculator
   {
      [Observation]
      public void should_return_one_unit_expression_per_unit_ordered_by_unit()
      {
         _result.Select(x => x.Unit).ShouldOnlyContainInOrder(ARRAY_EXPRESS, EST, RT_PCR);
      }

      [Observation]
      public void should_return_the_expression_of_all_containers_in_the_order_of_the_containers()
      {
         _result.Each(x => x.ContainerExpressions.Select(c => c.Container).ShouldOnlyContainInOrder(_containers));
      }

      [Observation]
      public void should_use_the_mean_of_the_container_records_and_not_the_mean_of_the_database_means()
      {
         ShouldHaveExpression(EST, "Liver", 4, 4.0 / 6);
      }

      [Observation]
      public void should_ignore_records_without_value()
      {
         ShouldHaveExpression(EST, "Kidney", 3, 0.5);
      }

      [Observation]
      public void should_assign_the_records_of_a_tissue_to_all_containers_the_tissue_is_mapped_to()
      {
         ShouldHaveExpression(EST, "Duodenum", 6, 1);
         ShouldHaveExpression(EST, "Jejunum", 6, 1);
      }

      [Observation]
      public void should_compute_the_relative_expression_with_respect_to_the_highest_expression_of_the_unit()
      {
         ShouldHaveExpression(RT_PCR, "Kidney", 5, 0.5);
         ShouldHaveExpression(RT_PCR, "Liver", 10, 1);
      }

      [Observation]
      public void should_set_the_relative_expression_to_zero_when_the_highest_expression_of_the_unit_is_zero()
      {
         ShouldHaveExpression(ARRAY_EXPRESS, "Liver", 0, 0);
         ShouldHaveExpression(ARRAY_EXPRESS, "Kidney", 0, 0);
      }

      [Observation]
      public void should_not_have_an_expression_for_containers_without_record()
      {
         ShouldHaveNoExpression(EST, "Brain");
         ShouldHaveNoExpression(RT_PCR, "Duodenum");
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_with_a_container_whose_records_have_no_value : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query = Query(Record("DB1", MALE, "brain", 20, 30, null, EST));
      }

      [Observation]
      public void should_not_have_an_expression_for_that_container()
      {
         ShouldHaveNoExpression(EST, "Brain");
         ShouldHaveExpression(EST, "Duodenum", 6, 1);
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_including_some_values_of_a_field : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query.Filter = FilterOn(ExpressionDataFields.GENDER, ExpressionDataFilterType.Included, showBlanks: false, MALE);
      }

      [Observation]
      public void should_only_use_the_records_with_those_values()
      {
         ShouldHaveExpression(EST, "Liver", 4.5, 0.75);
         ShouldHaveExpression(EST, "Kidney", 3, 0.5);
         ShouldHaveNoExpression(ARRAY_EXPRESS, "Kidney");
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_excluding_some_values_of_a_field_and_showing_blanks : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query = Query(Record("DB2", null, "liver", 20, 30, 20, EST));
         _query.Filter = FilterOn(ExpressionDataFields.GENDER, ExpressionDataFilterType.Excluded, showBlanks: true, FEMALE);
      }

      [Observation]
      public void should_use_the_records_without_those_values_including_the_records_without_value_for_the_field()
      {
         ShouldHaveExpression(EST, "Liver", 29.0 / 3, 1);
         ShouldHaveExpression(EST, "Duodenum", 6, 6 / (29.0 / 3));
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_excluding_some_values_of_a_field_and_hiding_blanks : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query = Query(Record("DB2", null, "liver", 20, 30, 20, EST));
         _query.Filter = FilterOn(ExpressionDataFields.GENDER, ExpressionDataFilterType.Excluded, showBlanks: false, FEMALE);
      }

      [Observation]
      public void should_not_use_the_records_without_value_for_the_field()
      {
         ShouldHaveExpression(EST, "Liver", 4.5, 0.75);
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_filtered_on_the_age : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query.Filter = FilterOn(ExpressionDataFields.AGE, ExpressionDataFilterType.Included, showBlanks: false, ExpressionDataRecord.AgeFrom(20, 30));
      }

      [Observation]
      public void should_only_use_the_records_in_that_age_range()
      {
         ShouldHaveExpression(EST, "Kidney", 4, 4.0 / 6);
         ShouldHaveExpression(EST, "Liver", 2, 2.0 / 6);
         ShouldHaveExpression(RT_PCR, "Kidney", 5, 1);
         ShouldHaveNoExpression(RT_PCR, "Liver");
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_filtered_on_the_container_display_name : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query.Filter = FilterOn(ExpressionDataFields.CONTAINER_DISPLAY_NAME, ExpressionDataFilterType.Included, showBlanks: false, "Liver D", "Kidney D");
      }

      [Observation]
      public void should_only_use_the_records_of_those_containers()
      {
         ShouldHaveExpression(EST, "Kidney", 3, 0.75);
         ShouldHaveExpression(EST, "Liver", 4, 1);
         ShouldHaveNoExpression(EST, "Duodenum");
      }
   }

   public class When_computing_the_unit_expressions_of_a_query_whose_filter_was_not_restored : concern_for_ExpressionQueryCalculator
   {
      protected override void Context()
      {
         base.Context();
         _query.Filter = null;
      }

      protected override void Because()
      {
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.UnitExpressionsFor(_query, _containers)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_retrieving_the_container_records_of_a_query : concern_for_ExpressionQueryCalculator
   {
      private IReadOnlyList<ContainerExpressionDataRecord> _containerRecords;

      protected override void Because()
      {
         _containerRecords = sut.ContainerRecordsFor(_query, _containers);
      }

      [Observation]
      public void should_assign_each_record_to_every_container_its_tissue_is_mapped_to()
      {
         _containerRecords.Where(x => x.Record.Tissue == "intestine").Select(x => x.Container.ContainerName).ShouldOnlyContain("Duodenum", "Jejunum");
      }

      [Observation]
      public void should_not_return_the_records_of_unmapped_tissues_or_of_tissues_mapped_to_other_containers()
      {
         _containerRecords.Select(x => x.Record.Tissue).Distinct().ShouldOnlyContain("liver", "kidney", "intestine");
      }
   }

   public class When_retrieving_the_age_of_an_expression_data_record : StaticContextSpecification
   {
      [Observation]
      public void should_return_the_age_range_when_minimum_and_maximum_ages_differ()
      {
         ExpressionDataRecord.AgeFrom(20, 30).ShouldBeEqualTo("20 - 30");
      }

      [Observation]
      public void should_return_the_age_when_minimum_and_maximum_ages_are_equal()
      {
         ExpressionDataRecord.AgeFrom(40, 40).ShouldBeEqualTo("40");
      }

      [Observation]
      public void should_return_unspecified_when_the_ages_are_not_defined()
      {
         new ExpressionDataRecord().Age.ShouldBeEqualTo(ExpressionDataRecord.UNSPECIFIED_AGE);
      }
   }

   public class When_describing_an_expression_data_filter : StaticContextSpecification
   {
      [Observation]
      public void should_describe_each_field_filter_with_its_handling_of_blanks()
      {
         new ExpressionDataFieldFilter("GENDER", ExpressionDataFilterType.Included, new object[] {"MALE"}, showBlanks: false).ToString().ShouldBeEqualTo("[GENDER] In ('MALE')");
         new ExpressionDataFieldFilter("GENDER", ExpressionDataFilterType.Included, new object[] {"MALE"}, showBlanks: true).ToString().ShouldBeEqualTo("[GENDER] In ('MALE') Or [GENDER] Is Null");
         new ExpressionDataFieldFilter("GENDER", ExpressionDataFilterType.Excluded, new object[] {"MALE", "FEMALE"}, showBlanks: true).ToString().ShouldBeEqualTo("Not [GENDER] In ('MALE', 'FEMALE') Or [GENDER] Is Null");
         new ExpressionDataFieldFilter("GENDER", ExpressionDataFilterType.Excluded, new object[] {"MALE"}, showBlanks: false).ToString().ShouldBeEqualTo("Not [GENDER] In ('MALE') And [GENDER] Is Not Null");
      }

      [Observation]
      public void should_combine_the_field_filters()
      {
         new ExpressionDataFilter(new[]
         {
            new ExpressionDataFieldFilter("GENDER", ExpressionDataFilterType.Included, new object[] {"MALE"}, showBlanks: false),
            new ExpressionDataFieldFilter("UNIT", ExpressionDataFilterType.Included, new object[] {"EST"}, showBlanks: false)
         }).ToString().ShouldBeEqualTo("([GENDER] In ('MALE')) And ([UNIT] In ('EST'))");
      }
   }
}
