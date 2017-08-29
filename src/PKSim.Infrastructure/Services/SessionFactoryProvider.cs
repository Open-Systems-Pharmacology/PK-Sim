using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Services
{
   public class SessionFactoryProvider : ISessionFactoryProvider
   {
      public ISessionFactory InitalizeSessionFactoryFor(string dataSource)
      {
         var cfg = createSqlLiteConfigurationFor(dataSource);
         //Create schema for database
         new SchemaExport(cfg).Execute(false, true, false);

         return createSessionFactory(cfg);
      }

      public ISessionFactory OpenSessionFactoryFor(string dataSource)
      {
         var cfg = createSqlLiteConfigurationFor(dataSource);
         var update = new SchemaUpdate(cfg);
         update.Execute(useStdOut: false, doUpdate:true);
         return createSessionFactory(cfg);
      }

      private static ISessionFactory createSessionFactory(Configuration cfg)
      {
         var sessionFactory = cfg.BuildSessionFactory();
         sessionFactory.Evict(typeof (SimulationResults));
         sessionFactory.Evict(typeof (IndividualResults));
         return sessionFactory;
      }

      public SchemaExport GetSchemaExport(string dataSource)
      {
         var cfg = createSqlLiteConfigurationFor(dataSource);
         //Create schema for database
         return new SchemaExport(cfg);
      }

      private Configuration createSqlLiteConfigurationFor(string dataSource)
      {
         var configuration = new Configuration();
         var path = dataSource.ToUNCPath();

         configuration.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
         configuration.SetProperty("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
         configuration.SetProperty("dialect", "NHibernate.Dialect.SQLiteDialect");
         configuration.SetProperty("query.substitutions", "true=1;false=0");
         configuration.SetProperty("show_sql", "false");
         configuration.SetProperty("connection.connection_string", $"Data Source={path};Version=3;New=False;Compress=True;");

         return Fluently.Configure(configuration)
            .Mappings(cfg =>
            {
               cfg.HbmMappings.AddFromAssemblyOf<SessionFactoryProvider>();
               cfg.FluentMappings.AddFromAssemblyOf<SessionFactoryProvider>();
            }).BuildConfiguration();
      }
   }
}