using System.Linq;
using OSPSuite.Utility.Extensions;
using NHibernate;
using NHibernate.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface ISimulationResultsQuery : IQuery<SimulationResults, string>
   {

   }

   public class SimulationResultsQuery : ISimulationResultsQuery
   {
      private readonly ISessionManager _sessionManager;

      public SimulationResultsQuery(ISessionManager sessionManager)
      {
         _sessionManager = sessionManager;
      }

      public SimulationResults LoadSimulationResultsById(int simulationResultId, ISession session)
      {
         //http://stackoverflow.com/questions/5266180/fighting-cartesian-product-x-join-when-using-nhibernate-3-0-0
         // and http://ayende.com/blog/4367/eagerly-loading-entity-associations-efficiently-with-nhibernate
         //avoid cartesian product 
         //load only simulation results
         var sim = session.Query<SimulationResults>().Where(x => x.Id == simulationResultId);
         
         //specify that we will load the collection in the future
         sim.FetchMany(x => x.AllIndividualResults).ToFuture();

         //loads all the values into another future query
         var s = session.Query<IndividualResults>()
            .Where(x => x.SimulationResults.Id == simulationResultId)
            .FetchMany(x=>x.AllValues)
            .ToFuture();

         //actually performes the query
         var simResults =  sim.ToFuture().Single();
         simResults.AllIndividualResults.Each(x => x.UpdateQuantityTimeReference());
         return simResults;
      }

      
      public SimulationResults ResultFor(string simulationId)
      {
         if (!_sessionManager.IsOpen)
            return new NullSimulationResults();

         using (var session = _sessionManager.OpenSession())
         {
            const string query = "SELECT SimulationResultsId From SIMULATIONS WHERE SimulationId=:id";
            var result = session.CreateSQLQuery(query)
               .SetParameter("id", simulationId)
               .List();

            if (result.Count == 0 || result[0] == null)
               return null;

            int internalId = result[0].ConvertedTo<int>();

            return LoadSimulationResultsById(internalId, session);
         }
      }
   }
}