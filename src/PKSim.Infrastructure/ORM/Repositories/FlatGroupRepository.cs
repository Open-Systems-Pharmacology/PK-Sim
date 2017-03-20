using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatGroupRepository : IMetaDataRepository<FlatGroup>
   {
   }

   public class FlatGroupRepository : MetaDataRepository<FlatGroup>, IFlatGroupRepository
   {
      public FlatGroupRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatGroup> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewGroups)
      {
      }
   }
}