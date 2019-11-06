using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using NHibernate;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Services;
using OSPSuite.Infrastructure.Services;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_lazy_loading_integration_test : ContextForIntegration<ISessionFactoryProvider>
    {
        private string _dataBaseFile;
        protected ISessionFactory _sessionFactory;
        protected byte[] _content;

       public override void GlobalContext()
        {
            base.GlobalContext();
            sut = new SessionFactoryProvider();
            _dataBaseFile = FileHelper.GenerateTemporaryFileName();
            _sessionFactory = sut.InitalizeSessionFactoryFor(_dataBaseFile);
            _content = Encoding.UTF8.GetBytes("content");
        }

        public override void GlobalCleanup()
        {
            base.GlobalCleanup();
            FileHelper.DeleteFile(_dataBaseFile);
        }
    }

    
    public class When_loading_an_existing_meta_data_from_database : concern_for_lazy_loading_integration_test
    {
        private IndividualMetaData _individualMetaData;

        protected override void Context()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                _individualMetaData = new IndividualMetaData {Id = "asdsad", Name = "toto"};
                _individualMetaData.Content.Data = _content;
                session.Save(_individualMetaData);
                transaction.Commit();
            }
        }

        [Observation]
        public void content_should_be_lazy_loaded()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var  fromDb = session.Load<IndividualMetaData>(_individualMetaData.Id);
                NHibernateUtil.IsInitialized(fromDb.Content).ShouldBeFalse();
                var content = fromDb.Content.Data;
                NHibernateUtil.IsInitialized(fromDb.Content).ShouldBeTrue();
            }

            
        }
    }
}