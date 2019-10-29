using OSPSuite.BDDHelper;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

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
                var projectMetaData = new ProjectMetaData {Id = 1, Name = "toto"};
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

   //This test is just used to dump the schema of the project database on file. For debug purpose only
   //public class Comparing_schema_files_for_debug_purpose : ContextSpecificationWithSerializationDatabase<ISessionFactoryProvider>
   //{
   //    //test only to compare xml mapping and fluent nhibernate mapping
   //    [Observation]
   //    public void should_create_the_database_schema()
   //    {
   //        var schemaExport = sut.GetSchemaExport(_dataBaseFile);
   //        schemaExport.SetOutputFile(@"C:\Tests\NHibernate\schema.txt");
   //        schemaExport.Create(true, true);
   //    }
   //}
}