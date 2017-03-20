using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NHibernate;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_DataRepositoryQuery : ContextSpecificationWithSerializationDatabase<IDataRepositoryQuery>
   {
      protected ISessionManager _sessionManager;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sessionManager = A.Fake<ISessionManager>();
         sut = new DataRepositoryQuery(_sessionManager);
      }
   }

   public class When_retrieving_the_results_for_a_given_simulation_for_which_no_results_are_available : concern_for_DataRepositoryQuery
   {
      private SimulationMetaData _simulation;
      private ISession _session;

      protected override void Context()
      {
         base.Context();
         _session = _sessionFactory.OpenSession();
         A.CallTo(() => _sessionManager.OpenSession()).Returns(_session);

         _simulation = new SimulationMetaData();
         _simulation.Id = "ugygy";
         _simulation.Name = " toto";

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_simulation);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_return_null()
      {
         sut.ResultFor(_simulation.Id).ShouldBeNull();
      }
   }
}