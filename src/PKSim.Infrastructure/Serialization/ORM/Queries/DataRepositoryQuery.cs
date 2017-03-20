using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface IDataRepositoryQuery : IQuery<DataRepositoryMetaData, string>
   {
   }

   public class DataRepositoryQuery : IDataRepositoryQuery
   {
      private readonly ISessionManager _sessionManager;

      public DataRepositoryQuery(ISessionManager sessionManager)
      {
         _sessionManager = sessionManager;
      }

      public DataRepositoryMetaData ResultFor(string simulationId)
      {
         if (!_sessionManager.IsOpen)
            return null;

         using (var session = _sessionManager.OpenSession())
         {
            var dataRepositoryId = session.CreateQuery("select simulation.DataRepositoryId from SimulationMetaData simulation where simulation.Id =:id")
               .SetString("id", simulationId)
               .UniqueResult<string>();

            if (string.IsNullOrEmpty(dataRepositoryId))
               return null;

            return session.Get<DataRepositoryMetaData>(dataRepositoryId);
         }
      }
   }
}