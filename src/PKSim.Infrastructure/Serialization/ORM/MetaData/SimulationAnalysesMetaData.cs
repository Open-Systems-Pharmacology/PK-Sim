using NHibernate;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   public class SimulationAnalysesMetaData : MetaDataWithContent<int>, IUpdatableFrom<SimulationAnalysesMetaData>
   {
      public virtual void UpdateFrom(SimulationAnalysesMetaData simulationAnalyses, ISession session)
      {
         UpdateContentFrom(simulationAnalyses);
      }
   }
}