using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Services;
using static PKSim.Presentation.Presenters.ProteinExpression.DatabaseConfiguration;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_GeneExpressionQueries : ContextForIntegration<IGeneExpressionQueries>
   {
      private IApplicationSettings _applicationSettings;
      private IDisposable _databaseConnection;
      protected IGeneExpressionsDatabasePathManager _databasePathManager;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _applicationSettings = IoC.Resolve<IApplicationSettings>();
         _applicationSettings.AddSpeciesDatabaseMap(new SpeciesDatabaseMap { Species = CoreConstants.Species.CAT, DatabaseFullPath = GeneExpressionDatabaseForSpecs.CatDatabasePath });
         _databasePathManager = IoC.Resolve<IGeneExpressionsDatabasePathManager>();
         _databaseConnection = _databasePathManager.ConnectToDatabaseFor(CoreConstants.Species.CAT);
      }

      public override void GlobalCleanup()
      {
         _applicationSettings.RemoveSpeciesDatabaseMap(CoreConstants.Species.CAT);
         _databaseConnection?.Dispose();
         base.GlobalCleanup();
      }

      protected static IEnumerable<T> valuesIn<T>(DataTable table, string column) => table.Rows.Cast<DataRow>().Select(row => (T) row[column]);
   }

   public class When_connecting_to_a_released_gene_expression_database : concern_for_GeneExpressionQueries
   {
      [Observation]
      public void should_find_the_database_defined_for_the_species() => _databasePathManager.HasDatabaseFor(CoreConstants.Species.CAT).ShouldBeTrue();

      [Observation]
      public void should_accept_the_database_schema() => sut.ValidateDatabase();
   }

   public class When_searching_a_released_gene_expression_database_for_a_protein : concern_for_GeneExpressionQueries
   {
      private DataTable _proteins;

      protected override void Because() => _proteins = sut.GetProteinsByName("%CYP3A4%");

      [Observation]
      public void should_return_every_gene_with_a_matching_name() => valuesIn<long>(_proteins, ProteinColumns.COL_ID).ShouldOnlyContain(13959, 15850, 18956);

      [Observation]
      public void should_report_that_the_genes_have_expression_data() => valuesIn<long>(_proteins, ProteinColumns.HAS_DATA).Distinct().ShouldOnlyContain(1);

      [Observation]
      public void should_type_the_columns_used_to_select_a_protein()
      {
         _proteins.Columns[ProteinColumns.COL_ID].DataType.ShouldBeEqualTo(typeof(long));
         _proteins.Columns[ProteinColumns.HAS_DATA].DataType.ShouldBeEqualTo(typeof(long));
         _proteins.Columns[ProteinColumns.COL_GENE_NAME].DataType.ShouldBeEqualTo(typeof(string));
         _proteins.Columns[ProteinColumns.COL_NAME_TYPE].DataType.ShouldBeEqualTo(typeof(string));
      }
   }

   public class When_searching_for_a_protein_whose_first_match_has_no_symbol : concern_for_GeneExpressionQueries
   {
      private DataTable _proteins;

      protected override void Because() => _proteins = sut.GetProteinsByName("%C1R%");

      [Observation]
      public void should_start_with_a_gene_without_symbol_gene_id_or_full_name()
      {
         _proteins.Rows[0][ProteinColumns.COL_SYMBOL].ShouldBeEqualTo(DBNull.Value);
         _proteins.Rows[0][ProteinColumns.COL_GENE_ID].ShouldBeEqualTo(DBNull.Value);
         _proteins.Rows[0][ProteinColumns.COL_OFFICIAL_FULL_NAME].ShouldBeEqualTo(DBNull.Value);
      }

      [Observation]
      public void should_return_every_gene_with_a_matching_name() => valuesIn<long>(_proteins, ProteinColumns.COL_ID).ShouldOnlyContain(70, 72, 3496, 14412, 15507, 16204, 17705, 23980);

      [Observation]
      public void should_type_the_name_columns_as_text()
      {
         _proteins.Columns[ProteinColumns.COL_SYMBOL].DataType.ShouldBeEqualTo(typeof(string));
         _proteins.Columns[ProteinColumns.COL_GENE_ID].DataType.ShouldBeEqualTo(typeof(string));
         _proteins.Columns[ProteinColumns.COL_OFFICIAL_FULL_NAME].DataType.ShouldBeEqualTo(typeof(string));
      }
   }

   public class When_searching_for_a_protein_that_is_not_in_the_database : concern_for_GeneExpressionQueries
   {
      private DataTable _proteins;

      protected override void Because() => _proteins = sut.GetProteinsByName("%NOT_A_GENE%");

      [Observation]
      public void should_not_return_any_protein() => _proteins.Rows.Count.ShouldBeEqualTo(0);
   }

   public class When_retrieving_the_expression_data_of_a_gene : concern_for_GeneExpressionQueries
   {
      private DataTable _expressionData;
      private DataTable _containerTissueMapping;

      protected override void Because()
      {
         _expressionData = sut.GetExpressionDataByGeneId(18956);
         _containerTissueMapping = sut.GetContainerTissueMapping();
      }

      [Observation]
      public void should_return_the_columns_of_the_expression_data() =>
         _expressionData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ShouldOnlyContainInOrder(
            ExpressionDataColumns.COL_VARIANT_NAME, ExpressionDataColumns.COL_DATA_BASE, ExpressionDataColumns.COL_DATA_BASE_REC_ID,
            ExpressionDataColumns.COL_GENDER, ExpressionDataColumns.COL_TISSUE, ExpressionDataColumns.COL_HEALTH_STATE,
            ExpressionDataColumns.COL_SAMPLE_SOURCE, ExpressionDataColumns.COL_AGE_MIN, ExpressionDataColumns.COL_AGE_MAX,
            ExpressionDataColumns.COL_SAMPLE_COUNT, ExpressionDataColumns.COL_TOTAL_COUNT, ExpressionDataColumns.COL_RATIO,
            ExpressionDataColumns.COL_NORM_VALUE, ExpressionDataColumns.COL_UNIT);

      [Observation]
      public void should_return_one_row_per_sample_of_the_gene() => _expressionData.Rows.Count.ShouldBeEqualTo(8);

      [Observation]
      public void should_return_the_tissues_of_the_samples() => valuesIn<string>(_expressionData, ExpressionDataColumns.COL_TISSUE).Distinct().ShouldOnlyContain("ADULT MAMMALIAN KIDNEY", "LIVER", "TESTIS");

      [Observation]
      public void should_only_return_tpm_values() => valuesIn<string>(_expressionData, ExpressionDataColumns.COL_UNIT).Distinct().ShouldOnlyContain("TPM");

      [Observation]
      public void should_type_the_values_as_numbers()
      {
         _expressionData.Columns[ExpressionDataColumns.COL_SAMPLE_COUNT].DataType.ShouldBeEqualTo(typeof(double));
         _expressionData.Columns[ExpressionDataColumns.COL_TOTAL_COUNT].DataType.ShouldBeEqualTo(typeof(double));
         _expressionData.Columns[ExpressionDataColumns.COL_RATIO].DataType.ShouldBeEqualTo(typeof(double));
         _expressionData.Columns[ExpressionDataColumns.COL_NORM_VALUE].DataType.ShouldBeEqualTo(typeof(double));
      }

      [Observation]
      public void should_type_the_tissue_like_the_container_tissue_mapping() =>
         _expressionData.Columns[ExpressionDataColumns.COL_TISSUE].DataType.ShouldBeEqualTo(_containerTissueMapping.Columns[MappingColumns.COL_TISSUE].DataType);
   }

   public class When_retrieving_the_container_tissue_mapping_of_a_released_gene_expression_database : concern_for_GeneExpressionQueries
   {
      private DataTable _containerTissueMapping;

      protected override void Because() => _containerTissueMapping = sut.GetContainerTissueMapping();

      [Observation]
      public void should_return_every_mapping_of_the_database() => _containerTissueMapping.Rows.Count.ShouldBeEqualTo(485);

      [Observation]
      public void should_map_the_liver_to_both_liver_zones() =>
         _containerTissueMapping.Rows.Cast<DataRow>()
            .Where(row => Equals(row[MappingColumns.COL_TISSUE], "LIVER"))
            .Select(row => (string) row[MappingColumns.COL_CONTAINER])
            .ShouldOnlyContain("Pericentral", "Periportal");
   }

   public class When_retrieving_the_hints_of_a_released_gene_expression_database : concern_for_GeneExpressionQueries
   {
      [Observation]
      public void should_describe_the_gender() => sut.GetGenderHint("FEMALE").ShouldBeEqualTo("A FEMALE individual or population");

      [Observation]
      public void should_describe_the_tissue() => sut.GetTissueHint("LIVER").ShouldBeEqualTo("Organ");

      [Observation]
      public void should_describe_the_health_state() => sut.GetHealthStateHint("UNSPECIFIED NORMAL").ShouldBeEqualTo("This refers to from an UNSPECIFIED NORMAL individual.");

      [Observation]
      public void should_describe_the_sample_source() => sut.GetSampleSourceHint("TISSUE").ShouldBeEqualTo("The sample source is tissue.");

      [Observation]
      public void should_describe_the_unit() => sut.GetUnitHint("TPM").ShouldBeEqualTo("NGS data");

      [Observation]
      public void should_describe_the_name_type() => sut.GetNameTypeHint("SYMBOL").ShouldBeEqualTo("The identifier 'SYMBOL' is based on the Cran R biomaRt package.");

      [Observation]
      public void should_not_describe_an_unknown_value() => sut.GetGenderHint("UNKNOWN").ShouldBeEqualTo(string.Empty);
   }

   [IntegrationTests]
   public class When_checking_for_a_newer_release_of_the_gene_expression_databases : StaticContextSpecification
   {
      private string _latestRelease;

      protected override void Because() => _latestRelease = GeneExpressionDatabaseForSpecs.LatestRelease();

      [Observation]
      public void should_test_against_the_latest_release() =>
         _latestRelease.ShouldBeEqualTo(GeneExpressionDatabaseForSpecs.RELEASE, $"Gene expression databases {_latestRelease} have been released. Update {nameof(GeneExpressionDatabaseForSpecs)}.{nameof(GeneExpressionDatabaseForSpecs.RELEASE)} and the expectations of the gene expression database specs.");
   }
}
