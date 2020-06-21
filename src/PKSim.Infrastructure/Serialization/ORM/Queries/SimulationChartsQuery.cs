using System.Collections.Generic;
using System.Linq;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   /// <summary>
   ///    Returns all simulation charts meta data define for the simulation with the id given as parameter
   /// </summary>
   public interface ISimulationChartsQuery : IQuery<IList<SimulationChartMetaData>, string>
   {
   }

   public class SimulationChartsQuery : ISimulationChartsQuery
   {
      private readonly ISessionManager _sessionManager;

      public SimulationChartsQuery(ISessionManager sessionManager)
      {
         _sessionManager = sessionManager;
      }

      public IList<SimulationChartMetaData> ResultFor(string simulationId)
      {
         using (var session = _sessionManager.OpenSession())
         {
            return session.Query<SimulationMetaData>()
               .First(x => x.Id == simulationId)
               .Charts.ToList();
         }
      }
   }
}