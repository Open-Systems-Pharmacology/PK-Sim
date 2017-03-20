using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NHibernate;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationAnalysesQuery : ContextSpecificationWithSerializationDatabase<ISimulationAnalysesQuery>
   {
      protected ISessionManager _sessionManager;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sessionManager = A.Fake<ISessionManager>();
         sut = new SimulationAnalysesQuery(_sessionManager);
      }
   }

   public class When_retrieving_the_simulation_anaylyses_for_a_given_simulation : concern_for_SimulationAnalysesQuery
   {
      private SimulationMetaData _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         A.CallTo(() => _sessionManager.OpenSession()).ReturnsLazily(() => _sessionFactory.OpenSession());

         _simulation = new SimulationMetaData {Id = "tralala", Name = " toto"};
         _simulation.SimulationAnalyses = new SimulationAnalysesMetaData {Content = {Data = Encoding.UTF8.GetBytes("Content")}};

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_simulation);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_retrieve_the_values_saved_into_the_database()
      {
         var result = sut.ResultFor(_simulation.Id);
         Encoding.UTF8.GetString(result.Content.Data).ShouldBeEqualTo("Content");
      }

      [Observation]
      public void should_not_load_the_pk_analyses_results_when_loading_the_simulation()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var sim = session.Load<SimulationMetaData>(_simulation.Id);
            NHibernateUtil.IsInitialized(sim.SimulationAnalyses).ShouldBeFalse();
         }
      }
   }
}