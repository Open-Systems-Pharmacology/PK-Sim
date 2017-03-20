using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using NHibernate;
using OSPSuite.Infrastructure.Serialization.ORM.History;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_HistoryItemMetaDataRepository : ContextSpecificationWithSerializationDatabase<HistoryItemMetaDataRepository>
   {
      protected ISession _session;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = new HistoryItemMetaDataRepository(new CommandMetaDataRepository());
         _session = _sessionFactory.OpenSession();
      }
   }

   public class When_retrieving_all_history_items : concern_for_HistoryItemMetaDataRepository
   {
      private IEnumerable<HistoryItemMetaData> _result;
      private List<HistoryItemMetaData> _savedItems;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _savedItems = new List<HistoryItemMetaData>
         {
            new HistoryItemMetaData {Id = "1"},
            new HistoryItemMetaData {Id = "2"}
         };

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            _savedItems.Each(item => session.Save(item));
            transaction.Commit();
         }
      }

      protected override void Because()
      {
         _result = sut.All(_session);
      }

      [Observation]
      public void should_have_retrieved_all_the_saved_history_items()
      {
         _result.Count().ShouldBeEqualTo(_savedItems.Count());
      }
   }
}