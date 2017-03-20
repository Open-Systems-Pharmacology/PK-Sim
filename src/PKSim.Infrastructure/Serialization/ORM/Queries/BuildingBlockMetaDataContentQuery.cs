using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface IBuildingBlockMetaDataContentQuery : IQuery<MetaDataContent, string>
   {
   }

   public class BuildingBlockMetaDataContentQuery : MetaDataContentQuery, IBuildingBlockMetaDataContentQuery
   {
      public BuildingBlockMetaDataContentQuery(ISessionManager sessionManager) : base(sessionManager, "BuildingBlockMetaData")
      {
      }
   }
}