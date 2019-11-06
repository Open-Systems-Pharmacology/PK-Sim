using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NHibernate;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_CommandMetaDataRepository : ContextSpecificationWithSerializationDatabase<CommandMetaDataRepository>
   {
      private ISessionManager _sessionManager;
      protected ISession _session;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sessionManager = A.Fake<ISessionManager>();
         sut = new CommandMetaDataRepository();
         _session = _sessionFactory.OpenSession();
         A.CallTo(() => _sessionManager.OpenSession()).Returns(_session);
         A.CallTo(() => _sessionManager.IsOpen).Returns(true);
      }
   }

   public class When_retrieving_all_commands : concern_for_CommandMetaDataRepository
   {
      private IEnumerable<CommandMetaData> _results;
      private List<CommandMetaData> _savedCommands;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _savedCommands = new List<CommandMetaData>
         {
            new CommandMetaData {Id = "1", CommandId = "1", Discriminator = "Label"},
            new CommandMetaData {Id = "2", CommandId = "2", Discriminator = "Label"}
         };

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            _savedCommands.Each(item => session.Save(item));
            transaction.Commit();
         }

         sut.LoadFromSession(_session);
      }

      protected override void Because()
      {
         _results = sut.All();
      }

      [Observation]
      public void all_the_saved_commands_should_be_retrieved()
      {
         _results.Count().ShouldBeEqualTo(_savedCommands.Count());
      }
   }
}