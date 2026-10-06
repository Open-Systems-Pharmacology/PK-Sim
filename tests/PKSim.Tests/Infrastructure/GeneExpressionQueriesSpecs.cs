using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.DAS;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_GeneExpressionQueries : ContextSpecification<GeneExpressionQueries>
   {
      private string _databaseFile;
      private GeneExpressionDatabase _database;

      protected override void Context()
      {
         _databaseFile = FileHelper.GenerateTemporaryFileName();
         var das = new DAS();
         das.Connect(_databaseFile, string.Empty, string.Empty, DataProviders.SQLite);
         das.ExecuteSQL("CREATE TABLE tab_expression_data_records (data_source_id bigint, data_base text, data_base_rec_id text, last_refresh_date text)");
         das.ExecuteSQL("CREATE TABLE tab_expression_data_properties (data_source_id bigint, property text, property_value text)");
         das.ExecuteSQL("CREATE TABLE tab_expression_data_bases (data_base text, url text)");
         das.ExecuteSQL("INSERT INTO tab_expression_data_records VALUES (1, 'RNAseq', 'ERX1403298_TPM', '2023-08-30'), (2, 'E-GEOD-2361', 'TP_Adrenal_U133A', '2009-10-15T00:00:00')");
         das.ExecuteSQL("INSERT INTO tab_expression_data_properties VALUES (1, 'DATA_BASE', 'ERP014610'), (1, 'TISSUE', 'LIVER'), (2, 'RACE', 'Caucasians')");
         das.ExecuteSQL("INSERT INTO tab_expression_data_bases VALUES ('ERP014610', 'https://www.ncbi.nlm.nih.gov/sra/ERP014610'), ('E-GEOD-2361', 'http://www.ebi.ac.uk/arrayexpress/experiments/E-GEOD-2361')");
         das.DisConnect();

         _database = new GeneExpressionDatabase();
         _database.Connect(_databaseFile);
         sut = new GeneExpressionQueries(_database);
      }

      public override void Cleanup()
      {
         _database.Disconnect();
         FileHelper.DeleteFile(_databaseFile);
         base.Cleanup();
      }
   }

   public class When_retrieving_the_information_of_a_database_record_whose_study_is_one_of_its_properties : concern_for_GeneExpressionQueries
   {
      private string[] _information;

      protected override void Because() => _information = sut.GetDataBaseRecInfos("RNAseq", "ERX1403298_TPM");

      [Observation]
      public void should_return_the_refresh_date_and_the_url_of_the_study() =>
         _information.ShouldOnlyContainInOrder("LAST_REFRESH_DATE: 2023-08-30", "DB_URL: https://www.ncbi.nlm.nih.gov/sra/ERP014610");
   }

   public class When_retrieving_the_information_of_a_database_record_whose_database_has_a_url : concern_for_GeneExpressionQueries
   {
      private string[] _information;

      protected override void Because() => _information = sut.GetDataBaseRecInfos("E-GEOD-2361", "TP_Adrenal_U133A");

      [Observation]
      public void should_return_the_refresh_date_and_the_url_of_the_database() =>
         _information.ShouldOnlyContainInOrder("LAST_REFRESH_DATE: 2009-10-15T00:00:00", "DB_URL: http://www.ebi.ac.uk/arrayexpress/experiments/E-GEOD-2361");
   }

   public class When_retrieving_the_properties_of_a_database_record : concern_for_GeneExpressionQueries
   {
      private string[] _properties;

      protected override void Because() => _properties = sut.GetDataBaseRecProperties("RNAseq", "ERX1403298_TPM");

      [Observation]
      public void should_return_the_properties_of_the_record() => _properties.ShouldOnlyContain("DATA_BASE: ERP014610", "TISSUE: LIVER");
   }

   public class When_retrieving_the_information_and_the_properties_of_an_unknown_database_record : concern_for_GeneExpressionQueries
   {
      private string[] _information;
      private string[] _properties;

      protected override void Because()
      {
         _information = sut.GetDataBaseRecInfos("RNAseq", "UNKNOWN");
         _properties = sut.GetDataBaseRecProperties("RNAseq", "UNKNOWN");
      }

      [Observation]
      public void should_not_return_any_information() => _information.ShouldBeEmpty();

      [Observation]
      public void should_not_return_any_property() => _properties.ShouldBeEmpty();
   }
}
