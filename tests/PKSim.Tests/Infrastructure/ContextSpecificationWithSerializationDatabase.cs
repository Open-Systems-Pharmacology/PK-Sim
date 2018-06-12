using OSPSuite.BDDHelper;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using NHibernate;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   [IntegrationTests]
   public abstract class ContextSpecificationWithSerializationDatabase<T> : ContextSpecification<T>
   {
      protected SessionFactoryProvider _sessionFactoryProvider;
      protected ISessionFactory _sessionFactory;
      protected string _dataBaseFile;
      private readonly bool _shouldDeleteFile;

      protected ContextSpecificationWithSerializationDatabase(string dataBaseFile = null)
      {
         _dataBaseFile = dataBaseFile;
         _shouldDeleteFile = (dataBaseFile != null);
      }

      public override void GlobalContext()
      {
         base.GlobalContext();

         _sessionFactoryProvider = new SessionFactoryProvider();

         if (string.IsNullOrEmpty(_dataBaseFile))
            _dataBaseFile = FileHelper.GenerateTemporaryFileName();

         _sessionFactory = _sessionFactoryProvider.InitalizeSessionFactoryFor(_dataBaseFile);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         if (_sessionFactory == null) return;
         _sessionFactory.Close();

         if (_shouldDeleteFile)
            FileHelper.DeleteFile(_dataBaseFile);
      }
   }
}