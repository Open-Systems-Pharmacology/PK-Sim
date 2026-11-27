using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Core.ExpressionQueryForSpecs;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionQueryTask : ContextSpecification<IExpressionQueryTask>
   {
      protected IGeneExpressionQueries _geneExpressionQueries;
      protected IExpressionDataTableMapper _expressionDataTableMapper;
      protected IExpressionQuerySerializer _expressionQuerySerializer;
      protected IReadOnlyList<ExpressionContainerInfo> _containers;
      protected ExpressionQuery _query;

      protected override void Context()
      {
         _geneExpressionQueries = A.Fake<IGeneExpressionQueries>();
         _expressionDataTableMapper = new ExpressionDataTableMapper();
         _expressionQuerySerializer = new ExpressionQuerySerializer(_expressionDataTableMapper);
         sut = new ExpressionQueryTask(_geneExpressionQueries, _expressionDataTableMapper, new ExpressionQueryCalculator(), _expressionQuerySerializer);
         _containers = Containers();
         _query = Query();
      }
   }

   public class When_creating_the_expression_query_of_a_gene : concern_for_ExpressionQueryTask
   {
      private ExpressionQuery _result;

      protected override void Context()
      {
         base.Context();
         var dataSet = _expressionDataTableMapper.DataSetFrom(_query);
         A.CallTo(() => _geneExpressionQueries.GetExpressionDataByGeneId(5)).Returns(dataSet.Tables[ExpressionDataTableMapper.EXPRESSION_DATA].Copy());
         A.CallTo(() => _geneExpressionQueries.GetContainerTissueMapping()).Returns(_expressionDataTableMapper.MappingTableFrom(Mapping().Where(x => x.Tissue != "skin")));
      }

      protected override void Because()
      {
         _result = sut.CreateQueryFor(5, "CYP3A4");
      }

      [Observation]
      public void should_return_a_query_on_the_records_of_the_gene()
      {
         _result.ProteinName.ShouldBeEqualTo("CYP3A4");
         _result.Records.Count.ShouldBeEqualTo(_query.Records.Count);
      }

      [Observation]
      public void should_add_the_tissues_not_mapped_by_the_database_without_container_to_the_mapping()
      {
         _result.Mapping.Last().Tissue.ShouldBeEqualTo("skin");
         _result.Mapping.Last().Container.ShouldBeNull();
         _result.Mapping.Count(x => x.Tissue == "skin").ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_not_filter_the_records()
      {
         _result.Filter.FieldFilters.ShouldBeEmpty();
      }
   }

   public class When_computing_the_results_of_an_expression_query_in_a_selected_unit : concern_for_ExpressionQueryTask
   {
      private QueryExpressionResults _result;

      protected override void Context()
      {
         base.Context();
         _query.SelectedUnit = EST;
         _query.Filter = FilterOn(ExpressionDataFields.GENDER, ExpressionDataFilterType.Included, showBlanks: false, MALE);
      }

      protected override void Because()
      {
         _result = sut.ResultsFor(_query, _containers);
      }

      [Observation]
      public void should_return_the_relative_expression_of_every_container_in_that_unit()
      {
         _result.ExpressionResults.Select(x => x.ContainerName).ShouldOnlyContainInOrder("Liver", "Kidney", "Duodenum", "Jejunum", "Brain");
         _result.ExpressionResultFor("Liver").RelativeExpression.ShouldBeEqualTo(0.75, 1e-12);
         _result.ExpressionResultFor("Kidney").RelativeExpression.ShouldBeEqualTo(0.5, 1e-12);
         _result.ExpressionResultFor("Duodenum").RelativeExpression.ShouldBeEqualTo(1, 1e-12);
      }

      [Observation]
      public void should_set_the_relative_expression_of_containers_without_record_to_zero()
      {
         _result.ExpressionResultFor("Brain").RelativeExpression.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_return_the_protein_and_the_unit()
      {
         _result.ProteinName.ShouldBeEqualTo("CYP3A4");
         _result.SelectedUnit.ShouldBeEqualTo(EST);
      }

      [Observation]
      public void should_return_a_query_configuration_restoring_the_query()
      {
         var query = _expressionQuerySerializer.Deserialize(_result.QueryConfiguration);
         query.SelectedUnit.ShouldBeEqualTo(EST);
         query.Filter.ToString().ShouldBeEqualTo(_query.Filter.ToString());
         query.Records.Count.ShouldBeEqualTo(_query.Records.Count);
      }

      [Observation]
      public void should_describe_the_protein_unit_filter_and_mapped_tissues()
      {
         _result.Description.Contains("Selected protein: CYP3A4").ShouldBeTrue();
         _result.Description.Contains($"Selected unit: {EST}").ShouldBeTrue();
         _result.Description.Contains(_query.Filter.ToString()).ShouldBeTrue();
         _result.Description.Contains("Tissue [intestine] -> Container [Jejunum]").ShouldBeTrue();
         _result.Description.Contains("[skin]").ShouldBeFalse();
      }
   }

   public class When_computing_the_results_of_an_expression_query_without_selected_unit : concern_for_ExpressionQueryTask
   {
      private QueryExpressionResults _result;

      protected override void Because()
      {
         _result = sut.ResultsFor(_query, _containers);
      }

      [Observation]
      public void should_use_the_first_unit_with_data_and_select_it_in_the_query()
      {
         _result.SelectedUnit.ShouldBeEqualTo(ARRAY_EXPRESS);
         _query.SelectedUnit.ShouldBeEqualTo(ARRAY_EXPRESS);
      }
   }

   public class When_computing_the_results_of_an_expression_query_in_a_unit_without_data : concern_for_ExpressionQueryTask
   {
      protected override void Context()
      {
         base.Context();
         _query.SelectedUnit = "TPM";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ResultsFor(_query, _containers)).ShouldThrowAn<PKSimException>();
      }
   }
}
