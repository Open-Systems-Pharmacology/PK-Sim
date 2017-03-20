using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface ISimulationComparisonMetaDataContentQuery : IQuery<MetaDataContent, string>
   {
   }

   public class SimulationComparisonMetaDataContentQuery : MetaDataContentQuery, ISimulationComparisonMetaDataContentQuery
   {
      public SimulationComparisonMetaDataContentQuery(ISessionManager sessionManager)
         : base(sessionManager, "SimulationComparisonMetaData")
      {
      }
   }
}