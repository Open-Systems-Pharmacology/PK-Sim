using NHibernate;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   public class WorkspaceLayoutMetaData : MetaDataWithContent<int>, IUpdatableFrom<WorkspaceLayoutMetaData>
   {
      public void UpdateFrom(WorkspaceLayoutMetaData source, ISession session)
      {
         UpdateContentFrom(source);
      }
   }
}