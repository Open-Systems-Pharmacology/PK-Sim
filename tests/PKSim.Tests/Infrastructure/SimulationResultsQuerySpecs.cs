using FakeItEasy;
using NHibernate;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Core;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationResultsQuery : ContextSpecificationWithSerializationDatabase<ISimulationResultsQuery>
   {
      protected ISessionManager _sessionManager;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sessionManager = A.Fake<ISessionManager>();
         sut = new SimulationResultsQuery(_sessionManager);
      }
   }

   public class When_retrieving_the_simulation_results_for_a_given_simulation : concern_for_SimulationResultsQuery
   {
      private SimulationMetaData _simulation;
      private SimulationResults _simResults;
      private SimulationResults _result;
      private ISession _session;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _session = _sessionFactory.OpenSession();
         A.CallTo(() => _sessionManager.OpenSession()).Returns(_session);
         A.CallTo(() => _sessionManager.IsOpen).Returns(true);

         _simulation = new SimulationMetaData {Id = "tralala", Name = " toto"};
         _simResults = DomainHelperForSpecs.CreateSimulationResults();
         _simulation.SimulationResults = _simResults;

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_simulation);
            transaction.Commit();
         }

         _result = sut.ResultFor(_simulation.Id);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_values_saved_into_the_database()
      {
         _result.Count.ShouldBeEqualTo(_simResults.Count);
      }

      [Observation]
      public void should_have_updated_the_reference_to_time_in_each_indidividual_results()
      {
         foreach (var indResults in _result)
         {
            foreach (var value in indResults)
            {
               value.Time.ShouldBeEqualTo(indResults.Time);
            }
         }
      }
   }
}