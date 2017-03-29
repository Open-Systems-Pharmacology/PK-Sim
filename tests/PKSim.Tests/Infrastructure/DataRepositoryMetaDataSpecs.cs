using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_DataRepositoryMetaData : ContextSpecificationWithSerializationDatabase<DataRepositoryMetaData>
   {
      protected byte[] _content;

      protected override void Context()
      {
         sut = new DataRepositoryMetaData();
         sut.Id = "Id";
         sut.Name = "DataRepositoryName";
         sut.Description = "tralala";
         _content = Encoding.UTF8.GetBytes("content");
         sut.Content.Data = _content;
      }
   }

   public class When_storing_a_data_repository_with_data : concern_for_DataRepositoryMetaData
   {
      protected override void Because()
      {
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_retrieve_all_the_column()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var dataRepositoryFromDb = session.Get<DataRepositoryMetaData>(sut.Id);
            dataRepositoryFromDb.ShouldNotBeNull();
            dataRepositoryFromDb.Content.Data.ShouldBeEqualTo(_content);
         }
      }
   }
}