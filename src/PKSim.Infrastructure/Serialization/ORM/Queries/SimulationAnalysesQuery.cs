using System.Linq;
using NHibernate.Linq;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface ISimulationAnalysesQuery : IQuery<SimulationAnalysesMetaData, string>
   {
   }

   public class SimulationAnalysesQuery : ISimulationAnalysesQuery
   {
      private readonly ISessionManager _sessionManager;

      public SimulationAnalysesQuery(ISessionManager sessionManager)
      {
         _sessionManager = sessionManager;
      }

      public SimulationAnalysesMetaData ResultFor(string simulationId)
      {
         using (var session = _sessionManager.OpenSession())
         {
            var sim = session.Query<SimulationMetaData>()
               .Where(x => x.Id == simulationId)
               .Fetch(x => x.SimulationAnalyses)
               .ToList().First();

            return sim.SimulationAnalyses;
         }
      }
   }
}