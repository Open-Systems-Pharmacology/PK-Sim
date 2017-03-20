using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NHibernate;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_BuildingBlockMetaDataContentQuery : ContextSpecificationWithSerializationDatabase<IBuildingBlockMetaDataContentQuery>
   {
      private ISessionManager _sessionManager;
      private ISession _session;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sessionManager = A.Fake<ISessionManager>();
         _session = _sessionFactory.OpenSession();
         A.CallTo(() => _sessionManager.OpenSession()).Returns(_session);
         sut = new BuildingBlockMetaDataContentQuery(_sessionManager);
      }
   }

   
   public class When_retrieving_the_content_of_a_entity_meta_data_saved_into_the_database : concern_for_BuildingBlockMetaDataContentQuery
   {
      private BuildingBlockMetaData _buildingBlockMetaData;
      private MetaDataContent _contentValue;

      protected override void Context()
      {
         _buildingBlockMetaData = new IndividualMetaData();
         _buildingBlockMetaData.Id = "tralala";
         _buildingBlockMetaData.Name = " toto";
         _buildingBlockMetaData.Content.Data = Encoding.UTF8.GetBytes("content");

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_buildingBlockMetaData);
            transaction.Commit();
         }
      }

      protected override void Because()
      {
         _contentValue = sut.ResultFor(_buildingBlockMetaData.Id);
      }

      [Observation]
      public void should_be_able_to_return_the_saved_content()
      {
         _contentValue.Data.ShouldBeEqualTo(_buildingBlockMetaData.Content.Data);
      }

   }
}	