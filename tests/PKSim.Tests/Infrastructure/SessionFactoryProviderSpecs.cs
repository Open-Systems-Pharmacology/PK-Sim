using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;
using PKSim.Core;

namespace PKSim.Infrastructure
{

   public class When_creating_a_session_factory_for_an_existing_given_data_source : ContextSpecificationWithSerializationDatabase<ISessionFactoryProvider>
   {
      [Observation]
      public void should_create_the_database_schema()
      {
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var projectMetaData = new ProjectMetaData { Id = 1, Name = "toto" };
            session.Save(projectMetaData);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_open_a_session()
      {
         using (var session = _sessionFactory.OpenSession())
         {
         }
      }
   }

   // Dumps the schema of the project database. This makes changes in the schema trackable through version control
   public class Comparing_schema_files_for_tracking_purpose : ContextSpecificationWithSerializationDatabase<ISessionFactoryProvider>
   {
      [Observation]
      public void should_create_the_database_schema()
      {

         var schemaExport = _sessionFactoryProvider.GetSchemaExport(_dataBaseFile);
         var directoryName = Path.GetDirectoryName(DomainHelperForSpecs.ProjectSchemaDumpFilePath);
         if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
         
         schemaExport.SetOutputFile(DomainHelperForSpecs.ProjectSchemaDumpFilePath);
         schemaExport.Create(true, true);
      }
   }
}